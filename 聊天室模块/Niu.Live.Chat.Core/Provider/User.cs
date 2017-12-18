using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;

namespace Niu.Live.Chat.Core.Provider
{
    public class User
    {

        public static Model.Im_User NeteaseIm_Get(long userId)
        {
            Model.Im_User user = Access.User.UserQueryById(userId);
            return user;
        }

        public static bool NeteaseIm_CreateRobot(string accid, string userName)
        {
            string json = NeteaseIm.Implements.User_Create(accid, userName, "");
            if (json.Contains("200"))
            {
                var jObject = JObject.Parse(json);
                string token = (string)jObject["info"]["token"];
                return Access.User.UserInsert(accid, userName, token, 0) > 0;
            }
            return false;
        }


        public static Model.Im_User NeteaseIm_Create(string userId, string userName, string userLogoUrl, int channid = 0, int usertype = 0)
        {
            string accid = CreateImeAccId();
            Model.Im_User user = Access.User.UserQueryById(long.Parse(userId));
            if (user == null)
            {
                //通过netease im api 创建用户


                string json = NeteaseIm.Implements.User_Create(accid, userName, userLogoUrl);
                //{"desc":"already register","code":414}

                var jObject = JObject.Parse(json);

                if (jObject != null && jObject["code"] != null)
                {
                    string code = (string)jObject["code"];
                    string desc = string.Empty;
                    if (code == "200")
                    {
                        string token = (string)jObject["info"]["token"];
                        user = new Model.Im_User { accid = accid, name = userName, token = token, blockFlag = false };

                        int returnValue = Access.User.UserInsert(accid, userName, token, long.Parse(userId));
                        if (returnValue <= 0) Access.User.UserFailureLogInsert(userId.ToString(), userName, token, string.Empty, "storage failure");
                    }
                    else
                    {
                        if (jObject["desc"] != null) desc = (string)jObject["desc"];
                        Access.User.UserFailureLogInsert(userId, userName, string.Empty, code, desc);
                    }
                }

            }
            return user;
        }

        public static void NeteaseIm_Update(long userId, string userName, string userLogoUrl)
        {
        }



        public static string NeteaseIm_RefreshToken(long userId)
        {
            string token = string.Empty;
            string json = NeteaseIm.Implements.User_RefreshToken(userId.ToString());

            var jObject = JObject.Parse(json);
            if (jObject != null && jObject["code"] != null && (string)jObject["code"] == "200")
            {
                token = (string)jObject["info"]["token"];

                dynamic parameters = new ExpandoObject();
                parameters.token = token;

                Model.Im_User user = NeteaseIm_Get(userId);
                if (user != null && !string.IsNullOrEmpty(user.accid))
                {
                    Access.User.UserUpdateColumn(userId.ToString(), parameters);
                }
            }
            return token;
        }



        /// <summary>
        /// 是否踢掉被禁用户，true或false，默认false
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="needkick"></param>
        /// <param name="niuguAdmin"></param>
        /// <param name="niuguRemark"></param>
        /// <returns></returns>
        //public static bool NeteaseIm_Block(string userId, string needkick = "", long niuguAdmin = 0, string niuguRemark = "")
        //{
        //    Model.Im_User user = NeteaseIm_Get(userId);
        //    if (user != null && !string.IsNullOrEmpty(user.accid) && user.blockFlag == false)
        //    {
        //        string json = NeteaseIm.Implements.User_Block(userId.ToString());
        //        var jObject = JObject.Parse(json);
        //        if (jObject != null && jObject["code"] != null && (string)jObject["code"] == "200")
        //        {
        //            return Access.User.UserBlockUpdate(userId.ToString(), true, niuguAdmin, niuguRemark) > 0 ? true : false;
        //        }
        //    }
        //    return false;
        //}



        //public static bool NeteaseIm_Unblock(string userId, long niuguAdmin = 0, string niuguRemark = "")
        //{
        //    Model.Im_User user = NeteaseIm_Get(userId);
        //    if (user != null && !string.IsNullOrEmpty(user.accid) && user.blockFlag == true)
        //    {
        //        string json = NeteaseIm.Implements.User_Unblock(userId.ToString());
        //        var jObject = JObject.Parse(json);
        //        if (jObject != null && jObject["code"] != null && (string)jObject["code"] == "200")
        //        {
        //            return Access.User.UserBlockUpdate(userId.ToString(), true, niuguAdmin, niuguRemark) > 0 ? true : false;
        //        }
        //    }
        //    return false;
        //}






        //public static bool NeteaseIm_UpdateUserInfo(string userId, string userName, string userLogoUrl)
        //{
        //    Model.Im_User user = NeteaseIm_Get(userId);
        //    if (user != null && !string.IsNullOrEmpty(user.accid))
        //    {
        //        string json = NeteaseIm.Implements.User_UpdateUserInfo(userId.ToString(), name: userName, icon: userLogoUrl);
        //        var jObject = JObject.Parse(json);
        //        if (jObject != null && jObject["code"] != null && (string)jObject["code"] == "200")
        //        {
        //            dynamic parameters = new ExpandoObject();
        //            parameters.name = userName;

        //            return Access.User.UserUpdateColumn(userId.ToString(), parameters) > 0 ? true : false;
        //        }
        //    }
        //    return false;
        //}



        public static string NeteaseIm_GetUserInfos(List<long> userList)
        {
            string result = string.Empty;
            if (userList != null && userList.Count > 0)
            {
                string json = NeteaseIm.Implements.User_GetUserInfos(userList.Select(t => t.ToString()).ToArray());
                var jObject = JObject.Parse(json);
                if (jObject != null && jObject["code"] != null && (string)jObject["code"] == "200")
                {

                }
            }
            return result;
        }

        /// <summary>
        /// 生成云信 accId
        /// </summary>
        /// <returns></returns>
        public static string CreateImeAccId()
        {
            return Guid.NewGuid().ToString().Substring(0, 31);
            //string guid = string.Format("{0}_{1}_{2}", channelid, userType, userId);
            //return guid.Length > 32 ? guid.Substring(0, 31) : guid;
        }
    }
}