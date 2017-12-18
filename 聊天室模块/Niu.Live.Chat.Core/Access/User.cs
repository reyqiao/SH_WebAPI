using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Niu.Cabinet.Config;
using Niu.Live.Chat.Core.Model;

namespace Niu.Live.Chat.Core.Access
{
    public class User
    {
        private static string DB_LiveConnection = ConnectionString.Get("DB_Live", String.Empty);



        private static List<Im_User> GetDataRowEntity(string condition)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                return conn.Query<Im_User>("SELECT * FROM dbo.NeteaseImUser (NOLOCK) WHERE " + condition).ToList();
            }
        }

        public static Im_User UserQuery(string accId)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                return conn.Query<Im_User>(string.Format("SELECT * FROM dbo.NeteaseImUser (NOLOCK) WHERE accId = '{0}'", accId)).SingleOrDefault();
            }
        }
        public static Im_User UserQueryById(long Id)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                return conn.Query<Im_User>(string.Format("SELECT * FROM dbo.NeteaseImUser (NOLOCK) WHERE UserId = '{0}'", Id)).SingleOrDefault();
            }
        }

        public static int UserInsert(string accId, string name, string token, long userId)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                Im_User user = new Im_User() { accid = accId, name = name, token = token, userId = userId };
                string sql = "INSERT INTO [dbo].[NeteaseImUser]([accid],[name],[token],[userId])VALUES(@accid,@name,@token,@userId)";
                return conn.Execute(sql, user);
            }
        }
        public static string QueryImUserByUserId(long userId)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = "select accid from NeteaseImUser where UserId=@UserId ";
                return conn.Query<string>(sql, new { userId }).FirstOrDefault();
            }
        }


        public static int UserFailureLogInsert(string accId, string name, string token, string code, string desc)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = "INSERT INTO [dbo].[NeteaseImUserFailureLog]([accid],[name],[token],[code],[desc])VALUES(@accid,@name,@token,@code,@desc)";
                return conn.Execute(sql, new { accid = accId, name = name, token = token, code = code, desc = desc });
            }
        }



        public static int UserUpdate(string accId, string name, string token)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                Im_User user = new Im_User() { accid = accId, name = name, token = token };
                string sql = "UPDATE [dbo].[NeteaseImUser] SET [name] = @name, [token] = @token WHERE accid=@accid";
                return conn.Execute(sql, user);
            }
        }

        public static int UserUpdateColumn(string accId, dynamic parameters)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                List<string> list = new List<string>();

                var dict = (IDictionary<string, object>)parameters;

                foreach (KeyValuePair<string, object> kv in dict)
                {
                    list.Add(string.Format("[{0}] = @{0}", kv.Key));
                }

                dict["accid"] = accId;

                string sql = string.Format("UPDATE [dbo].[NeteaseImUser] SET {0} WHERE accid=@accid", string.Join(",", list));
                return conn.Execute(sql, (object)parameters);
            }
        }



        public static int UserBlockUpdate(string accId, bool blockFlag, long niuguAdmin, string niuguRemark)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = @"UPDATE [dbo].[NeteaseImUser] SET [blockFlag] = @blockFlag  WHERE accid=@accid
                               INSERT INTO [dbo].[NeteaseImUserBlockLog]([accId],[blockFlag],[niuguAdmin],[niuguRemark])VALUES(@accId,@blockFlag,@niuguAdmin,@niuguRemark)
                               ";
                return conn.Execute(sql, new { accId = accId, blockFlag = blockFlag, niuguAdmin = niuguAdmin, niuguRemark = niuguRemark });
            }
        }



    }
}