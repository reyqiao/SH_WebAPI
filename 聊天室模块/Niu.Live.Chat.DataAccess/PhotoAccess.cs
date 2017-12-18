using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
namespace Niu.Live.Chat.DataAccess
{
    public class PhotoAccess
    {

        static string DB_LiveConnection = ConfigurationManager.ConnectionStrings["DB_Live"].ConnectionString;
        public static bool AddPhoto(long userId, string hashId, int width, int height, string extension, string ip, out long photoId, out DateTime addTime)
        {
            photoId = 0;
            addTime = DateTime.Now;
            string errormessage = string.Empty;
            var parameters = new DynamicParameters();
            parameters.Add("@userid", userId);
            parameters.Add("@hashid", hashId);
            parameters.Add("@width", width);
            parameters.Add("@height", height);
            parameters.Add("@extension", extension);
            parameters.Add("@ip", ip);
            parameters.Add("@photoid", photoId, DbType.Int64, ParameterDirection.Output);
            parameters.Add("@addtime", addTime, DbType.DateTime, ParameterDirection.Output);
            //parameters.Add(new SqlParameter() { ParameterName = "@userid", Value = userId });
            //parameters.Add(new SqlParameter() { ParameterName = "@hashid", Value = hashId });
            //parameters.Add(new SqlParameter() { ParameterName = "@width", Value = width });
            //parameters.Add(new SqlParameter() { ParameterName = "@height", Value = height });
            //parameters.Add(new SqlParameter() { ParameterName = "@extension", Value = extension });
            //parameters.Add(new SqlParameter() { ParameterName = "@ip", Value = ip });
            //parameters.Add(new SqlParameter() { ParameterName = "@photoid", Value = photoId, Direction = ParameterDirection.Output });
            //parameters.Add(new SqlParameter() { ParameterName = "@addtime", Value = addTime, Direction = ParameterDirection.Output });
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                var temp = conn.Execute("GraphicLivePhoto_Add", parameters, null, null, CommandType.StoredProcedure);
                photoId = parameters.Get<Int64>("photoid");
                addTime = parameters.Get<DateTime>("addtime");
                return true;
                //if (temp.IsConsumed)
                //{
                //    photoId = temp.Read<long>().Single();
                //    addTime = temp.Read<DateTime>().Single();
                //    return true;
                //}

                // var t = conn.Execute("GraphicLivePhoto_Add", parameters, null, null, CommandType.StoredProcedure);
                //if (conn.Execute("GraphicLivePhoto_Add", parameters, null, null, CommandType.StoredProcedure) > 0)
                //{
                //  photoId = parameters.Get<Int64>("@photoid");
                // addTime = parameters.Get<DateTime>("@addTime");
                //}
            }
        }

        public static bool UpdateSaveFilePath(long photoId, string saveFilePath)
        {
            string errormessage = string.Empty;
            var parameters = new DynamicParameters();
            parameters.Add("@ID", photoId);
            parameters.Add("@saveFilePath", saveFilePath);
            //SqlParameter[] thisParams = new SqlParameter[2];
            //thisParams[0] = new System.Data.SqlClient.SqlParameter("@ID", photoId);
            //thisParams[0].Direction = ParameterDirection.Input;
            //thisParams[1] = new System.Data.SqlClient.SqlParameter("@saveFilePath", saveFilePath);
            //thisParams[1].Direction = ParameterDirection.Input;
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                //var temp = conn.Execute("GraphicLivePhoto_Update_SaveFilePath", parameters, null, null, CommandType.StoredProcedure);
                return conn.Execute("GraphicLivePhoto_Update_SaveFilePath", parameters, null, null, CommandType.StoredProcedure) == -1 ? true : false;
            }
            // return SQLCommon.ExecuteStoredProcedure("GraphicLivePhoto_Update_SaveFilePath", DB_LiveConnection, ref thisParams, out errormessage);
        }


    }
}