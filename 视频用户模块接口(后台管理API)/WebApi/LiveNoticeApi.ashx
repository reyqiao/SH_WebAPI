<%@ WebHandler Language="C#" Class="LiveNoticeApi" %>

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

public class LiveNoticeApi : IHttpHandler {

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
                    string Id = Util.GetRequest("Id");
                    string Day = Util.GetRequest("Day");
                    string BeginTime = Util.GetRequest("BeginTime");
                    string EndTime = Util.GetRequest("EndTime");
                    string LiveTheme = Util.GetRequest("LiveTheme");
                    string Teacher = Util.GetRequest("Teacher");
                    string LiveType = Util.GetRequest("LiveType");
                    string LiveIntroduce = Util.GetRequest("LiveIntroduce");
                    switch (action)
                    {
                        //获取直播预告列表
                        case "GetLiveNotice":
                            ret = GetLiveNotice();
                            break;
                        //获取直播预告列表
                        case "GetLiveNoticePic":
                            ret = GetLiveNoticePic();
                            break;  
                            
                        //添加直播预告
                        case "AddLiveNotice":
                            ret = AddLiveNotice(Day, BeginTime, EndTime, LiveTheme, Teacher, LiveType, LiveIntroduce);
                            break;
                        //添加直播预告
                        case "AddLiveNoticePic":
                            ret = AddLiveNoticePic(context);
                            break;
                        //删除直播预告
                        case "DeleteLiveNotice":
                            ret = DeleteLiveNotice(int.Parse(Id));
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


    //获取直播预告列表
    public string GetLiveNotice()
    {
        DataSet ds;
        string errorMessage = string.Empty;
        JsonString js = new JsonString();
        if (DataAccess.BeforeAccess.GetLive_Notice(out ds, out errorMessage))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                JsonString json = new JsonString();
                json.Set("Id", dr["Id"] + "");
                json.Set("Day", dr["Day"] + "");
                json.Set("BeginTime", dr["BeginTime"] + "");
                json.Set("EndTime", dr["EndTime"] + "");
                json.Set("LiveTheme", dr["LiveTheme"] + "");
                json.Set("Teacher", dr["Teacher"] + "");
                json.Set("LiveType", dr["LiveType"] + "");
                json.Set("LiveIntroduce", dr["LiveIntroduce"] + "");
                js.AddArrayItem("list", json);
            }
        }
        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("action", "GetLiveNotice");
        js.Set("message", "成功");
        return js.ToString();
    }

    //获取直播预告列表
    public string GetLiveNoticePic()
    {
        DataSet ds;
        string errorMessage = string.Empty;
        JsonString js = new JsonString();
        string NoticePic = "";
        if (DataAccess.BeforeAccess.GetLive_NoticePic(out ds, out errorMessage))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                NoticePic = "http://live.inquant.cn/Api/" + dr["NoticePic"].ToString();
            }
        }
        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("NoticePic", NoticePic);
        js.Set("action", "GetLiveNoticePic");
        js.Set("message", "成功");
        return js.ToString();
    }

    //添加直播预告
    public string AddLiveNotice( string Day, string BeginTime, string EndTime, string LiveTheme, string Teacher, string LiveType, string LiveIntroduce)
    {
        string errorMessage = string.Empty;
        string datas = "";
        if (!DataAccess.BeforeAccess.AddLive_Notice(Day,  BeginTime,  EndTime,  LiveTheme,   Teacher,  LiveType,  LiveIntroduce, out errorMessage))
        {

            datas = "{\"result\":0,\"code\":0,\"action\":\"AddLiveNotice\",\"message\":\"" + errorMessage + "\"}";
        }
        else
        {
            datas = "{\"result\":1,\"code\":0,\"action\":\"AddLiveNotice\",\"message\":\"设置成功\"}";
        }
        return datas;
    }
    
    //添加直播预告
    public string AddLiveNoticePic(HttpContext context)
    {
        string errorMessage = string.Empty;
        string datas = "";
        string NoticePic = context.Request.Form["NoticePic"].ToString();

        try
        {
            if (NoticePic.IndexOf("data:image".ToLower()) >= 0)
            {
                string image = NoticePic.Split(';')[0].Split('/')[1];

                byte[] byt = Convert.FromBase64String(NoticePic.Split(',')[1]);

                Guid filename = Guid.NewGuid();
                NoticePic = "Images/" + filename.ToString() + "." + image;
                using (System.IO.Stream stream = new System.IO.MemoryStream(byt))
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(HttpContext.Current.Server.MapPath(NoticePic), System.IO.FileMode.CreateNew))
                    {
                        //将得到的文件流复制到写入流中
                        stream.CopyTo(fs);
                        fs.Flush();
                    }
                }
            }
            else
            {
                NoticePic = NoticePic.Split('/')[4].ToString() + "/" + NoticePic.Split('/')[5].ToString();
            }
            if (!DataAccess.BeforeAccess.AddLive_NoticePic(NoticePic, out errorMessage))
            {

                datas = "{\"result\":0,\"code\":0,\"action\":\"AddLiveNoticePic\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"AddLiveNoticePic\",\"message\":\"更新成功\"}";
            }
        }
        catch (Exception ex)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"AddLiveNoticePic\",\"message\":\"更新失败\"}";
        }
        return datas;
        
        
        
    }
    //删除直播预告
    public string DeleteLiveNotice(int id)
    {
        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            if (!DataAccess.BeforeAccess.DeleteLive_Notice(id, out errorMessage))
            {
                datas = "{\"result\":0,\"code\":0,\"action\":\"AddLiveTeacher\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"AddLiveTeacher\",\"message\":\"设置成功\"}";
            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"AddLiveTeacher\",\"message\":\"失败\"}";
        }
        return datas;
    }

 
    public bool IsReusable {
        get {
            return false;
        }
    }

}