<%@ WebHandler Language="C#" Class="LiveMesMag" %>

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

public class LiveMesMag : IHttpHandler
{

    public Model.BaseInfo bs = new Model.BaseInfo();
    public Model.BaseResult br = new Model.BaseResult();
    public Niu.Live.User.IModel.TokenManager.TokenUserInfo UserInfo = new Niu.Live.User.IModel.TokenManager.TokenUserInfo();
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
            bool result = Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(strUserToken, out UserInfo);

            if (!result || UserInfo == null)
            {
                if (action != null && action != "")
                {
                    switch (action)
                    {
                        case "getblackpagelist":
                            ret = GetBlackPageList();
                            break;
                        case "getallteacherlist":
                            ret = GetAllTeacherList();
                            break;
                        case "getcallorderlist":
                            ret = GetCallOrderList();
                            break;
                        case "addblacklist":
                            ret = AddBlackList();
                            break;
                        case "deleteblacklist":
                            ret = DeleteBlackList();
                            break;

                        case "callorder":
                            ret = AddCallOrder();
                            break;
                        case "callpc":
                            ret = CallPc();
                            break;
                        case "callorderdel":
                            ret = CallOrderDel();
                            break;

                    }
                    context.Response.Write(ret);
                }
                else
                {

                    br.result = 0;
                    br.code = -1;
                    br.message = "未登录。";
                    context.Response.Write(br.ToString());
                }
            }
            else
            {
                if (action != null && action != "")
                {
                    switch (action)
                    {
                        case "addblacklist":
                            ret = AddBlackList();
                            break;
                        case "deleteblacklist":
                            ret = DeleteBlackList();
                            break;
                        case "getblackpagelist":
                            ret = GetBlackPageList();
                            break;
                        case "callorder":
                            ret = AddCallOrder();
                            break;
                        case "callpc":
                            ret = CallPc();
                            break;
                        case "callorderdel":
                            ret = CallOrderDel();
                            break;
                        case "getallteacherlist":
                            ret = GetAllTeacherList();
                            break;
                        case "getcallorderlist":
                            ret = GetCallOrderList();
                            break;

                    }
                    context.Response.Write(ret);
                }
            }
        }
        catch (Exception ex)
        {
            br.result = 0;
            br.code = -1;
            br.message = "服务异常";
            context.Response.Write(br.ToString());
        }

    }

    public string GetCallOrderList()
    {

        int pageIndex = 0;
        if (Niugu.Common.Util.GetRequestQueryString("page") != "")
        {
            pageIndex = Convert.ToInt32(Niugu.Common.Util.GetRequestQueryString("page"));
        }
        if (pageIndex <= 0)
        {
            pageIndex = 1;
        }
        int pageSize = 10;
        if (Niugu.Common.Util.GetRequestQueryString("pagesize") != "")
        {
            pageSize = Convert.ToInt32(Niugu.Common.Util.GetRequestQueryString("pagesize"));
        }
        string searchfilter = "";

        int type = Convert.ToInt32(Util.GetRequest("type"));
        long tid = Convert.ToInt64(Util.GetRequest("tid"));

        if (type == 1)
        {
            searchfilter = searchfilter + " and a.createTime>=CONVERT(varchar(10),GETDATE(),120) ";
        }
        if (tid > 0)
        {
            searchfilter = searchfilter + " and a.teacherId='" + tid + "'";
        }
        StringBuilder sb = new StringBuilder();
        DataSet ds;
        string errorMessage = string.Empty;

        int totalCount = 0;

        if (DataAccess.LiveMesMagAccess.GetCallOrderList(searchfilter, pageIndex, pageSize, out ds, out totalCount, out errorMessage))
        {
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    long callId = Convert.ToInt64(dr["CallID"]);
                    string NickName = Convert.ToString(dr["nickName"]);
                    string CTypeName = Convert.ToString(dr["CTypeName"]);
                    string Direction = Convert.ToString(dr["Direction"]);
                    if (Direction == "1")
                    {
                        Direction = "多";
                    }
                    else
                    {
                        Direction = "空";
                    }
                    string createtime = Convert.ToDateTime(dr["createtime"]).ToString("yyyy-MM-dd HH:mm");
                    string JCPrice = Convert.ToString(dr["JCPrice"]);
                    string ZYPrice = Convert.ToString(dr["ZYPrice"]);
                    string ZSPrice = Convert.ToString(dr["ZSPrice"]);
                    string pctime = string.Empty;
                    string zyPoint = string.Empty;
                    string pcPrice = string.Empty;
                    string isPc = Convert.ToString(dr["isPC"]);
                    if (isPc == "1")
                    {
                        pcPrice = Convert.ToString(dr["PCPrice"]);
                        pctime = Convert.ToDateTime(dr["PCTime"]).ToString("yyyy-MM-dd HH:mm");
                        if (Direction == "多")
                        {
                            zyPoint = (Convert.ToDecimal(pcPrice) - Convert.ToDecimal(JCPrice)).ToString();
                        }
                        else
                        {
                            zyPoint = (Convert.ToDecimal(JCPrice) - Convert.ToDecimal(pcPrice)).ToString();
                        }
                    }
                    string color = "0";
                    if (zyPoint != "")
                    {
                        if (Convert.ToSingle(zyPoint) > 0)
                        {
                            color = "1";
                        }
                        else if (Convert.ToSingle(zyPoint) == 0)
                        {
                            color = "0";
                        }
                        else
                        {
                            color = "2";
                        }
                    }
                    sb.Append("{\"callId\":\"" + callId + "\",\"nickName\":\"" + NickName + "\",\"ctypeName\":\"" + CTypeName + "\",\"direction\":\"" + Direction + "\",\"createtime\":\"" + createtime + "\",\"jcPrice\":\"" + JCPrice + "\",\"zyPrice\":\"" + ZYPrice + "\",\"zsPrice\":\"" + ZSPrice + "\",\"pctime\":\"" + pctime + "\",\"zyPoint\":\"" + zyPoint + "\",\"pcPrice\":\"" + pcPrice + "\",\"color\":\"" + color + "\"},");

                }
            }
        }

        string data = sb.ToString();
        if (data.Length > 0)
        {
            data = data.TrimEnd(',');
        }
        return "{\"action\":\"getcallorderlist\",\"code\":0,\"message\":\"\", \"page\":\"" + pageIndex + "\", \"totalCount\":\"" + totalCount + "\",\"size\":\"" + pageSize + "\",\"result\":1,\"data\":[" + data + "]}";



    }



    public string GetAllTeacherList()
    {
        StringBuilder sb = new StringBuilder();
        DataSet ds;
        string errorMessage = string.Empty;
        if (DataAccess.LiveMesMagAccess.GetALlTeacherList(out ds, out errorMessage))
        {
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    long uId = Convert.ToInt64(dr["UserID"]);
                    string nickName = Convert.ToString(dr["NickName"]);
                    sb.Append("{\"userid\":\"" + uId + "\",\"nickName\":\"" + nickName + "\"},");
                }
            }
        }
        string data = sb.ToString();
        if (data.Length > 0)
        {
            data = data.TrimEnd(',');
        }
        return "{\"action\":\"getallteacherlist\",\"code\":0,\"message\":\"\",\"result\":1,\"data\":[" + data + "]}";

    }


    public string CallOrderDel()
    {
        JsonString js = new JsonString();
        long cid = Convert.ToInt64(Util.GetRequest("cid"));

        int flag = 0;
        string errorMessage = string.Empty;
        if (UserInfo != null && UserInfo.userId > 0)
        {
            if (DataAccess.LiveMesMagAccess.DelCallOrder(UserInfo.userId, cid, out flag, out errorMessage))
            {
                if (flag == -2)
                {
                    js.Set("result", "0");
                    js.Set("code", "-1");
                    js.Set("action", "callorderdel");
                    js.Set("message", "无此记录");
                    return js.ToString();
                }
                else if (flag == -1)
                {
                    js.Set("result", "0");
                    js.Set("code", "-1");
                    js.Set("action", "callorderdel");
                    js.Set("message", "无权限");
                    return js.ToString();
                }
                else if (flag == 1)
                {
                    js.Set("result", "1");
                    js.Set("code", "0");
                    js.Set("action", "callorderdel");
                    js.Set("message", "操作成功");
                    return js.ToString();
                }
            }
        }
        js.Set("result", "0");
        js.Set("code", "-1");
        js.Set("action", "callorderdel");
        js.Set("message", "操作异常");
        return js.ToString();

    }

    public string CallPc()
    {
        JsonString js = new JsonString();
        long cid = Convert.ToInt64(Util.GetRequest("cid"));
        string pcprice = Util.GetRequest("pcprice");
        int flag = 0;
        string errorMessage = string.Empty;
        if (UserInfo != null && UserInfo.userId > 0)
        {
            if (DataAccess.LiveMesMagAccess.PcOrder(UserInfo.userId, cid, pcprice, out flag, out errorMessage))
            {
                if (flag == -2)
                {
                    js.Set("result", "0");
                    js.Set("code", "-1");
                    js.Set("action", "callpc");
                    js.Set("message", "无此记录");
                    return js.ToString();
                }
                else if (flag == -1)
                {
                    js.Set("result", "0");
                    js.Set("code", "-1");
                    js.Set("action", "callpc");
                    js.Set("message", "无权限");
                    return js.ToString();
                }
                else if (flag == 1)
                {
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        sendMsg(UserInfo.nickName, "平仓");
                    });
                    js.Set("result", "1");
                    js.Set("code", "0");
                    js.Set("action", "callpc");
                    js.Set("message", "操作成功");
                    return js.ToString();
                }
            }
        }
        js.Set("result", "0");
        js.Set("code", "-1");
        js.Set("action", "callpc");
        js.Set("message", "操作异常");
        return js.ToString();


    }

    public string AddCallOrder()
    {
        JsonString js = new JsonString();
        string errorMessage = string.Empty;
        int ctype = Convert.ToInt32(Util.GetRequest("ctype"));
        string ctypname = Util.GetRequest("ctypename");
        int dir = Convert.ToInt32(Util.GetRequest("dir"));
        string jcprice = Util.GetRequest("jcprice");
        string zypirce = Util.GetRequest("zyprice");
        string zsprice = Util.GetRequest("zsprice");

        if (UserInfo != null && UserInfo.userId > 0)
        {
            if (DataAccess.LiveMesMagAccess.AddCallOrder(UserInfo.userId, ctype, ctypname, dir, jcprice, zypirce, zsprice, out errorMessage))
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    sendMsg(UserInfo.nickName, "建仓");
                });
                js.Set("result", "1");
                js.Set("code", "0");
                js.Set("action", "addcallorder");
                js.Set("message", "喊单成功");
                return js.ToString();
            }
        }
        js.Set("result", "0");
        js.Set("code", "-1");
        js.Set("action", "addcallorder");
        js.Set("message", "操作异常");
        return js.ToString();



    }

    public string GetBlackPageList()
    {
        DataSet ds;
        int totalCount = 0;
        string message = string.Empty;
        int pageIndex = Convert.ToInt32(Util.GetRequest("page"));
        if (pageIndex < 1)
        {
            pageIndex = 1;
        }
        int pageSize = Convert.ToInt32(Util.GetRequest("size"));
        if (pageSize < 1)
        {
            pageSize = 10;
        }
        StringBuilder sb = new StringBuilder();
        if (DataAccess.LiveMesMagAccess.GetBlackListByPage(pageIndex, pageSize, out totalCount, out ds, out message))
        {
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    long uId = Convert.ToInt64(dr["UserID"]);
                    string nickName = Convert.ToString(dr["NickName"]);
                    sb.Append("{\"userid\":\"" + uId + "\",\"nickName\":\"" + nickName + "\"},");
                }
            }
        }
        string data = sb.ToString();
        if (data.Length > 0)
        {
            data = data.TrimEnd(',');
        }
        return "{\"action\":\"getblackpagelist\",\"code\":0,\"message\":\"\",\"result\":1,\"totalcount\":\"" + totalCount + "\",\"data\":[" + data + "]}";
    }

    public string DeleteBlackList()
    {
        JsonString js = new JsonString();
        long delUserId = Convert.ToInt64(Util.GetRequest("uid"));
        int flag = 0;
        string errorMessage = string.Empty;
        //  if (UserInfo != null && UserInfo.userId > 0)
        // {
        if (DataAccess.LiveMesMagAccess.DeleteBlackList(delUserId, out flag, out errorMessage))
        {
            if (flag == 1)
            {
                js.Set("result", "1");
                js.Set("code", "0");
                js.Set("action", "deleteblacklist");
                js.Set("message", "删除成功");
                return js.ToString();
            }
            else if (flag == -1)
            {
                js.Set("result", "0");
                js.Set("code", "-1");
                js.Set("action", "deleteblacklist");
                js.Set("message", "非黑名单用户");
                return js.ToString();
            }
        }
        //  }
        js.Set("result", "0");
        js.Set("code", "-1");
        js.Set("action", "deleteblacklist");
        js.Set("message", "操作异常");
        return js.ToString();
    }
    public string AddBlackList()
    {
        JsonString js = new JsonString();
        long addUserId = Convert.ToInt64(Util.GetRequest("uid"));
        int flag = 0;
        string errorMessage = string.Empty;
        string userIp = Util.GetIP();
        if (UserInfo != null && UserInfo.userId > 0)
        {
            if (DataAccess.LiveMesMagAccess.AddBlackList(addUserId, UserInfo.userId, userIp, out flag, out errorMessage))
            {
                if (flag == 1)
                {
                    js.Set("result", "1");
                    js.Set("code", "0");
                    js.Set("action", "addblacklist");
                    js.Set("message", "添加成功");
                    return js.ToString();
                }
                else if (flag == -1)
                {
                    js.Set("result", "0");
                    js.Set("code", "-1");
                    js.Set("action", "addblacklist");
                    js.Set("message", "不能重复添加");
                    return js.ToString();
                }
            }
        }
        js.Set("result", "0");
        js.Set("code", "-1");
        js.Set("action", "addblacklist");
        js.Set("message", "操作异常");
        return js.ToString();

    }
    void sendMsg(string username, string t)
    {
        string url = string.Format("https://live.fxtrade888.com/chatroom/chartroom/StopLiveMsg?Id=77777&username={0}&t={1}", username, t);
        WebClient wc = new WebClient();

        wc.DownloadString(url);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}