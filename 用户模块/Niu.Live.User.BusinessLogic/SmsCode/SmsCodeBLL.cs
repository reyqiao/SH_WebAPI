using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;

namespace Niu.Live.User.BusinessLogic
{
    #region using

    using Niu.Live.User.Model;
    using Niu.Live.User.Core.Valid;
    using Niu.Live.User.DataAccess.SmsCode;
    using Niu.Base.MobileService.Provider;
    using Niu.Cabinet.Conversion;

    #endregion

    /// <summary>
    /// 短信发送类
    /// </summary>
    public class SmsCodeBLL
    {
        #region 获得随机验证码4位数字

        /// <summary>
        /// 获得随机验证码4位数字
        /// </summary>
        /// <returns></returns>
        public static string GetSmsCode()
        {
            System.Random rdm = new System.Random();
            return rdm.Next(0, 9999).ToString("0000");
        }

        #endregion

        #region 判断是否还可以发送短信

        /// <summary>
        /// 判断是否还可以发送短信
        /// </summary>
        /// <param name="mobileLong"></param>
        /// <param name="interfaceType"></param>
        /// <param name="todaySendNumber"></param>
        /// <param name="spacingSeconds"></param>
        /// <returns></returns>
        public static bool IsCanSend(int channelId, long mobileLong, int interfaceType, out int todaySendNumber, out double spacingSeconds,out string code)
        {
            todaySendNumber = 0;
            spacingSeconds = double.MaxValue;
            string errorMessage = string.Empty;
            code = string.Empty;

            getSendNumAndSpacs(channelId, mobileLong, interfaceType, out todaySendNumber, out spacingSeconds, out code);

            if (todaySendNumber >= 5) return false;
            if (spacingSeconds < 50) return false;

            return true;
        }

        #region 获取发送数量和间隔

        /// <summary>
        /// 获取发送数量和间隔
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="interfaceType"></param>
        /// <param name="todaySendNumber"></param>
        /// <param name="spacingSeconds"></param>
        /// <param name="code"></param>
        public static void getSendNumAndSpacs(int channelId,long mobile, int interfaceType, out int todaySendNumber, out double spacingSeconds, out string code)
        {
            todaySendNumber = 0;
            spacingSeconds = double.MaxValue;
            code = string.Empty;
            string errorMessage = string.Empty;
            DateTime updateTime = DateTime.Now.AddDays(1);
            DateTime now = DateTime.Now;
            VerificationCode verifyCodeModel = null;

            if (VerificationCodeAccess.GetByMobile(channelId, mobile, interfaceType, out verifyCodeModel, out errorMessage))
            {
                if (verifyCodeModel != null)
                {
                    code = verifyCodeModel.verificationCode;
                    updateTime = verifyCodeModel.updateTime;
                    if (updateTime.ToString("yyyyMMdd") == now.ToString("yyyyMMdd"))
                    {
                        todaySendNumber = verifyCodeModel.sendNumber;
                        spacingSeconds = (now - updateTime).TotalSeconds;
                    }
                }
            }
            return;
        }

        #endregion

        #endregion

        #region 验证短信验证码

         //<summary>
         //验证短信验证码
         //</summary>
         //<param name="mobile"></param>
         //<param name="verifyCode"></param>
         //<param name="interfaceType"></param>
         //<param name="codeType"></param>
         //<param name="errorMessage"></param>
         //<returns></returns>
        public static bool CheckSmsCode(int channelId, string mobile, string verifyCode, int interfaceType, out CodeType codeType, out string errorMessage,bool isVertify = true)
        {
            codeType = CodeType.Default;
            errorMessage = string.Empty;

            long mobileNumber = ObjectConvert.ChangeType<long>(mobile, 0L);

            if (!ValidHelper.isMobile(mobile) || mobileNumber < 0)
            {
                codeType = CodeType.InvalidMobile;
                errorMessage = "手机格式错误";
                return false;
            }

            VerificationCode verifyCodeModel = null;
            if (VerificationCodeAccess.GetByMobile(channelId, mobileNumber, interfaceType, out verifyCodeModel, out errorMessage) && verifyCodeModel != null && verifyCodeModel.checkTime >20)
            {
                codeType = CodeType.SmsCodeError;
                errorMessage = "短信验证码失效";
                return false;
            }

            DateTime effectTime = DateTime.Now.AddMinutes(-20);
            bool isGetCodeSuccess = VerificationCodeAccess.GetByMobileCode(channelId, mobileNumber, verifyCode, effectTime, interfaceType, out verifyCodeModel, out errorMessage, isVertify);

            if (!isGetCodeSuccess || verifyCodeModel == null)
            {
                codeType = CodeType.SmsCodeError;
                errorMessage = "短信验证码错误";
                return false;
            }

            int isVerification = verifyCodeModel.isVerification;
            string VerificationCode = verifyCodeModel.verificationCode;
            DateTime updateTime = verifyCodeModel.updateTime;

            if(isVertify)
            {
                if (isVerification != 1)
                {
                    codeType = CodeType.SmsCodeError;
                    errorMessage = "短信验证码错误";
                    return false;
                }
            }

            if (updateTime < effectTime)
            {
                codeType = CodeType.SmsCodeExpire;
                errorMessage = "短信验证码超时";
                return false;
            }

            if (verifyCode == VerificationCode)
            {
                codeType = CodeType.Default;
                return true;
            }
            else
            {
                codeType = CodeType.SmsCodeError;
                return false;
            }
        }

