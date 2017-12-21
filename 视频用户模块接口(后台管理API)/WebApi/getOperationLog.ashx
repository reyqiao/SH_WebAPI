<%@ WebHandler Language="C#" Class="getOperationLog" %>

using System;
using System.Web;
using System.Linq;
using System.Data;
using System.Text;
using System.Collections.Generic;
using DataAccess;
using Niugu.Common;

public class getOperationLog : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) 
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.ContentType = "application/json";
        context.Response.Charset = "UTF-8";

        string mobile = Utility.GetParam<string>(context.Request["mobile"]) ?? "";
        string nickName = Utility.GetParam<string>(context.Request["nickName"]) ?? "";
        int channelId = Utility.GetParam<int>(context.Request["channelId"], 0);
        int source = Utility.GetParam<int>(context.Request["source"], 0);
        string addTime = Utility.GetParam<string>(context.Request["addTime"]) ?? "";

        int pageSize = Utility.GetParam<int>(context.Request["pageSize"], 20);
        long pageIndex = Utility.GetParam<long>(context.Request["pageindex"], 1L);
        
        long startIndex = 0;
        long endIndex = 0;

        startIndex = (pageIndex - 1) * pageSize + 1;
        endIndex = pageIndex * pageSize;

        DataSet ds = UserAccess.getOperationLogPaging(mobile, nickName, channelId, source, addTime, startIndex, endIndex);

        //获取代理数据并缓存
        DataSet channelDs = UserAccess.getChannelList();
        
        List<dynamic> dyList = new List<dynamic>();
        List<dynamic> channelList = new List<dynamic>();
        int tempSource = 0;
        int tempChannelId = 0;
        int tempOperateType = 0;
        string strChannel = "";
        DataRow[] drArray = null;
        
        if(Utility.NotNullDataSet(ds))
        {
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                dynamic dyData = new System.Dynamic.ExpandoObject();

                tempChannelId = Convert.ToInt16(dr["channelId"]);
                tempSource = Convert.ToInt16(dr["source"]);
                tempOperateType = Convert.ToInt16(dr["operateType"]);
                
                if(Utility.NotNullDataSet(channelDs))
                {
                    drArray = channelDs.Tables[0].Select(string.Format(" channelId = {0}", tempChannelId));

                    if(drArray.Length >0)
                       strChannel = drArray[0]["channelName"].ToString();
                }
                
                dyData.id = Convert.ToInt64(dr["id"]);
                dyData.channelName = strChannel;
                dyData.nickName = dr["nickName"].ToString();
                dyData.mobile = dr["mobile"].ToString();
                dyData.roleName = dr["roleName"].ToString();
                dyData.source = tempSource == 1 ? "web端" : tempSource == 2 ? "h5" : "";
                dyData.operateType = tempOperateType == 1 ? "登录" : tempOperateType == 2 ? "登出" : "";
                dyData.addTime = Util.GetDateTimeValue(dr["addTime"].ToString(),DateTime.MinValue).ToString("yy-MM-dd hh:mm");
                
                dyList.Add(dyData);
            }
        }

        if (Utility.NotNullDataSet(channelDs))
        {
            foreach (System.Data.DataRow channelDr in channelDs.Tables[0].Rows)
            {
                dynamic dyData = new System.Dynamic.ExpandoObject();
                
                dyData.channelId = channelDr["ChannelId"].ToString();
                dyData.channelName = channelDr["channelName"].ToString();

                channelList.Add(dyData);
            }
        }

        int recordCount = (ds != null && ds.Tables[1] != null && ds.Tables[1].Rows.Count >0) ? Util.GetIntValue(ds.Tables[1].Rows[0][0].ToString(), 0) : 0;
        
        var data = new { result = 1, code = 0, message = "获取成功", totalcount = recordCount, data = dyList, channelList = channelList };

        context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(data));
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}