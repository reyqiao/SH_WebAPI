<%@ WebHandler Language="C#" Class="LiveBaseDataApi" %>

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

public class LiveBaseDataApi : IHttpHandler
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

            //登录
            if (action == "login")
            {
                ret = Login(context);
            }
            else
            {
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
                        int roomId = Util.GetIntValue(Util.GetRequest("roomId"), 1);//当前类型
                        switch (action)
                        {
                            //获取直播间基础数据
                            case "GetLiveBaseData":
                                ret = GetLiveBaseData();
                                break;
                            //游客模式
                            case "UpdateVisitor":
                                string IsVisitor = Util.GetRequest("IsVisitor");
                                ret = UpdateVisitor(int.Parse(IsVisitor));
                                break;
                            //跑马灯添加
                            case "AddLiveMarquee":
                                string MarqueeText = Util.GetRequest("MarqueeText");
                                string MarqueeLink = Util.GetRequest("MarqueeLink");
                                ret = AddLiveMarquee(MarqueeText, MarqueeLink);
                                break;
                            //跑马灯删除   
                            case "UpdateLiveMarquee":
                                string Id = Util.GetRequest("Id");
                                ret = UpdateLiveMarquee(int.Parse(Id));
                                break;
                            //直播间公告
                            case "UpdateLiveBaseNotice":
                                string Notice = Util.GetRequest("Notice");
                                ret = UpdateLiveBaseNotice(Notice);
                                break;
                            //顶部banner管理
                            case "UpdateLiveBaseTopBanner":
                                //string TopBanner = Util.GetRequest("TopBanner");
                                ret = UpdateLiveBaseTopBanner(context);
                                break;
                            //顶部banner管理
                            case "AddLiveBaseTopBanner":
                                //string TopBanner = Util.GetRequest("TopBanner");
                                ret = UpdateLiveBaseTopBanner(context);
                                break;
                        }
                    }

                }
            }
            context.Response.Write(ret);
        }
        catch (Exception ex)
        {
            br.result = 0;
            br.code = -1;
            br.message = "服务不太给力，请稍后再试。";
            context.Response.Write(br.ToString());
        }
    }

    //游客模式
    public string Login(HttpContext context)
    {
        string errorMessage = string.Empty;
        string datas = "";
        DataSet ds;
        string userName = context.Request.Form["userName"];
        string passWord = context.Request.Form["passWord"];
        try
        {
            if (DataAccess.BeforeAccess.Live_login(userName, out ds, out errorMessage))
            {
                string pwd = NiuGu.Security.HashManager.Hash("MD5", NiuGu.Security.HashManager.Hash("MD5", passWord));
                if (ds.Tables[0].Rows[0]["pwd"].ToString() == pwd)
                {
                    string userToken = string.Empty;
                    UserInfo userInfo = new Niugu.Common.UserInfo()
                    {
                        ID = int.Parse(ds.Tables[0].Rows[0]["AccountId"].ToString()),
                        UserName = ds.Tables[0].Rows[0]["NickName"].ToString(),
                        Type = UserType.NiuGuWang,
                        State = 0,
                        Rcode = "0000",
                    };
                    TokenManager.GenerateUserToken(userInfo, out userToken);

                    datas = "{\"result\":1,\"code\":0,\"action\":\"Login\",\"LoginUserID\":\"" + ds.Tables[0].Rows[0]["AccountId"].ToString() + "\",\"NickName\":\"" + ds.Tables[0].Rows[0]["NickName"].ToString() + "\",\"userToken\":\"" + userToken + "\",\"message\":\"" + errorMessage + "\"}";
                }
                else
                {
                    datas = "{\"result\":0,\"code\":-1,\"action\":\"login\",\"message\":\"密码错误\"}";
                }
            }
            else
            {
                datas = "{\"result\":0,\"code\":-1,\"action\":\"login\",\"message\":\"用户名不存在\"}";
            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"login\",\"message\":\"登录失败\"}";
        }
        return datas;
    }
    #region
    //获取整个界面数据
    public string GetLiveBaseData(int roomId = 1)
    {
        DataSet ds;
        string errorMessage = string.Empty;
        JsonString js = new JsonString();
        if (DataAccess.BeforeAccess.GetLive_BaseData(out ds, out errorMessage))
        {
            //游客模式，直播间公告，顶部banner
            js.Set("IsVisitor", ds.Tables[0].Rows[0]["IsVisitor"] + "");
            js.Set("Notice", ds.Tables[0].Rows[0]["Notice"] + "");
            js.Set("TopBanner", "http://live.fxtrade888.com/Api/" + ds.Tables[0].Rows[0]["TopBanner"] + "");
            js.Set("BannerLink", ds.Tables[0].Rows[0]["BannerLink"] + "");
        }
        if (ds != null && ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
        {
            foreach (DataRow dr in ds.Tables[1].Rows)
            {
                JsonString json = new JsonString();
                json.Set("Id", dr["id"] + "");
                json.Set("MarqueeText", dr["MarqueeText"] + "");
                json.Set("MarqueeLink", dr["MarqueeLink"] + "");

                js.AddArrayItem("Marquee", json);
            }
        }

        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("action", "GetLiveBaseData");
        js.Set("message", "成功");

        return js.ToString();
    }
    #endregion

    //游客模式
    public string UpdateVisitor(int IsVisitor)
    {

        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            if (!DataAccess.BeforeAccess.UpdateLive_LiveBaseIsVisitor(IsVisitor, out errorMessage))
            {

                datas = "{\"result\":0,\"code\":0,\"action\":\"UpdateVisitor\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"UpdateVisitor\",\"message\":\"更新成功\"}";
            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"UpdateVisitor\",\"message\":\"更新失败\"}";
        }
        return datas;
    }
    //跑马灯管理
    public string AddLiveMarquee(string MarqueeText, string MarqueeLink)
    {
        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            if (!DataAccess.BeforeAccess.AddLive_Marquee(MarqueeText, MarqueeLink, out errorMessage))
            {

                datas = "{\"result\":0,\"code\":0,\"action\":\"AddLiveMarquee\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"AddLiveMarquee\",\"message\":\"添加成功\"}";
            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"AddLiveMarquee\",\"message\":\"添加失败\"}";
        }
        return datas;
    }

    //跑马灯管理
    public string UpdateLiveMarquee(int id)
    {
        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            if (!DataAccess.BeforeAccess.UpdateLive_Marquee(id, out errorMessage))
            {

                datas = "{\"result\":0,\"code\":0,\"action\":\"UpdateLiveMarquee\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"UpdateLiveMarquee\",\"message\":\"更新成功\"}";
            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"UpdateLiveMarquee\",\"message\":\"更新失败\"}";
        }
        return datas;
    }

    //直播间公告
    public string UpdateLiveBaseNotice(string Notice)
    {
        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            if (!DataAccess.BeforeAccess.UpdateLive_LiveBaseNotice(Notice, out errorMessage))
            {

                datas = "{\"result\":0,\"code\":0,\"action\":\"UpdateLiveMarquee\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"UpdateLiveMarquee\",\"message\":\"更新成功\"}";
            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"UpdateLiveMarquee\",\"message\":\"更新失败\"}";
        }
        return datas;
    }
    //顶部banner管理
    public string UpdateLiveBaseTopBanner(HttpContext context)
    {
        string TopBanner = context.Request.Form["liveBanner"].ToString();
        string BannerLink = context.Request.Form["BannerLink"].ToString();
        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            if (TopBanner.IndexOf("data:image".ToLower()) >= 0)
            {
                string image = TopBanner.Split(';')[0].Split('/')[1];

                byte[] byt = Convert.FromBase64String(TopBanner.Split(',')[1]);

                Guid filename = Guid.NewGuid();
                TopBanner = "Images/" + filename.ToString() + "." + image;
                using (System.IO.Stream stream = new System.IO.MemoryStream(byt))
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(HttpContext.Current.Server.MapPath(TopBanner), System.IO.FileMode.CreateNew))
                    {
                        //将得到的文件流复制到写入流中
                        stream.CopyTo(fs);
                        fs.Flush();
                    }
                }
            }
            if (!DataAccess.BeforeAccess.UpdateLive_LiveBaseTopBanner(TopBanner, BannerLink, out errorMessage))
            {

                datas = "{\"result\":0,\"code\":0,\"action\":\"UpdateLiveBaseTopBanner\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"UpdateLiveBaseTopBanner\",\"message\":\"更新成功\"}";
            }
        }
        catch (Exception ex)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"UpdateLiveBaseTopBanner\",\"message\":\"更新失败\"}";
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