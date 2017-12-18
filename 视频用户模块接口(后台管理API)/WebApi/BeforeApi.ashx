<%@ WebHandler Language="C#" Class="BeforeApi" %>

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


public class BeforeApi : IHttpHandler
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
            if (action != null && action != "")
            {
                int roomId = Util.GetIntValue(Util.GetRequest("roomId"), 1);
                string type = Util.GetRequest("type");
                string pagesize = Util.GetRequest("pagesize");
                string pageindex = Util.GetRequest("pageindex");
                string day = Util.GetRequest("day");
                switch (action)
                {
                    //获取banner、跑马灯、直播间公告、游客模式
                    case "GetLiveMainData":
                        ret = GetLiveMainData();
                        break;
                    //直播老师列表
                    case "GetLiveTeacherList":
                        ret = GetLiveTeacherList();
                        break;
                    //精品课程栏目
                    case "GetLiveVideoType":
                        ret = GetLiveVideoType();
                        break;
                    //直播介绍
                    case "GetLiveNoticeInfo":
                        ret = GetLiveNoticeInfo(int.Parse(Util.GetRequest("Id")));
                        break;
                    //精品视频列表
                    case "GetLiveVideo":
                        ret = GetLiveVideo(int.Parse(type), int.Parse(pagesize), int.Parse(pageindex));
                        break;
                    //获取直播日
                    case "GetLiveNoticeDay":
                        ret = GetLiveNoticeDay();
                        break;
                    //直播预告列表
                    case "GetLiveNotice":
                        ret = GetLiveNotice(day);
                        break;
                    case "GetLiveNoticePic":
                        ret = GetLiveNoticePic();
                        break;
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

    //获取整个界面数据
    public string GetLiveMainData()
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
        js.Set("action", "GetLiveMainData");
        js.Set("message", "成功");

        return js.ToString();
    }

    //获取老师介绍列表
    public string GetLiveTeacherList()
    {
        DataSet ds;
        string errorMessage = string.Empty;
        JsonString js = new JsonString();
        if (DataAccess.BeforeAccess.GetLive_TeacherList(out ds, out errorMessage))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                JsonString json = new JsonString();
                json.Set("Id", dr["Id"] + "");
                json.Set("teachername", dr["teachername"] + "");
                json.Set("TeacherFace", "http://live.fxtrade888.com/Api/" + dr["TeacherFace"] + "");
                json.Set("TeacherTag", dr["TeacherTag"] + "");
                json.Set("WinRate", dr["WinRate"] + "");
                json.Set("Income", dr["Income"] + "");
                json.Set("teacherinfo", dr["teacherinfo"] + "");
                json.Set("Introduce", dr["Introduce"] + "");
                js.AddArrayItem("list", json);
            }
        }
        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("action", "GetLiveTeacherList");
        js.Set("message", "成功");
        return js.ToString();
    }

    //获取视频栏目
    public string GetLiveVideoType()
    {
        DataSet ds;
        string errorMessage = string.Empty;
        JsonString js = new JsonString();
        if (DataAccess.BeforeAccess.GetLive_VideoList(0, out ds, out errorMessage))
        {
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    JsonString json = new JsonString();
                    json.Set("Id", dr["Id"] + "");
                    json.Set("VideoName", dr["VideoName"] + "");
                    DataSet dsvideo;
                    int totalcount = 0;
                    DataAccess.BeforeAccess.GetLive_Video(int.Parse(dr["Id"].ToString()), 1, 1, out totalcount, out dsvideo, out errorMessage);
                    json.Set("Totalcount", totalcount + "");
                    js.AddArrayItem("list", json);
                }
            }
        }

        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("action", "GetLiveVideoType");
        js.Set("message", "成功");

        return js.ToString();
    }

    //直播详情
    public string GetLiveNoticeInfo(int id)
    {
        DataSet ds;
        string errorMessage = string.Empty;
        string LiveIntroduce = "";
        JsonString js = new JsonString();

        if (DataAccess.BeforeAccess.GetLive_NoticeById(id, out ds, out errorMessage))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                LiveIntroduce = dr["LiveIntroduce"].ToString();
            }
        }
        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("action", "GetLiveNoticeInfo");
        js.Set("LiveIntroduce", LiveIntroduce);
        js.Set("message", "成功");
        return js.ToString();
    }

    //精品课程
    public string GetLiveVideo(int type, int pagesize, int pageindex)
    {
        DataSet ds;
        int totalcount = 0;
        string errorMessage = string.Empty;
        JsonString js = new JsonString();

        if (DataAccess.BeforeAccess.GetLive_Video(type, pagesize, pageindex, out totalcount, out ds, out errorMessage))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                JsonString json = new JsonString();
                json.Set("Id", dr["Id"] + "");
                json.Set("VideoId", dr["VideoId"] + "");
                json.Set("VideoTheme", dr["VideoTheme"] + "");
                json.Set("VideoPath", dr["VideoPath"] + "");
                json.Set("Cover", "http://live.fxtrade888.com/Api/" + dr["Cover"] + "");
                json.Set("Introduce", dr["Introduce"] + "");
                js.AddArrayItem("list", json);
            }
        }
        js.Set("totalcount", totalcount);
        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("action", "GetLiveVideo");
        js.Set("message", "成功");
        return js.ToString();
    }
    //获取直播日
    public string GetLiveNoticeDay()
    {
        DataSet ds;

        string errorMessage = string.Empty;
        JsonString js = new JsonString();

        if (DataAccess.BeforeAccess.GetLive_NoticeDay(out ds, out errorMessage))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                JsonString json = new JsonString();
                json.Set("Day", dr["Day"] + "");
                json.Set("shortday", dr["shortday"] + "");
                json.Set("weekdays", dr["weekdays"] + "");
                DataSet Noticeds;
                if (DataAccess.BeforeAccess.GetLive_NoticeByDay(dr["Day"].ToString(), out Noticeds, out errorMessage))
                {
                    foreach (DataRow drnotice in Noticeds.Tables[0].Rows)
                    {
                        JsonString jsonnotice = new JsonString();
                        jsonnotice.Set("Id", drnotice["Id"] + "");
                        jsonnotice.Set("Day", drnotice["Day"] + "");
                        jsonnotice.Set("BeginTime", drnotice["BeginTime"] + "");
                        jsonnotice.Set("EndTime", drnotice["EndTime"] + "");
                        jsonnotice.Set("LiveTheme", drnotice["LiveTheme"] + "");
                        jsonnotice.Set("Teacher", drnotice["Teacher"] + "");
                        jsonnotice.Set("LiveType", drnotice["LiveType"] + "");
                        jsonnotice.Set("LiveIntroduce", drnotice["LiveIntroduce"] + "");
                        json.AddArrayItem("noticelist", jsonnotice);
                    }
                }
                js.AddArrayItem("list", json);
            }
        }
        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("action", "GetLiveNoticeDay");
        js.Set("message", "成功");
        return js.ToString();
    }

    //直播预告列表
    public string GetLiveNotice(string day)
    {
        DataSet ds;
        string errorMessage = string.Empty;
        JsonString js = new JsonString();
        if (DataAccess.BeforeAccess.GetLive_NoticeByDay(day, out ds, out errorMessage))
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
                NoticePic = "http://live.fxtrade888.com/Api/" + dr["NoticePic"].ToString();
            }
        }
        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("NoticePic", NoticePic);
        js.Set("action", "GetLiveNoticePic");
        js.Set("message", "成功");
        return js.ToString();
    }
    public string GetLiveSet()
    {
        return null;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}