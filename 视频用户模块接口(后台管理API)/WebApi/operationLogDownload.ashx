<%@ WebHandler Language="C#" Class="operationLogDownload" %>

using System;
using System.Web;
using System.Linq;
using System.Data;
using System.Text;
using System.IO;
using System.Collections.Generic;
using DataAccess;
using Niugu.Common;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public class operationLogDownload : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.ContentType = "application/json";
        context.Response.Charset = "UTF-8";

        string mobile = Utility.GetParam<string>(context.Request["mobile"]) ?? "";
        string nickName = Utility.GetParam<string>(context.Request["nickName"]) ?? "";
        int channelId = Utility.GetParam<int>(context.Request["channelId"], 0);
        int source = Utility.GetParam<int>(context.Request["source"], 0);
        string addTime = Utility.GetParam<string>(context.Request["addTime"]) ?? "";

        DataSet ds = UserAccess.getOperationLog(mobile, nickName, channelId, source, addTime);

        //获取代理数据并缓存
        DataSet channelDs = UserAccess.getChannelList();

        List<dynamic> dyList = new List<dynamic>();
        int tempSource = 0;
        int tempChannelId = 0;
        int tempOperateType = 0;
        string strChannel = "";
        DataRow[] drArray = null;

        if (Utility.NotNullDataSet(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dynamic dyData = new System.Dynamic.ExpandoObject();

                tempChannelId = Convert.ToInt16(dr["channelId"]);
                tempSource = Convert.ToInt16(dr["source"]);
                tempOperateType = Convert.ToInt16(dr["operateType"]);


                if (Utility.NotNullDataSet(channelDs))
                {
                    drArray = channelDs.Tables[0].Select(string.Format(" channelId = {0}", tempChannelId));

                    strChannel = drArray[0]["channelName"].ToString();
                }

                dyData.id = Convert.ToInt64(dr["id"]);
                dyData.channelId = strChannel;
                dyData.nickName = dr["nickName"].ToString();
                dyData.mobile = dr["mobile"].ToString();
                dyData.roleName = dr["roleName"].ToString();
                dyData.source = tempSource == 1 ? "web端" : tempSource == 2 ? "h5" : "";
                dyData.operateType = tempOperateType == 1 ? "登录" : tempOperateType == 2 ? "登出" : "";
                dyData.addTime = Util.GetDateTimeValue(dr["addTime"].ToString(), DateTime.MinValue).ToString("yy-MM-dd hh:mm");

                dyList.Add(dyData);
            }
        }

        dataTableToExcel(ds.Tables[0], context);
    }

    /// <summary>
    /// 列表导出到 Excel
    /// </summary>
    /// <param name="userList"></param>
    private void dataTableToExcel(DataTable dt, HttpContext context)
    {
        string file = string.Format("登录退出{0}.xls", DateTime.Now.ToString("yyMMddHHmmss"));

        IWorkbook workbook;
        workbook = new HSSFWorkbook();

        ISheet sheet = workbook.CreateSheet("登录退出查询列表");

        //表头  
        IRow row = sheet.CreateRow(0);
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            ICell cell = row.CreateCell(i);
            cell.SetCellValue(dt.Columns[i].ColumnName);
        }

        //数据  
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            IRow row1 = sheet.CreateRow(i + 1);
            for (int j = 0; j < 8; j++)
            {
                ICell cell = row1.CreateCell(j);
                cell.SetCellValue(dt.Rows[i][j].ToString());
            }
        }

        context.Response.ContentType = "application/vnd.ms-excel";
        context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", file));
        context.Response.Clear();

        using (MemoryStream ms = new MemoryStream())
        {
            //将工作簿的内容放到内存流中
            workbook.Write(ms);
            //将内存流转换成字节数组发送到客户端
            context.Response.BinaryWrite(ms.GetBuffer());
            context.Response.End();
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}