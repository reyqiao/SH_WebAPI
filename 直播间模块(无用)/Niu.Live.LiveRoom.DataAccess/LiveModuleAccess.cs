using Niu.Live.LiveRoom.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Niu.Live.LiveRoom.DataAccess
{
    public class LiveModuleAccess
    {
        public static IEnumerable<LiveModule> GetAllLiveModule()
        {
            return SystemConnection.OutFun<IEnumerable<LiveModule>>(conn => conn.Query<LiveModule>(@"SELECT [ModuleId]
                                                                                                          ,[ModuleName]
                                                                                                          ,[ModuleUrl]
                                                                                                          ,[ModuleWeight]
                                                                                                          ,[LiveId]
                                                                                                          ,[Display],[ModuleGoUrl],[Moduletype]
                                                                                                      FROM [DB_Live].[dbo].[LiveModule] order by ModuleWeight desc"));
        }

        public static bool AddLiveModule(LiveModule livemodule)
        {
            return SystemConnection.OutFun<int>(conn => conn.Execute(@"INSERT INTO [DB_Live].[dbo].[LiveModule]
                                                                               ([ModuleName]
                                                                               ,[LiveId]
                                                                               ,[ModuleUrl],[ModuleGoUrl],[Moduletype])
                                                                         VALUES
                                                                               (@ModuleName
                                                                               ,@LiveId
                                                                               ,@ModuleUrl,@ModuleGoUrl,@Moduletype)", livemodule)) > 0;
        }

        public static bool UpdateLiveModule(LiveModule livemodule)
        {
            return SystemConnection.OutFun<int>(conn => conn.Execute(@"UPDATE [DB_Live].[dbo].[LiveModule]
                                                                           SET [ModuleName] = @ModuleName
                                                                              ,[ModuleUrl] = @ModuleUrl
                                                                              ,[ModuleWeight] = @ModuleWeight
                                                                              ,[Display] = @Display
                                                                              ,[ModuleGoUrl]=@ModuleGoUrl
                                                                         WHERE [ModuleId]=@ModuleId and [LiveId]=@LiveId", livemodule)) > 0;
        }
        public static IEnumerable<LiveModule> FindOneLiveModule(long liveId)
        {
            return SystemConnection.OutFun<IEnumerable<LiveModule>>(con => con.Query<LiveModule>(@"SELECT[ModuleName],[Moduletype]
                                                                                                          ,[ModuleUrl]
                                                                                                          ,[ModuleWeight]
                                                                                                          ,[Display],[ModuleGoUrl]
                                                                                                      FROM [DB_Live].[dbo].[LiveModule]  where [LiveId]=@liveId and [Display]=1 order by ModuleWeight desc", new { liveId }));
        }
    }
}
