using Niugu.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class UserAccess
    {
        // Token: 0x06000069 RID: 105 RVA: 0x00004CEC File Offset: 0x00002EEC
        public static DataSet getOperationLogPaging(string mobile, string nickName, int channelId, int source, string addTime, long startIndex, long endIndex)
        {
            DataSet result = null;
            string empty = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(" 1=1 ");
            if (!string.IsNullOrEmpty(mobile))
            {
                stringBuilder.AppendFormat(" and mobile = '{0}' ", mobile);
            }
            if (!string.IsNullOrEmpty(nickName))
            {
                stringBuilder.AppendFormat(" and nickName like '{0}%' ", nickName);
            }
            if (channelId > 0)
            {
                stringBuilder.AppendFormat(" and channelId = {0}", channelId);
            }
            if (source > 0)
            {
                stringBuilder.AppendFormat(" and source = {0} ", source);
            }
            if (!string.IsNullOrEmpty(addTime))
            {
                DateTime dateTimeValue = Util.GetDateTimeValue(addTime, DateTime.MinValue);
                if (dateTimeValue != DateTime.MinValue)
                {
                    stringBuilder.AppendFormat(" and addTime >= '{0}' and addTime <= '{1}' ", string.Format("{0} 00:00:00:000", dateTimeValue.ToString("yyyy-MM-dd")), string.Format("{0} 23:29:59:999", dateTimeValue.ToString("yyyy-MM-dd")));
                }
            }
            string text = " select * from     (     select Id,ChannelId,NickName,Mobile,RoleName,Source,OperateType,AddTime,row_number() over(order by AddTime desc) rowid        from [dbo].[UserOperationLog] with(nolock)             where {0}       ) a where a.rowid BETWEEN {1} and {2};  select count(Id)     from [dbo].[UserOperationLog] with(nolock)      where {0}  ";
            text = string.Format(text, stringBuilder, startIndex, endIndex);
            SQLCommon.ExecuteDataset(text, UserAccess.DB_Live, out result, out empty);
            return result;
        }

        // Token: 0x0600006A RID: 106 RVA: 0x00004DF8 File Offset: 0x00002FF8
        public static DataSet getOperationLog(string mobile, string nickName, int channelId, int source, string addTime)
        {
            DataSet result = null;
            string empty = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(" 1=1 ");
            if (!string.IsNullOrEmpty(mobile))
            {
                stringBuilder.AppendFormat(" and mobile = '{0}' ", mobile);
            }
            if (!string.IsNullOrEmpty(nickName))
            {
                stringBuilder.AppendFormat(" and nickName like '{0}%' ", nickName);
            }
            if (channelId > 0)
            {
                stringBuilder.AppendFormat(" and channelId = {0}", channelId);
            }
            if (source > 0)
            {
                stringBuilder.AppendFormat(" and source = {0} ", source);
            }
            if (!string.IsNullOrEmpty(addTime))
            {
                DateTime dateTimeValue = Util.GetDateTimeValue(addTime, DateTime.MinValue);
                if (dateTimeValue != DateTime.MinValue)
                {
                    stringBuilder.AppendFormat(" and addTime >= '{0}' and addTime <= '{1}' ", string.Format("{0} 00:00:00:000", dateTimeValue.ToString("yyyy-MM-dd")), string.Format("{0} 23:29:59:999", dateTimeValue.ToString("yyyy-MM-dd")));
                }
            }
            string text = " select Id,ChannelId,NickName,Mobile,RoleName,Source,OperateType,AddTime                               from [dbo].[UserOperationLog] with(nolock)                                where {0}                              ";
            text = string.Format(text, stringBuilder);
            SQLCommon.ExecuteDataset(text, UserAccess.DB_Live, out result, out empty);
            return result;
        }

        // Token: 0x0600006B RID: 107 RVA: 0x00004EF4 File Offset: 0x000030F4
        public static DataSet getChannelList()
        {
            DataSet result = null;
            string empty = string.Empty;
            string text = " select channelId,channelName from  dbo.[Channels] with(nolock) ";
            SQLCommon.ExecuteDataset(text, UserAccess.DB_Live, out result, out empty);
            return result;
        }

        // Token: 0x04000008 RID: 8
        public static string DB_Live = ConfigurationManager.ConnectionStrings["DB_Live"].ConnectionString;
    }
}

