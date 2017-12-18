using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Net;
using Niugu.Common;
using NiuGu.Utility;

namespace DataAccess
{
    public class LiveMesMagAccess
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["DB_CFD"].ConnectionString;
        public static string DB_Live = ConfigurationManager.ConnectionStrings["DB_Live"].ConnectionString;

        public static bool AddBlackList(long userId, long adminId, out int flag,out string errorMessage)
        {
            flag = 0;
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[3];
            thisParams[0] = new SqlParameter("@UserId", userId);
            thisParams[0].Direction = ParameterDirection.Input;

            thisParams[1] = new SqlParameter("@AdminId", adminId);
            thisParams[1].Direction = ParameterDirection.Input;

            thisParams[2] = new SqlParameter("@Flag", SqlDbType.Int, 32);
            thisParams[2].Direction = ParameterDirection.Output;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[SP_LiveMag_AddBlackList]", DB_Live, ref thisParams, out errorMessage))
            {
                if (DBNull.Value != thisParams[2].Value)
                {
                    flag = Convert.ToInt32(thisParams[2].Value);
                    return true;
                }
            }
            return false;
        }
        public static bool AddBlackList(long userId, long adminId, string ip, out int flag, out string errorMessage)
        {
            flag = 0;
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[4];
            thisParams[0] = new SqlParameter("@UserId", userId);
            thisParams[0].Direction = ParameterDirection.Input;

            thisParams[1] = new SqlParameter("@AdminId", adminId);
            thisParams[1].Direction = ParameterDirection.Input;

            thisParams[2] = new SqlParameter("@Flag", SqlDbType.Int, 32);
            thisParams[2].Direction = ParameterDirection.Output;

            thisParams[3] = new SqlParameter("@IP", ip);
            thisParams[3].Direction = ParameterDirection.Input;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[SP_LiveMag_AddBlackList_V1]", DB_Live, ref thisParams, out errorMessage))
            {
                if (DBNull.Value != thisParams[2].Value)
                {
                    flag = Convert.ToInt32(thisParams[2].Value);
                    return true;
                }
            }
            return false;
        }

        public static bool GetBlackListByPage(int pageindex, int pagesize, out int totalcount, out DataSet ds, out string errorMessage)
        {
            ds = null;
            totalcount = 0;
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[3];

            thisParams[0] = new SqlParameter("@PageIndex", pageindex);
            thisParams[0].Direction = ParameterDirection.Input;

            thisParams[1] = new SqlParameter("@PageSize", pagesize);
            thisParams[1].Direction = ParameterDirection.Input;

            thisParams[2] = new SqlParameter("@TotalCount", SqlDbType.Int, 32);
            thisParams[2].Direction = ParameterDirection.Output;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[SP_BlackList_GetPageList]", DB_Live, ref thisParams, out ds, out errorMessage))
            {
                totalcount = Convert.ToInt32(thisParams[2].Value);
                return true;

            }
            return false;


        }

        public static bool AddCallOrder(long tid, int ctype, string ctypename, int dir, string jcprice, string zyprice, string zsprice, out string errorMessage)
        {
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[7];
            thisParams[0] = new SqlParameter("@TId", tid);
            thisParams[0].Direction = ParameterDirection.Input;

            thisParams[1] = new SqlParameter("@CType", ctype);
            thisParams[1].Direction = ParameterDirection.Input;

            thisParams[2] = new SqlParameter("@CTypeName", ctypename);
            thisParams[2].Direction = ParameterDirection.Input;

            thisParams[3] = new SqlParameter("@Dir", dir);
            thisParams[3].Direction = ParameterDirection.Input;

            thisParams[4] = new SqlParameter("@JCPrice", jcprice);
            thisParams[4].Direction = ParameterDirection.Input;

            thisParams[5] = new SqlParameter("@ZYPrice", zyprice);
            thisParams[5].Direction = ParameterDirection.Input;

            thisParams[6] = new SqlParameter("@ZSPrice", zsprice);
            thisParams[6].Direction = ParameterDirection.Input;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[SP_CallOrder_Add]", DB_Live, ref thisParams, out errorMessage))
            {
                return true;
            }
            return false;
        }
        public static bool PcOrder(long tid, long callid, string pcprice, out int flag, out string errorMessage)
        {
           
            flag = 0;
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[4];
            thisParams[0] = new SqlParameter("@TId", tid);
            thisParams[0].Direction = ParameterDirection.Input;

            thisParams[1] = new SqlParameter("@CId", callid);
            thisParams[1].Direction = ParameterDirection.Input;

            thisParams[2] = new SqlParameter("@PcPrice", pcprice);
            thisParams[2].Direction = ParameterDirection.Input;

            thisParams[3] = new SqlParameter("@Flag", SqlDbType.Int, 32);
            thisParams[3].Direction = ParameterDirection.Output;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[SP_CallOrder_PC]", DB_Live, ref thisParams, out errorMessage))
            {
                if (DBNull.Value != thisParams[3].Value)
                {
                    flag = Convert.ToInt32(thisParams[3].Value);
                    return true;
                }
            }
            return false;
        }


