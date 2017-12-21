<%@ WebHandler Language="C#" Class="getChannelList" %>

using System;
using System.Web;
using DataAccess;
using System.Data;
using Niugu.Common;

public class getChannelList : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.ContentType = "application/json";
        context.Response.Charset = "UTF-8";

        DataSet ds = null;
        JsonString js = new JsonString();

        js.Set("result", "1");
        js.Set("code", "0");
        js.Set("message", "成功");

        Niu.Cabinet.Caching.CacheHelper.GetOrSetCache<DataSet>("cacheChannel", out ds, () =>
        {
            return UserAccess.getChannelList();
        }, 60 * 5);

        foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
        {
            JsonString json = new JsonString();
            json.Set("channelId", dr["ChannelId"].ToString());
            json.Set("channelName", dr["channelName"].ToString());

            js.AddArrayItem("channelList", json);
        }

        context.Response.Write(js.ToString());
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}