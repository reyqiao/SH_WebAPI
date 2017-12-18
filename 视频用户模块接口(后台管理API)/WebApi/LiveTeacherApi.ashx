<%@ WebHandler Language="C#" Class="LiveTeacherApi" %>

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
public class LiveTeacherApi : IHttpHandler
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
                    int category = Util.GetIntValue(Util.GetRequest("teachername"), 0);//当前类型
                    string Id = Util.GetRequest("Id");
                    string teachername = Util.GetRequest("teachername");
                    string TeacherFace = Util.GetRequest("TeacherFace");
                    string TeacherTag = Util.GetRequest("TeacherTag");
                    string WinRate = Util.GetRequest("WinRate");
                    string Income = Util.GetRequest("Income");
                    string Introduce = Util.GetRequest("Introduce");
                    string Teacherinfo = Util.GetRequest("Teacherinfo");
                    switch (action)
                    {
                        //老师介绍列表
                        case "GetLiveTeacherList":
                            ret = GetLiveTeacherList();
                            break;
                        //老师列表编辑获取
                        case "GetLiveTeacher":
                            ret = GetLiveTeacher(int.Parse(Id));
                            break;
                        //老师添加
                        case "AddLiveTeacher":
                            ret = AddLiveTeacher(context);
                            break;
                        //老师列表编辑
                        case "UpdateLiveTeacher":
                            ret = UpdateLiveTeacher(context);
                            break;
                        //老师删除
                        case "DeleteLiveTeacher":
                            ret = DeleteLiveTeacher(int.Parse(Id));
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
                json.Set("TeacherFace", "http://live.inquant.cn/Api/" + dr["TeacherFace"] + "");
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

    //老师列表编辑获取
    public string GetLiveTeacher(int id)
    {
        DataSet ds;
        string errorMessage = string.Empty;
        JsonString js = new JsonString();
        if (DataAccess.BeforeAccess.GetLive_Teacher(id, out ds, out errorMessage))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                js.Set("Id", dr["Id"] + "");
                js.Set("teachername", dr["teachername"] + "");
                js.Set("TeacherFace", dr["TeacherFace"] + "");
                js.Set("TeacherTag", dr["TeacherTag"] + "");
                js.Set("WinRate", dr["WinRate"] + "");
                js.Set("Income", dr["Income"] + "");
                js.Set("teacherinfo", dr["teacherinfo"] + "");
                js.Set("Introduce", dr["Introduce"] + "");
            }
        }

        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("action", "GetLiveTeacher");
        js.Set("message", "成功");

        return js.ToString();
    }

    //老师添加
    public string AddLiveTeacher(HttpContext context)
    {
        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            string teachername = context.Request.Form["teachername"].ToString();
            string TeacherFace = context.Request.Form["TeacherFace"].ToString();
            string TeacherTag = context.Request.Form["TeacherTag"].ToString();
            string WinRate = context.Request.Form["WinRate"].ToString();
            string Income = context.Request.Form["Income"].ToString();
            string teacherinfo = context.Request.Form["teacherinfo"].ToString();
            string Introduce = context.Request.Form["Introduce"].ToString();

            string image = TeacherFace.Split(';')[0].Split('/')[1];

            byte[] byt = Convert.FromBase64String(TeacherFace.Split(',')[1]);
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

            if (!DataAccess.BeforeAccess.AddLive_Teacher(teachername, topurl, TeacherTag, WinRate, Income, Introduce, teacherinfo, out errorMessage))
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

    //老师列表编辑
    public string UpdateLiveTeacher(HttpContext context)
    {
        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            string id = context.Request.Form["id"].ToString();
            string teachername = context.Request.Form["teachername"].ToString();
            string TeacherFace = context.Request.Form["TeacherFace"].ToString();
            string TeacherTag = context.Request.Form["TeacherTag"].ToString();
            string WinRate = context.Request.Form["WinRate"].ToString();
            string Income = context.Request.Form["Income"].ToString();
            string teacherinfo = context.Request.Form["teacherinfo"].ToString();
            string Introduce = context.Request.Form["Introduce"].ToString();

            if (TeacherFace.IndexOf("data:image".ToLower()) >= 0)
            {
                string image = TeacherFace.Split(';')[0].Split('/')[1];

                byte[] byt = Convert.FromBase64String(TeacherFace.Split(',')[1]);
                Guid filename = Guid.NewGuid();
                TeacherFace = "Images/" + filename.ToString() + "." + image;
                using (System.IO.Stream stream = new System.IO.MemoryStream(byt))
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(HttpContext.Current.Server.MapPath(TeacherFace), System.IO.FileMode.CreateNew))
                    {
                        //将得到的文件流复制到写入流中
                        stream.CopyTo(fs);
                        fs.Flush();
                    }
                }
            }

            if (!DataAccess.BeforeAccess.UpdateLive_Teacher(int.Parse(id), teachername, TeacherFace, TeacherTag, WinRate, Income, Introduce, teacherinfo, out errorMessage))
            {

                datas = "{\"result\":0,\"code\":0,\"action\":\"UpdateLiveTeacher\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"UpdateLiveTeacher\",\"message\":\"设置成功\"}";
            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"UpdateLiveTeacher\",\"message\":\"失败\"}";
        }
        return datas;
    }

    //老师列表编辑
    public string DeleteLiveTeacher(int id)
    {
        string errorMessage = string.Empty;
        string datas = "";
        try
        {
            if (!DataAccess.BeforeAccess.DeleteLive_Teacher(id, out errorMessage))
            {

                datas = "{\"result\":0,\"code\":0,\"action\":\"DeleteLiveTeacher\",\"message\":\"" + errorMessage + "\"}";
            }
            else
            {
                datas = "{\"result\":1,\"code\":0,\"action\":\"DeleteLiveTeacher\",\"message\":\"设置成功\"}";
            }
        }
        catch (Exception)
        {
            datas = "{\"result\":0,\"code\":0,\"action\":\"DeleteLiveTeacher\",\"message\":\"失败\"}";
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