        #endregion

        #region 发送验证码

        #region 发送参数验证

        /// <summary>
        /// 发送参数验证
        /// </summary>
        /// <returns></returns>
        private static bool validParaSendCode(string mobile,int channelId,int interfaceType, out string errorMessage)
        {
            bool result = true;
            bool isExist = false;
            errorMessage = string.Empty;

            if (!ValidHelper.isMobile(mobile))
            {
                errorMessage = "手机号码格式错误";
                return result;
            }

            //增加注册验证手机
            if (interfaceType == (int)InterfaceType.GetRegisterVerifyCode)
            {
                isExist = UserUtils.isExistMobile(channelId, mobile);
                if (isExist)
                {
                    errorMessage = "手机号已被占用";
                    return result;
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="mobile"></param>
        /// <param name="userIP"></param>
        /// <param name="interfaceType"></param>
        /// <param name="errorMessage"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public static bool SendCode(RequestInfo requestInfo, int channelId, string mobile, string userIP, int interfaceType, out string errorMessage)
        {
            errorMessage = "完成";

            //参数验证
            if(!validParaSendCode(mobile,channelId,interfaceType,out errorMessage))
                return false;

            long mobileLong = ObjectConvert.ChangeType<long>(mobile, 0L);
            int todaySendNumber = 0;
            double spacingSeconds = 0d;
            string code = string.Empty;

            bool isCanSend = IsCanSend(channelId, mobileLong, interfaceType, out todaySendNumber, out spacingSeconds, out code);

            if(!isCanSend)
            {
                if (todaySendNumber >= 5)
                {
                    errorMessage = "亲，您今天发送短信已超过5次，请明天再试";
                    return false;
                }
                if (spacingSeconds < 50)  return true;
            }

            code = SmsCodeBLL.GetSmsCode();

            bool isAddSuccess = VerificationCodeAccess.Add(channelId, mobileLong, code, todaySendNumber, userIP, interfaceType, out errorMessage);

            if(!isAddSuccess)
            {
                errorMessage = "操作异常";
                return false;
            }

            int nowTodaySendNumber = 0;
            SmsCodeBLL.getSendNumAndSpacs(channelId, mobileLong, interfaceType, out nowTodaySendNumber, out spacingSeconds, out code);

            if (nowTodaySendNumber - todaySendNumber != 1)
            {
                errorMessage = "请求超频";
                return false;
            }

            string sendContent = string.Format("【汇交易】验证码{0}，如非本人操作请忽略", code);

            bool isSendSuccess = SmsCode.sendCode(Base.MobileService.Models.MobileServiceChannel.Entinfo, mobileLong.ToString(), sendContent, requestInfo.UserIP, out errorMessage, "", "SDK-BBX-010-25934", "53761c-B3ce");

            if(!isSendSuccess)
            {
                VerificationCodeAccess.Subtraction(channelId, mobileLong, interfaceType, out errorMessage);
                errorMessage = "发送失败";
                return false;
            }

            return true;
        }

        #endregion

        #region 短信验证

        /// <summary>
        /// 短信验证
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="mobileValue"></param>
        /// <param name="interfaceType"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool validVerifyCode(int channelId,long mobileValue,int interfaceType, out string errorMessage)
        {
            VerificationCode vcm = null;

            if (!VerificationCodeAccess.GetByMobile(channelId, mobileValue,interfaceType, out vcm, out errorMessage))
            {
                errorMessage = "验证错误";
                return false;
            }

            if (vcm == null)
            {
                errorMessage = "无手机号码验证信息";
                return false;
            }

            int isVerification = vcm.isVerification;
            string code = vcm.verificationCode;
            string ip = vcm.userIP;
            DateTime updateTime = vcm.updateTime;

            if (isVerification != 1)
            {
                errorMessage = "验证错误";
                return false;
            }

            if (updateTime.AddHours(2) < DateTime.Now)
            {
                errorMessage = "验证超时";
                return false;
            }

            return true;
        }

        #endregion
    }
}