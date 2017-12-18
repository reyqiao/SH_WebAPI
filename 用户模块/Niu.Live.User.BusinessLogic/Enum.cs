using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.BusinessLogic
{
    /// <summary>
    /// 结果返回类型
    /// </summary>
    public enum ResultType : short
    {
        None = 0,
        Success = 1,
        Error = -1
    }

    /// <summary>
    /// code返回类型
    /// </summary>
    public enum CodeType : short
    {
        #region 默认情况 无错误

        /// <summary>
        /// 默认情况 无错误
        /// </summary>
        [Description("成功")]
        Default = 0,

        #endregion

        #region 中断处理的错误 都是负数

        [Description("参数错误")]
        ParametersError = -11,

        [Description("数据库异常")]
        DbError = -12,//数据库异常

        [Description("无效的渠道号")]
        InvalidChannel = -13,

        #endregion

        #region 正常处理的情况  都是正数

        //预留 11-20
        [Description("禁止")]
        NoPermission = 11,

        [Description("已登录")]
        AlreadyLoggedIn = 12,
        [Description("未登录")]
        NotLoggedIn = 13,

        //昵称 21-30
        [Description("昵称已存在")]
        ExistNickName = 21,//存在
        [Description("昵称不存在")]
        NotExistNickName = 22,//存在
        [Description("无效昵称")]
        InvalidNickName = 23,//无效昵称

        //手机号 31-40
        [Description("手机号已存在")]
        ExistMobile = 31,//不存在
        [Description("手机号不存在")]
        NotExistMobile = 32,//不存在
        [Description("无效手机号")]
        InvalidMobile = 33,//无效手机号


        //密码 41-50
        [Description("无效密码")]
        InvalidPassword = 41,//无效密码
        [Description("两个密码不一致")]
        DifferencePassword = 42,//两个密码不一致

        //登录 51-70
        [Description("用户名或密码错误")]
        LoginError = 51, //用户名或密码错误
        [Description("用户名或密码错误次数过多已锁定")]
        LoginErrorLock = 52,//密码错误次数超过限制锁定


        [Description("用户名信息错误")]
        UserInfoError = 54,//用户名信息错误

        //短信验证码 301-350
        [Description("验证码错误")]
        SmsCodeError = 301,//验证码错误
        [Description("验证码过期")]
        SmsCodeExpire = 302,//验证码过期
        [Description("验证码下发超出最大次数")]
        SmsCodeMax = 303,//验证码下发超出最大次数
        [Description("无短信信息")]
        SmsCodeNoInfo = 304,//无短信信息

        #endregion
    }

    /// <summary>
    /// 页面功能枚举
    /// </summary>
    public enum InterfaceType : short
    {
        #region 页面功能枚举

        None = 0,
        CheckUserName = 1,
        CheckMobile = 2,

        GetVerifyCode = 20,
        GetRegisterVerifyCode = 21, //注册下发短信验证码
        GetForgotPasswordVerifyCode = 22, //找回密码下发短信验证码
        GetModifyPasswordVertifyCode = 23, //修改密码下发短信验证码

        Login = 101,
        Register = 102,
        GetUserInfo = 103,
        LoginOut = 104,
        ModifyPassword = 105,
        ResetPassword = 106

        #endregion
    }
}
