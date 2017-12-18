using Niu.Live.Video.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Niu.Live.Video.DataAccess
{
    public class FreeVideoAccess
    {
        public static IEnumerable<FreeVideoModel> GetAllByLiveId(long liveid)
        {
            return SystemConnection.OutFun(conn => conn.Query<FreeVideoModel, VideoModel, FreeVideoModel>(@"SELECT *
  FROM [DB_Live].[dbo].[FreeVideo] f join  [DB_Live].[dbo].[LiveVideo] v on f.LiveId =v.LiveId where f.LiveId=@liveId", (f, v) => { f.Video = v; return f; }, new { liveid = liveid }, splitOn: "LiveId"));
        }

        public static bool AddFreeVideo(FreeVideoModel freevideo)
        {
            return SystemConnection.OutFun<int>(conn => conn.Execute(@"INSERT INTO [DB_Live].[dbo].[FreeVideo]
                                                                               ([VideoId]
                                                                               ,[LiveId])
                                                                         VALUES
                                                                               (@VideoId
                                                                               ,@LiveId)", freevideo)) > 0;
        }

        public static bool UpdateFreeVideo(FreeVideoModel freevideo)
        {
            return SystemConnection.OutFun<int>(conn => conn.Execute(@"UPDATE [DB_Live].[dbo].[FreeVideo]
                                                                       SET [VideoId] = @VideoId
                                                                     WHERE  [FreeId]=@FreeId and LiveId=@LiveId", freevideo)) > 0;
        }
    }
}
