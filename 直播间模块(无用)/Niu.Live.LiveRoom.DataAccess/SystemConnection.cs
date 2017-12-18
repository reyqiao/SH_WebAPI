using Niu.Cabinet.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.LiveRoom.DataAccess
{
   public class SystemConnection
    {
       public static string DB_Live = ConnectionString.Get("DB_LiveConnection", string.Empty);

       public static T OutFun<T>(Func<IDbConnection, T> func)
       {
           using (IDbConnection conn = new SqlConnection(SystemConnection.DB_Live))
           {
               return func(conn);
           }
       }

       public static void Fun(Action<IDbConnection> action)
       {
           using (IDbConnection conn = new SqlConnection(SystemConnection.DB_Live))
           {
               action(conn);
           }
       }
    }
}
