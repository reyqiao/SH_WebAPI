<%@ WebHandler Language="C#" Class="LiveUserLevelApi" %>

using System;
using System.Web;
using Niugu.Common;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using NiuGu.Utility;

public class LiveUserLevelApi : IHttpHandler
{

    public Model.BaseInfo bs = new Model.BaseInfo();
    public Model.BaseResult br = new Model.BaseResult();
    public Niugu.Common.UserInfo UserInfo = new Niugu.Common.UserInfo();
    public void ProcessRequest(HttpContext context)
    {

        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.ContentType = "application/json";
        context.Response.Charset = "UTF-8";

        try
        {
            string action = Util.GetRequest("action");
            string ret = "";
            string strUserToken = Util.GetRequest("userToken");
            bool result = Niugu.Common.TokenManager.ValidateUserToken(ref strUserToken, out UserInfo);
            if (!result || UserInfo == null)
            {
                br.result = 0;
                br.code = -1;
                br.message = "未登录。";
                context.Response.Write(br.ToString());
            }
            else
            {
                if (action != null && action != "")
                {

                    string phone = Util.GetRequest("phone");
                    int pagesize = Util.GetRequest("pagesize").TryParseByInt();
                    if (pagesize == 0)
                        pagesize = 10;
                    int pageindex = Util.GetRequest("pageindex").TryParseByInt();
                    if (pageindex == 0)
                        pageindex = 1;



                    switch (action.ToLower())
                    {

                        //用户管理
                        case "getlist":
                            ret = GetLiveUser(phone, pagesize, pageindex);
                            break;

                        //用户管理
                        case "getrolelist":
                            ret = GetRoleList();
                            break;

                        //禁用启用 0启用 -1禁用
                        case "setdisable":

                            Int64 disableuserid = Util.GetRequest("userid").TryParseByLong(); //用户
                            int status = Util.GetRequest("status").TryParseByInt(); //0启用 -1禁用

                            ret = SetDisable(disableuserid, status);

                            break;

                        //删除用户
                        case "delete":

                            Int64 userid = Util.GetRequest("userid").TryParseByLong(); //删除用户

                            ret = Delete(userid);

                            break;

                        //重置密码
                        case "resetpassword":

                            Int64 resetpassworduserid = Util.GetRequest("userid").TryParseByLong(); //重置密码

                            ret = ResetPassword(resetpassworduserid);

                            break;

                        case "resetrole": //重置级别

                            Int64 ResetRoleUserid = Util.GetRequest("userid").TryParseByLong();
                            string roles = Util.GetRequest("roles");

                            ret = ResetRole(ResetRoleUserid, roles);

                            break;

                    }
                }

                context.Response.Write(ret);
            }
        }
        catch (Exception ex)
        {
            br.result = 0;
            br.code = -1;
            br.message = "服务不太给力，请稍后再试。";
            context.Response.Write(br.ToString());
        }
    }


    //用户管理
    public string GetLiveUser(string phone, int pageSize, int pageIndex)
    {
        int recordCount = 0;

        System.Text.StringBuilder sbwhere = new System.Text.StringBuilder("1=1 ");


        if (!string.IsNullOrEmpty(phone))
        {
            if (phone.Length == 11)
            {
                sbwhere.AppendFormat(" and [Mobile]='{0}'  ", Niu.Cabinet.Cryptography.NiuCryptoService.EncryptPassword(phone.ToPourIntoString()));

            }
            else
            {
                sbwhere.AppendFormat(" and [NickName] like '%{0}%' ", phone);
            }
        }

        string filedsql = @"[UserId]
                                  ,[ChannelId]
                                  ,[NickName]
                                  ,[Mobile]
                                  ,[Type]
                                  ,[Gender]
                                  ,[Status]
                                  ,[AddTime]
                                  ,[ip1]
                                  ,[ip2]
                                  ,[useragent]";

        string errorMessage = string.Empty;
        JsonString js = new JsonString();

        List<object> datalist = new List<object>();
        DataSet ds = DataAccess.DataList.GetList("[User]", filedsql, sbwhere.ToString(), "[UserId] desc", pageSize, pageIndex, out recordCount);
        if (ds != null && ds.Tables.Count > 0)
        {
            foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
            {
                JsonString json = new JsonString();
                json.Set("userid", dr["UserId"].ToString());
                json.Set("nickname", dr["NickName"].ToString());
                json.Set("mobile", Niu.Cabinet.Cryptography.NiuCryptoService.DecryptPassword(dr["Mobile"].ToString()));
                json.Set("status", dr["Status"].ToString() == "0" ? "正常" : "已禁用");
                json.Set("ip", dr["ip1"].ToString() + (!string.IsNullOrEmpty(dr["ip2"].ToString()) ? "," + dr["ip2"].ToString() : ""));
                json.Set("regtime", dr["AddTime"].TryParseByDateTime().ToString("yyyy-MM-dd HH:mm"));
                json.Set("role", DataAccess.LiveUserAccess.GetRole(dr["UserId"].TryParseByLong()));
                js.AddArrayItem("list", json);
            }
        }


        js.Set("totalcount", recordCount);
        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("action", "getlist");
        js.Set("message", "成功");
        return js.ToString();
    }

    //[RoleID],[RoleName],[IsManage]
    public string GetRoleList()
    {
        JsonString js = new JsonString();
        DataTable dt = DataAccess.LiveUserAccess.GetRoleList();
        if (dt != null && dt.Rows.Count > 0)
        {
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                JsonString json = new JsonString();
                json.Set("roleid", dr["RoleID"].ToString());
                json.Set("rolename", dr["RoleName"].ToString());
                json.Set("ismanage", dr["IsManage"].TryParseByInt());
                js.AddArrayItem("list", json);
            }
        }
        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("action", "getrolelist");
        js.Set("message", "成功");
        return js.ToString();
    }


    /// <summary>
    /// 禁用启用
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="status">-1 禁用 0启用</param>
    /// <returns></returns>
    public string SetDisable(Int64 userid, int status)
    {
        if (status != 0)
            status = -1;

        int result = DataAccess.LiveUserAccess.SetDisable(userid, status);
        var data = new { result = result, code = 0, action = "setdisable", message = (status == -1 ? "禁用成功" : "启用成功") };
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }


    public string Delete(Int64 userid)
    {
        int result = DataAccess.LiveUserAccess.Delete(userid);
        var data = new { result = result, code = 0, action = "delete", message = (result > -1 ? "删除成功" : "删除失败") };
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public string ResetPassword(Int64 userid)
    {
        int result = DataAccess.LiveUserAccess.ResetPassword(userid);
        var data = new { result = result, code = 0, action = "resetpassword", message = (result > -1 ? "重置密码成功" : "重置密码失败") };
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public string ResetRole(Int64 userid, string roles)
    {
        int result = DataAccess.LiveUserAccess.ResetRole(userid, roles);
        var data = new { result = result, code = 0, action = "resetrole", message = (result > 0 ? "设置级别成功" : "设置级别失败") };
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}