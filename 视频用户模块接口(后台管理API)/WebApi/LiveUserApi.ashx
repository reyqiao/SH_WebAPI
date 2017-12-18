<%@ WebHandler Language="C#" Class="LiveUserApi" %>

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

public class LiveUserApi : IHttpHandler
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
                //br.result = 0;
                //br.code = -1;
                //br.message = "未登录。";
                //context.Response.Write(br.ToString());
            }
            //else
            //{
            if (action != null && action != "")
            {
                int roomId = Util.GetIntValue(Util.GetRequest("roomId"), 1);
                long userId = Util.GetLongValue(Util.GetRequest("userid"), 1);
                int roleId = Util.GetIntValue(Util.GetRequest("roleId"), 1);
                string type = Util.GetRequest("type");
                string pagesize = Util.GetRequest("pagesize");
                string pageindex = Util.GetRequest("pageindex");
                switch (action)
                {

                    //用户管理
                    case "GetLiveUser":
                        ret = GetLiveUser(Util.GetIntValue(type, 3), Util.GetIntValue(pagesize, 20), Util.GetIntValue(pageindex, 1));
                        break;
                    //用户解禁、解除、转永久会员
                    case "DeleteLiveUser":
                        string Id = Util.GetRequest("Id");
                        ret = DeleteLiveUser(Id, int.Parse(type), roomId, roleId, userId);
                        break;

                }
            }

            context.Response.Write(ret);
            //}
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
    public string GetLiveUser(int type, int pagesize, int pageindex)
    {
        DataSet ds;
        int totalcount = 0;
        string errorMessage = string.Empty;
        JsonString js = new JsonString();
        if (type == 1)
        {
            if (DataAccess.BeforeAccess.GetNeteaseImChatroomMuteUserLog(type, pagesize, pageindex, out totalcount, out ds, out errorMessage))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    JsonString json = new JsonString();
                    json.Set("UserId", dr["UserId"] + "");
                    json.Set("nickname", dr["nickname"] + "");
                    json.Set("headpicurl", dr["headpicurl"] + "");
                    js.AddArrayItem("list", json);
                }
            }
        }
        if (type == 2)
        {
            if (DataAccess.BeforeAccess.GetNeteaseImUserBlock(type, pagesize, pageindex, out totalcount, out ds, out errorMessage))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    JsonString json = new JsonString();
                    json.Set("Id", dr["Id"] + "");
                    json.Set("UserId", dr["UserId"] + "");
                    json.Set("nickname", dr["nickname"] + "");
                    json.Set("headpicurl", dr["headpicurl"] + "");
                    js.AddArrayItem("list", json);
                }
            }
        }
        if (type == 3)
        {
            if (DataAccess.BeforeAccess.GetLive_User(type, pagesize, pageindex,"" ,out totalcount, out ds, out errorMessage))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    JsonString json = new JsonString();
                    json.Set("Id", dr["Id"] + "");
                    json.Set("UserId", dr["UserId"] + "");
                    var temp = Niugu.Common.Cryptogram.DecryptPassword(dr["mobile"].ToString());
                    json.Set("mobile", temp.Substring(0, 3) + "****" + temp.Substring(7) + "");
                    json.Set("nickname", dr["nickname"] + "");
                    json.Set("headpicurl", dr["headpicurl"] + "");
                    json.Set("addtime", dr["addtime"] + "");
                    json.Set("roleId", dr["roleId"] + "");
                    js.AddArrayItem("list", json);
                }
            }
        }
        js.Set("totalcount", totalcount);
        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("action", "GetLiveUser");
        js.Set("message", "成功");
        return js.ToString();
    }

    //用户解禁、解除、转永久会员
    public string DeleteLiveUser(string ids, int type, int roomId, int roleId, long userId)
    {
        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            string[] id = ids.Split(',');
            for (int i = 0; i < id.Length; i++)
            {
                if (type == 1)
                {
                    if (!DataAccess.BeforeAccess.DeleteNeteaseImChatroomMuteUserLog(int.Parse(id[i]), out errorMessage))
                    {

                        datas = "{\"result\":0,\"code\":0,\"action\":\"DeleteLiveUser\",\"message\":\"" + errorMessage + "\"}";
                    }
                    else
                    {
                        datas = "{\"result\":1,\"code\":0,\"action\":\"DeleteLiveUser\",\"message\":\"设置成功\"}";
                    }
                }
                if (type == 2)
                {
                    if (!DataAccess.BeforeAccess.DeleteNeteaseImUserBlock(int.Parse(id[i]), out errorMessage))
                    {

                        datas = "{\"result\":0,\"code\":0,\"action\":\"DeleteLiveUser\",\"message\":\"" + errorMessage + "\"}";
                    }
                    else
                    {
                        datas = "{\"result\":1,\"code\":0,\"action\":\"DeleteLiveUser\",\"message\":\"设置成功\"}";
                    }
                }
                if (type == 3)
                {
                    if (!DataAccess.BeforeAccess.SetUserRole(roomId, userId, roleId, out errorMessage))
                    {

                        datas = "{\"result\":0,\"code\":0,\"action\":\"DeleteLiveUser\",\"message\":\"" + errorMessage + "\"}";
                    }
                    else
                    {
                        datas = "{\"result\":1,\"code\":0,\"action\":\"DeleteLiveUser\",\"message\":\"设置成功\"}";
                    }
                }

            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"DeleteLiveUser\",\"message\":\"失败\"}";
        }
        return datas;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}