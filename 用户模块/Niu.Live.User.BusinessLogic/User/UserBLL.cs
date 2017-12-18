using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using Newtonsoft.Json;

namespace Niu.Live.User.BusinessLogic
{
    #region using

    using Niu.Cabinet;
    using Niu.Cabinet.Time;
    using Niu.Live.User.Core.Valid;
    using Niu.Live.User.Model;
    using Niu.Live.User.DataAccess.User;
    using Niu.Cabinet.Logging;
    using Niu.Live.User.DataAccess.SmsCode;
    using Niu.Live.User.Core.Password;
    using Niu.Live.User.Core.String;
    using Niu.Live.User.BusinessLogic;
    using Niu.Live.User.Core;
    using Niu.Cabinet.Conversion;
    using Niu.Live.User.BusinessLogic.User;

    #endregion

    /// <summary>
    /// 手机用户处理类
    /// </summary>
    public  class UserBLL
    {
        #region 用户注册

        #region 注册参数验证

        /// <summary>
        /// 注册参数验证
        /// </summary>
        /// <returns></returns>
        private static bool validRegisterPara(int channelId, string nickName, string mobile, string vcode, out CodeType codeType, out string errorMessage, bool isValidUserName, string password)
        {
            bool result = false;
            codeType = CodeType.Default;
            bool isExist;

            //验证手机和验证码
            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(vcode))
            {
                errorMessage = "手机号或验证码错误";
                return false;
            }

            if (channelId <=0)
            {
                errorMessage = CodeType.InvalidChannel.GetDescription();
                return false;
            }

            if (!ValidHelper.isMobile(mobile))
            {
                errorMessage = "手机号格式不正确";
                return false;
            }

            if (UserUtils.isExistMobile(channelId, mobile))
            {
                errorMessage = "手机号已存在";
                return false;
            }

            if (isValidUserName && UserUtils.isExistNickName(nickName, out isExist))
            {
                if (isExist)
                {
                    errorMessage = "昵称已被占用";
                    return false;
                }
            }

            //验证用户名
            if (isValidUserName && !UserUtils.checkNickName(nickName))
            {
                errorMessage = "非法昵称，请修改后完成注册";
                return false;
            }

            if (!PasswordHelper.CheckPassword(password))
            {
                errorMessage = "密码不符合规则";
                return false;
            }

            //检查验证码
            result = SmsCodeBLL.CheckSmsCode(channelId, mobile, vcode, (short)InterfaceType.GetRegisterVerifyCode, out codeType, out errorMessage);
            if (!result)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 注册用户信息