        public static bool GetCallOrderList(string search, int pageindex, int pagesize, out DataSet ds, out int totalCount, out string errorMessage)
        {
            ds = null;
            errorMessage = string.Empty;
            totalCount = 0;
            SqlParameter[] thisParams = new SqlParameter[4];
            thisParams[0] = new SqlParameter("@Search", search);
            thisParams[0].Direction = ParameterDirection.Input;

            thisParams[1] = new SqlParameter("@PageIndex", pageindex);
            thisParams[1].Direction = ParameterDirection.Input;

            thisParams[2] = new SqlParameter("@PageSize", pagesize);
            thisParams[2].Direction = ParameterDirection.Input;

            thisParams[3] = new SqlParameter("@TotalCount", SqlDbType.Int, 32);
            thisParams[3].Direction = ParameterDirection.Output;

            if (Niugu.Common.SQLCommon.ExecuteStoredProcedure("[dbo].[SP_CallOrder_List]", DB_Live, ref thisParams, out ds, out errorMessage))
            {
                if (DBNull.Value != thisParams[3].Value)
                {
                    totalCount = Convert.ToInt32(thisParams[3].Value);
                    return true;
                }
            }
            return false;

        }

        public static bool GetALlTeacherList(out DataSet ds, out string errorMessage)
        {
            ds = null;
            errorMessage = string.Empty;
            string sql = " select a.UserId,a. NickName from [DB_Live].[dbo].[User_Role_Live] b join  [DB_Live].[dbo].[User] a on a.userid=b.userid  where b.roleid=14";
            if (Niugu.Common.SQLCommon.ExecuteDataset(sql, DB_Live,out ds,out errorMessage))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public static bool DelCallOrder(long tid, long callid, out int flag, out string errorMessage)
        {

            flag = 0;
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[3];
            thisParams[0] = new SqlParameter("@TId", tid);
            thisParams[0].Direction = ParameterDirection.Input;

            thisParams[1] = new SqlParameter("@CId", callid);
            thisParams[1].Direction = ParameterDirection.Input;

            thisParams[2] = new SqlParameter("@Flag", SqlDbType.Int, 32);
            thisParams[2].Direction = ParameterDirection.Output;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[SP_CallOrder_Del]", DB_Live, ref thisParams, out errorMessage))
            {
                if (DBNull.Value != thisParams[2].Value)
                {
                    flag = Convert.ToInt32(thisParams[2].Value);
                    return true;
                }
            }
            return false;
        }

        public static bool DeleteBlackList(long userId, out int flag, out string errorMessage)
        {
            flag = 0;
            errorMessage = string.Empty;
            SqlParameter[] thisParams = new SqlParameter[2];
            thisParams[0] = new SqlParameter("@UserId", userId);
            thisParams[0].Direction = ParameterDirection.Input;

            thisParams[1] = new SqlParameter("@Flag", SqlDbType.Int, 32);
            thisParams[1].Direction = ParameterDirection.Output;

            if (SQLCommon.ExecuteStoredProcedure("[dbo].[SP_LiveMag_DelBlackList]", DB_Live, ref thisParams, out errorMessage))
            {
                if (DBNull.Value != thisParams[1].Value)
                {
                    flag = Convert.ToInt32(thisParams[1].Value);
                    return true;
                }
            }
            return false;
        }

        

    }
}
