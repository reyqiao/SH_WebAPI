<%@ WebHandler Language="C#" Class="LiveVideoApi" %>

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

public class LiveVideoApi : IHttpHandler
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
                    string VideoId = Util.GetRequest("VideoId");
                    string Id = Util.GetRequest("id");
                    string VideoName = Util.GetRequest("VideoName");
                    string VideoTheme = Util.GetRequest("VideoTheme");
                    string VideoPath = Util.GetRequest("VideoPath");
                    switch (action)
                    {
                        //获取整个界面数据
                        case "GetLiveVideoList":
                            ret = GetLiveVideoList();
                            break;
                        //获取视频栏目
                        case "GetLiveVideoType":
                            ret = GetLiveVideoType(int.Parse(Id));
                            break;
                        //添加视频栏目
                        case "AddiveVideoType":
                            ret = AddiveVideoType(VideoName);
                            break;
                        //修改视频栏目
                        case "UpdateLiveVideoType":
                            ret = UpdateLiveVideoType(int.Parse(Id), VideoName);
                            break;
                        //删除视频栏目
                        case "DeleteLiveVideoType":
                            ret = DeleteLiveVideoType(int.Parse(Id));
                            break;
                        //添加视频
                        case "AddiveVideo":
                            ret = AddiveVideo(context);
                            break;
                        //删除视频
                        case "DeleteLiveVideo":
                            ret = DeleteLiveVideo(int.Parse(VideoId));
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

    //获取整个界面数据
    public string GetLiveVideoList()
    {
        DataSet ds;
        string errorMessage = string.Empty;
        JsonString js = new JsonString();
        if (DataAccess.BeforeAccess.GetLive_VideoList(1, out ds, out errorMessage))
        {
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    JsonString json = new JsonString();
                    json.Set("Id", dr["Id"] + "");
                    json.Set("VideoName", dr["VideoName"] + "");
                    js.AddArrayItem("VideoType", json);
                }
            }

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    JsonString json = new JsonString();
                    json.Set("Id", dr["id"] + "");
                    json.Set("VideoId", dr["VideoId"] + "");
                    json.Set("VideoTheme", dr["VideoTheme"] + "");
                    js.AddArrayItem("Video", json);
                }
            }
        }

        try
        {
            //WebClient client = new WebClient();
            //NetworkCredential cred = new NetworkCredential("serviceuser", "!QAZ2wsx", "playback.file.niuguwang");
            //client.Credentials = cred;

            //System.IO.DirectoryInfo TheFolder = new System.IO.DirectoryInfo(@"\\playback.file.niuguwang\trafileu1");

            //遍历文件
            //foreach (System.IO.FileInfo NextFile in TheFolder.GetFiles())
            //{
            //    JsonString json = new JsonString();
            //    json.Set("filename", NextFile.Name+ "");
            //   //json.Set("VideoPath", "http://playback.niuguwang.com/" + NextFile.Name + "");
            //    js.AddArrayItem("file", json);
            //}

            string xml = Niugu.Common.Util.Get_Http("https://playback.niuguwang.com/cgi/list.sh", 30000000, System.Text.Encoding.UTF8);

            string[] str = xml.Split(' ');
            for (int i = 0; i < str.Length; i++)
            {
                JsonString json = new JsonString();
                json.Set("filename", str[i].Replace("\r\n","") + "");
                js.AddArrayItem("file", json);
            }
            
        }
        catch (Exception ex)
        {
            
        }

        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("action", "GetLiveVideoList");
        js.Set("message", "成功");

        return js.ToString();
    }

    //获取视频栏目
    public string GetLiveVideoType(int id)
    {
        DataSet ds;
        string errorMessage = string.Empty;
        JsonString js = new JsonString();
        if (DataAccess.BeforeAccess.GetLive_VideoType(id, out ds, out errorMessage))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                js.Set("Id", dr["Id"] + "");
                js.Set("VideoName", dr["VideoName"] + "");
            }
        }

        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("action", "GetLiveVideoType");
        js.Set("message", "成功");

        return js.ToString();
    }

    //添加视频栏目
    public string AddiveVideoType(string VideoName)
    {
        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            if (!DataAccess.BeforeAccess.Addive_VideoType(VideoName, out errorMessage))
            {

                datas = "{\"result\":0,\"code\":0,\"action\":\"AddiveVideoType\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"AddiveVideoType\",\"message\":\"设置成功\"}";
            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"AddiveVideoType\",\"message\":\"失败\"}";
        }
        return datas;
    }

    //修改视频栏目
    public string UpdateLiveVideoType(int id, string VideoName)
    {
        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            if (!DataAccess.BeforeAccess.UpdateLive_VideoType(id, VideoName, out errorMessage))
            {

                datas = "{\"result\":0,\"code\":0,\"action\":\"UpdateLiveVideoType\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"UpdateLiveVideoType\",\"message\":\"设置成功\"}";
            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"UpdateLiveVideoType\",\"message\":\"失败\"}";
        }
        return datas;
    }

    //删除视频栏目
    public string DeleteLiveVideoType(int id)
    {
        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            if (!DataAccess.BeforeAccess.DeleteLive_VideoType(id, out errorMessage))
            {

                datas = "{\"result\":0,\"code\":0,\"action\":\"DeleteLiveVideoType\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"DeleteLiveVideoType\",\"message\":\"设置成功\"}";
            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"DeleteLiveVideoType\",\"message\":\"失败\"}";
        }
        return datas;
    }

    //添加视频
    public string AddiveVideo(HttpContext context)
    {
        string errorMessage = string.Empty;
        string datas = "";
        string VideoId = context.Request.Form["VideoId"].ToString();
        string VideoTheme = context.Request.Form["VideoTheme"].ToString();
        string Cover = context.Request.Form["Cover"].ToString();
        string Introduce = context.Request.Form["Introduce"].ToString();

        string image = Cover.Split(';')[0].Split('/')[1];

        byte[] byt = Convert.FromBase64String(Cover.Split(',')[1]);
        Guid filename = Guid.NewGuid();
        string topurl = "Images/" + filename.ToString() + "." + image;
        using (System.IO.Stream stream = new System.IO.MemoryStream(byt))
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(HttpContext.Current.Server.MapPath(topurl), System.IO.FileMode.CreateNew))
            {
                //将得到的文件流复制到写入流中
                stream.CopyTo(fs);
                fs.Flush();
            }
        }
        
        try
        {
            if (!DataAccess.BeforeAccess.Addive_Video(VideoId, VideoTheme.Split('.')[0], "http://playback.fxtrade888.com/" + VideoTheme, topurl, Introduce, out errorMessage))
            {

                datas = "{\"result\":0,\"code\":0,\"action\":\"AddiveVideo\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"AddiveVideo\",\"message\":\"设置成功\"}";
            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"AddiveVideo\",\"message\":\"失败\"}";
        }
        return datas;
    }

    //删除视频
    public string DeleteLiveVideo(int id)
    {
        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            if (!DataAccess.BeforeAccess.DeleteLive_Video(id, out errorMessage))
            {

                datas = "{\"result\":0,\"code\":0,\"action\":\"DeleteLiveVideo\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"DeleteLiveVideo\",\"message\":\"设置成功\"}";
            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"DeleteLiveVideo\",\"message\":\"失败\"}";
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