        /// <summary>
        /// 注册用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool Register(int channelId, string nickName, string mobile, string password,  out string errorMessage, out long outUserId, int userType = 0, int updateStatus = 0)
        {
            bool result = false;
            errorMessage = string.Empty;
            UserInfo userInfo = new UserInfo();
            long userId = 0L;
            outUserId = 0L;
            DateTime addTime;

            long mobileValue = ObjectConvert.ChangeType<long>(mobile, 0L);

            bool isValidSuccess = SmsCodeBLL.validVerifyCode(channelId, mobileValue, (int)InterfaceType.GetRegisterVerifyCode, out errorMessage);

            if (!isValidSuccess) return false;

            bool isSuccess = UserAccess.UserAdd(channelId, nickName, Niu.Cabinet.Cryptography.NiuCryptoService.EncryptPassword(mobile), password, out userId, out addTime, out errorMessage, userType);

            if (isSuccess && userId > 0)
            {
                outUserId = userId;
                int level = 0;
                PasswordHelper.CheckPassword(password, out level);

                var userDigest = PasswordRules.GenerateUserDigest(userId, Niu.Cabinet.Time.TimeStmap.DateTimeToUnixTimeStmap(addTime));
                var securityCode = PasswordRules.GenerateSecurityCode(userDigest, password);

                result = UserAccess.UpdatePassword(userId, UserUtils.passwordMD5(userId, addTime, password), level, out errorMessage, securityCode, StringHelper.RandomString(4));
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="userName"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string userRegister(RequestInfo requestInfo, int channelId, string nickName, string mobile, string password, string vcode, int userType = 0, int updateStatus = 0, bool isValidUserName = true)
        {
            CodeType codeType;
            string errorMessage = string.Empty;
            BaseResult result = new BaseResult() { result = (int)ResultType.None, code = (int)CodeType.ParametersError };

            try
            {
                if (!validRegisterPara(channelId, nickName, mobile, vcode, out codeType, out errorMessage, isValidUserName, password))
                {
                    result.message = errorMessage;
                    return result.ToString();
                }

                bool isSuccess = false;
                long userId = 0L;

                //用户注册
                isSuccess = Register(channelId, nickName, mobile, password, out errorMessage, out userId, userType, updateStatus);

                if (!isSuccess)
                {
                    result.result = (int)ResultType.Error;
                    result.code = (int)CodeType.DbError;
                    result.message = errorMessage;
                    return result.ToString();
                }

                UserInfo uvm = UserUtils.getUserInfoById(userId);
                
                //用户令牌
                string userToken = string.Empty;
                uvm.tokenType = Model.tokenType.Formal;
                uvm.channelId = channelId;
                TokenManager.GenerateMobileUserToken(UserUtils.convertToTokenUserInfo(uvm), out userToken);

                //返回用户
                var userDigest = PasswordRules.GenerateUserDigest(uvm.userId, TimeStmap.DateTimeToUnixTimeStmap(uvm.addTime));

                //返回登录信息
                dynamic response = new ExpandoObject();
                dynamic userInfo = new ExpandoObject();

                response.result = (int)ResultType.Success;
                response.code = CodeType.Default;
                response.message = "注册成功";

                response.userInfo = userInfo;
                response.userInfo.state = uvm.status;
                response.userInfo.userId = uvm.userId;
                response.userInfo.nickName = uvm.nickName;
                response.userInfo.userLogoUrl = uvm.logoPhotoUrl;
                response.userInfo.userToken = userToken;
                response.userInfo.roleId = uvm.roleId;
                response.userInfo.roleName = uvm.roleName;

                byte[] bt = Encoding.GetEncoding("utf-8").GetBytes(mobile);
                var base64Mobile = Convert.ToBase64String(bt);
                response.userInfo.mobile = base64Mobile;

                return JsonConvert.SerializeObject(response);
            }
            catch(Exception ex)
            {
                LogRecord log = new LogRecord(Constant.appSettingKey);
                log.WriteSingleLog("userRegister", string.Format("error:{0}",ex.ToString()));
                return result.ToString();
            }
        }

        #endregion

        #region 用户添加

        #region 用户添加参数验证

        /// <summary>
        /// 注册参数验证
        /// </summary>
        /// <returns></returns>
        private static bool validUserAddPara(int channelId, string nickName, string mobile, out CodeType codeType, out string errorMessage, bool isValidUserName, string password)
        {
            codeType = CodeType.Default;
            errorMessage = string.Empty;
            bool isExist;

            //验证手机和验证码
            if (string.IsNullOrEmpty(mobile))
            {
                errorMessage = "手机号错误";
                return false;
            }

            if (channelId <= 0)
            {
                errorMessage = CodeType.InvalidChannel.GetDescription();
                return false;
            }

            if (!ValidHelper.isMobile(mobile))
            {
                errorMessage = "手机号格式不正确";
                return false;
            }

            if (UserUtils.isExistMobile(channelId, mobile))
            {
                errorMessage = "手机号已存在";
                return false;
            }

            if (isValidUserName && UserUtils.isExistNickName(nickName, out isExist))
            {
                if (isExist)
                {
                    errorMessage = "昵称已被占用";
                    return false;
                }
            }

            //验证用户名
            if (isValidUserName && !UserUtils.checkNickName(nickName))
            {
                errorMessage = "非法昵称，请修改后完成注册";
                return false;
            }

            if (!PasswordHelper.CheckPassword(password))
            {
                errorMessage = "密码不符合规则";
                return false;
            }

            return true;
        }

        #endregion

        #region 用户添加

        /// <summary>
        /// 用户添加
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool userAdd(int channelId, string nickName, string mobile, string password, out string errorMessage, out long outUserId, int userType = 0, int updateStatus = 0)
        {
            bool result = false;
            errorMessage = string.Empty;
            UserInfo userInfo = new UserInfo();
            long userId = 0L;
            outUserId = 0L;
            DateTime addTime;

            long mobileValue = ObjectConvert.ChangeType<long>(mobile, 0L);

            bool isSuccess = UserAccess.UserAdd(channelId, nickName, Niu.Cabinet.Cryptography.NiuCryptoService.EncryptPassword(mobile), password, out userId, out addTime, out errorMessage, userType);

            if (isSuccess && userId > 0)
            {
                outUserId = userId;
                int level = 0;
                PasswordHelper.CheckPassword(password, out level);

                var userDigest = PasswordRules.GenerateUserDigest(userId, Niu.Cabinet.Time.TimeStmap.DateTimeToUnixTimeStmap(addTime));
                var securityCode = PasswordRules.GenerateSecurityCode(userDigest, password);

                result = UserAccess.UpdatePassword(userId, UserUtils.passwordMD5(userId, addTime, password), level, out errorMessage, securityCode, StringHelper.RandomString(4));
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="userName"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string userAdd(RequestInfo requestInfo, int channelId, string nickName, string mobile, string password, int userType = 0, int updateStatus = 0, bool isValidUserName = true)
        {
            CodeType codeType;
            string errorMessage = string.Empty;
            BaseResult result = new BaseResult() { result = (int)ResultType.None, code = (int)CodeType.ParametersError };

            try
            {
                if (!validUserAddPara(channelId, nickName, mobile,out codeType, out errorMessage, isValidUserName, password))
                {
                    result.message = errorMessage;
                    return result.ToString();
                }

                bool isSuccess = false;
                long userId = 0L;

                //用户注册
                isSuccess = userAdd(channelId, nickName, mobile, password, out errorMessage, out userId, userType, updateStatus);

                if (!isSuccess)
                {
                    result.result = (int)ResultType.Error;
                    result.code = (int)CodeType.DbError;
                    result.message = errorMessage;
                    return result.ToString();
                }

                UserInfo uvm = UserUtils.getUserInfoById(userId);

                //用户令牌
                string userToken = string.Empty;
                uvm.tokenType = Model.tokenType.Formal;
                uvm.channelId = channelId;
                TokenManager.GenerateMobileUserToken(UserUtils.convertToTokenUserInfo(uvm), out userToken);

                //返回用户
                var userDigest = PasswordRules.GenerateUserDigest(uvm.userId, TimeStmap.DateTimeToUnixTimeStmap(uvm.addTime));

                //返回登录信息
                dynamic response = new ExpandoObject();

                response.result = (int)ResultType.Success;
                response.code = CodeType.Default;
                response.message = "添加成功";

                return JsonConvert.SerializeObject(response);
            }
            catch (Exception ex)
            {
                LogRecord log = new LogRecord(Constant.appSettingKey);
                log.WriteSingleLog("userRegister", string.Format("error:{0}", ex.ToString()));
                return result.ToString();
            }
        }

        #endregion

        #region 自动注册用户信息(历史数据)

        /// <summary>
        /// 自动注册用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool autoRegister(int channelId, string nickName, string mobile, string password, out string errorMessage, out long outUserId, int userType = 0, int updateStatus = 0,int gender = 0,int roleId = 1)
        {
            bool result = false;
            errorMessage = string.Empty;
            UserInfo userInfo = new UserInfo();
            long userId = 0L;
            outUserId = 0L;
            DateTime addTime;

            bool isSuccess = UserAccess.UserAdd(channelId, nickName, Niu.Cabinet.Cryptography.NiuCryptoService.EncryptPassword(mobile), password, out userId, out addTime, out errorMessage, userType,gender,roleId);

            if (isSuccess && userId > 0)
            {
                outUserId = userId;
                int level = 0;
                PasswordHelper.CheckPassword(password, out level);

                var userDigest = PasswordRules.GenerateUserDigest(userId, Niu.Cabinet.Time.TimeStmap.DateTimeToUnixTimeStmap(addTime));
                var securityCode = PasswordRules.GenerateSecurityCode(userDigest, password);

                result = UserAccess.UpdatePassword(userId, UserUtils.passwordMD5(userId, addTime, password), level, out errorMessage, securityCode, StringHelper.RandomString(4));
            }

            return result;
        }

        #endregion

        #region 用户登录

        #region 登录参数验证

        /// <summary>
        /// 登录参数验证
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private static bool validLoginPara(RequestInfo requestInfo, int channelId, string mobile, string password, out UserInfo uvm, out string userToken, out BaseResult result)
        {
            uvm = null;
            userToken = string.Empty;
            long userId = 0L;
            bool isSuccess = false;
            bool isValidPassword = true;
            result = new BaseResult() { result = (int)ResultType.None, code = (int)CodeType.ParametersError };

            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(password))
            {
                result.code = (int)CodeType.ParametersError;
                result.message = CodeType.ParametersError.GetDescription();
                return false;
            }

            uvm = UserUtils.getUserInfoByMobile(channelId,mobile);
            if (uvm == null || uvm.userId <= 0) //没有此用户信息
            {
                isSuccess = OldUserBLL.userDealWith(mobile, password, out userId);
                if(isSuccess && userId >0)
                {
                    uvm = UserUtils.getUserInfoById(userId);
                    isValidPassword = false;
                    goto ctn;
                }

                result.code = (int)CodeType.LoginError;
                result.message = CodeType.LoginError.GetDescription();
                return false;
            }

            if(uvm.status == -1)
            {
                result.code = (int)CodeType.LoginError;
                result.message = "您的账号已无权限。请联系管理员";

                return false;
            }

            bool isBlackUser = UserUtils.isBlackUser(uvm.userId);

            if(isBlackUser)
            {
                result.code = (int)CodeType.LoginError;
                result.message = "您已被拉黑，请联系管理员";
                return false;
            }

            ctn:
            //生成令牌
            uvm.tokenType = Model.tokenType.Formal;
            uvm.channelId = channelId;
            TokenManager.GenerateMobileUserToken(UserUtils.convertToTokenUserInfo(uvm), out userToken);

            string pwd = UserUtils.passwordMD5(uvm.userId, uvm.addTime, password);
            //新的验证方式
            if (isValidPassword && pwd != uvm.password)
            {
                LogUserActionFacade.UserLoginFailedRecord(requestInfo, (int)CodeType.LoginError, uvm.userId, uvm.nickName, CodeType.LoginError.GetDescription());
                result.code = (int)CodeType.LoginError;
                result.message = CodeType.LoginError.GetDescription();
                return false;
            }



            return true;
        }

        #endregion

        public static string userLogin(RequestInfo requestInfo, int channelId, string mobile, string password)
        {
            UserInfo uvm = null;
            string userToken = string.Empty; //用户令牌
            string errorMessage = string.Empty;
            BaseResult result = null;

            //登录参数验证
            if (!validLoginPara(requestInfo, channelId, mobile, password, out uvm, out userToken, out result))
            {
                return result.ToString();
            }

            var userDigest = PasswordRules.GenerateUserDigest(uvm.userId, TimeStmap.DateTimeToUnixTimeStmap(uvm.addTime));

            //返回登录信息
            dynamic response = new ExpandoObject();
            dynamic userInfo = new ExpandoObject();

            response.result = (int)ResultType.Success;
            response.code = CodeType.Default;
            response.message = "登录成功";

            response.userInfo = userInfo;
            response.userInfo.state = uvm.status;
            response.userInfo.userId = uvm.userId;
            response.userInfo.userName = uvm.nickName;
            response.userInfo.userLogoUrl = uvm.logoPhotoUrl;
            response.userInfo.userToken = userToken;
            response.userInfo.roleId = uvm.roleId;
            response.userInfo.roleName = uvm.roleName;

            byte[] bt = Encoding.GetEncoding("utf-8").GetBytes(mobile);
            var base64Mobile = Convert.ToBase64String(bt);

            response.userInfo.mobile = base64Mobile;

            //添加登录日志
            Task.Factory.StartNew(() =>
            {
                LogUserActionFacade.UserLoginSuccessRecord(requestInfo, uvm.userId);
                UserAccess.UpdateLastVisitTime(uvm.userId,requestInfo.UserIP);
            });

            return JsonConvert.SerializeObject(response);
        }

        #endregion

        #region 用户修改密码

        #region 用户修改密码参数验证

        /// <summary>
        /// 用户修改密码参数验证
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="oldPwd"></param>
        /// <param name="newPwd"></param>
        /// <param name="user"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool validParaModify(RequestInfo requestInfo, string oldPwd, string newPwd, TokenUserInfo user, out BaseResult result)
        {
            result = new BaseResult() { result = (int)ResultType.Error, code = (int)CodeType.ParametersError };

            if (string.IsNullOrEmpty(oldPwd) || string.IsNullOrEmpty(newPwd) || oldPwd == newPwd)
            {
                result.message = CodeType.InvalidPassword.GetDescription();
                return false;
            }

            if (user == null || user.userId == 0)
            {
                result.code = (int)CodeType.UserInfoError;
                result.message = CodeType.UserInfoError.GetDescription();
                return false;
            }

            if (!PasswordHelper.CheckPassword(newPwd))
            {
                result.code = (int)CodeType.InvalidPassword;
                result.message = "密码不符合规则";
                return false;
            }

            return true;
        }

        #endregion

        public static string modifyPassword(RequestInfo requestInfo, string oldPwd, string newPwd, TokenUserInfo user)
        {
            //变量
            BaseResult result = new BaseResult();

            if (!validParaModify(requestInfo, oldPwd, newPwd, user, out result))
                return result.ToString();

            string strResponse = string.Empty;
            bool isSuccess = false;
            int passwordLevel = PasswordHelper.GetPasswordLevel(newPwd);  
            UserInfo model = UserUtils.getUserInfoById(user.userId);

            //验证以前密码是否正确
            string password = PasswordRules.PasswordMd5(user.userId, model.addTime, oldPwd);
            if (model.password != password)
            {
                result.result = (int)ResultType.Error;
                result.code = (int)CodeType.InvalidPassword;
                result.message = CodeType.InvalidPassword.GetDescription();
                return result.ToString();
            }

            //生成Rcode和securityCode
            string Rcode = StringHelper.RandomString(4);
            string securityCode = string.Empty;

            var userDigest = PasswordRules.GenerateUserDigest(model.userId, TimeStmap.DateTimeToUnixTimeStmap(model.addTime));
            securityCode = PasswordRules.GenerateSecurityCode(userDigest, newPwd);

            //更新密码
            isSuccess = UserUtils.updatePassword(user.userId, model.addTime, newPwd, passwordLevel, securityCode, Rcode);

            if (!isSuccess)
            {
                result.result = (int)ResultType.Error;
                result.code = (int)CodeType.DbError;
                result.message = CodeType.DbError.GetDescription();
                return result.ToString();
            }

            string errorMessage = string.Empty;
            //添加登录日志
            Task.Factory.StartNew(() =>
            {
                LogUserActionFacade.UserActionSuccessRecord(requestInfo, user.userId, (int)InterfaceType.ModifyPassword);
            });

            result.result = (int)ResultType.Success;
            result.code = (int)CodeType.Default;
            result.message = CodeType.Default.GetDescription();

            return result.ToString();
        }

        #endregion

        #region 用户重置密码

        #region 用户重置密码参数验证

        /// <summary>
        /// 用户重置密码参数验证
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <param name="newPwd"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool validParaReset(RequestInfo requestInfo, int channelId, string mobile, string code, string newPwd, out BaseResult result)
        {
            result = new BaseResult() { result = (int)ResultType.Error, code = (int)CodeType.ParametersError };
            string errorMessage = string.Empty;
            CodeType codeType;

            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(code))
            {
                result.message = "手机号或短信码不能为空";
                return false;
            }

            if (channelId <= 0)
            {
                result.message = CodeType.InvalidChannel.GetDescription();
                return false;
            }

            if (string.IsNullOrEmpty(newPwd) || !PasswordHelper.CheckPassword(newPwd))
            {
                result.code = (int)CodeType.InvalidPassword;
                result.message = "密码不符合规则";
                return false;
            }

            //检查验证码
            var checkCode = SmsCodeBLL.CheckSmsCode(channelId, mobile, code, (short)InterfaceType.GetForgotPasswordVerifyCode, out codeType, out errorMessage);
            if (!checkCode)
            {
                result.message = errorMessage;
                return false;
            }

            return true;
        }

        #endregion

        public static string resetPassword(RequestInfo requestInfo,int channelId, string mobile, string code, string newPwd)
        {
            //变量
            BaseResult result = new BaseResult();

            if (!validParaReset(requestInfo,channelId, mobile, code, newPwd, out result))
                return result.ToString();

            bool isSuccess = false;
            string strResponse = string.Empty;
            int passwordLevel = PasswordHelper.GetPasswordLevel(newPwd);

            UserInfo model = UserUtils.getUserInfoByMobile(channelId, mobile);

            if (model == null || model.userId == 0)
            {
                result.result = (int)ResultType.None;
                result.code = (int)CodeType.ParametersError;
                result.message = "该手机号不存在";

                return result.ToString();
            }

            //生成Rcode和securityCode
            string Rcode = StringHelper.RandomString(4);
            string securityCode = string.Empty;
            var userDigest = PasswordRules.GenerateUserDigest(model.userId, TimeStmap.DateTimeToUnixTimeStmap(model.addTime));
            securityCode = PasswordRules.GenerateSecurityCode(userDigest, newPwd);

            //更新密码
            isSuccess = UserUtils.updatePassword(model.userId, model.addTime, newPwd, passwordLevel, securityCode, Rcode);

            if (!isSuccess)
            {
                result.result = (int)ResultType.Error;
                result.code = (int)CodeType.DbError;
                result.message = CodeType.DbError.GetDescription();
                return result.ToString();
            }

            string errorMessage = string.Empty;
            //添加登录日志
            Task.Factory.StartNew(() =>
            {
                LogUserActionFacade.UserActionSuccessRecord(requestInfo, model.userId, (int)InterfaceType.ResetPassword);
            });

            result.result = (int)ResultType.Success;
            result.code = (int)CodeType.Default;
            result.message = CodeType.Default.GetDescription();

            return result.ToString();
        }

        #endregion

        #region 获取用户信息
        
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public static string getUserInfo(string userToken, long userId)
        {
            TokenUserInfo user = null;
            TokenManager.ValidateMobileUserToken(userToken, out user);

            BaseResult result = new BaseResult() { result = (int)ResultType.Error, code = (int)CodeType.UserInfoError, message = "要查询的信息错误" };

            if(user == null || user.userId <= 0) return result.ToString();
            if (userId <=0) userId = user.userId;
            if(userId <=0) return result.ToString();

            UserInfo uvm = UserUtils.getUserInfoById(userId);

            if (uvm == null) return result.ToString();

            //返回登录信息
            dynamic response = new ExpandoObject();
            dynamic userInfo = new ExpandoObject();

            response.result = (int)ResultType.Success;
            response.code = CodeType.Default;
            response.message = "获取成功";

            response.userInfo = userInfo;
            response.userInfo.status = uvm.status;
            response.userInfo.userId = uvm.userId;
            response.userInfo.nickName = uvm.nickName;
            response.userInfo.userLogoUrl = uvm.logoPhotoUrl;

            response.userInfo.roleId = uvm.roleId;
            response.userInfo.roleName = uvm.roleName;



            return JsonConvert.SerializeObject(response);
        }

        #endregion

        #region 获取用户登录信息

        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static string getLoginInfo(int channelId, string mobile)
        {
            BaseResult result = new BaseResult() { result = (int) ResultType.Error, code = (int) CodeType.ParametersError, message = CodeType.ParametersError.GetDescription() };
            string userDigest = string.Empty;

            if (string.IsNullOrEmpty(mobile) || channelId <= 0) return result.ToString();

            try
            {
                UserInfo uvm = UserAccess.GetByMobile(channelId, Niu.Cabinet.Cryptography.NiuCryptoService.EncryptPassword(mobile));

                if (uvm != null)
                    userDigest = PasswordRules.GenerateUserDigest(uvm.userId, Niu.Cabinet.Time.TimeStmap.DateTimeToUnixTimeStmap(uvm.addTime));
            }
            catch(Exception ex)
            {
                LogRecord log = new LogRecord(Constant.appSettingKey);
                log.WriteSingleLog("getLoginInfo", string.Format("error:{0}", ex.ToString())); 
            }

            dynamic response = new ExpandoObject();
            dynamic userInfo = new ExpandoObject();

            response.result =  (int)ResultType.Success;
            response.code =  CodeType.Default;
            response.message = "操作成功";

            response.userInfo = userInfo;
            response.userInfo.userDigest = userDigest;

            return JsonConvert.SerializeObject(response);
        }

        #endregion
    }
}
