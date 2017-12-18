using Niu.Live.Video.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Niu.Live.Video.DataAccess
{
    public class LiveRoomAccess
    {
        public static LiveRoomModel GetLiveRoomById(long liveid)
        {
            return SystemConnection.OutFun<LiveRoomModel>(conn => conn.QueryFirst<LiveRoomModel>(@"SELECT [LiveId]
                                                                          ,[ChannelId]
                                                                          ,[Title]
                                                                          ,[KeyWord]
                                                                          ,[Description]
                                                                          ,[LogoUrl]
                                                                          ,[Audit]
                                                                          ,[Visitor]
                                                                          ,[OpenType]
                                                                          ,[ChatRoomId]
                                                                          ,[State]
                                                                          ,[ChatTitle]
                                                                      FROM [DB_Live].[dbo].[LiveRoom] where LiveId=@liveid", new { liveid = liveid }));

        }

        public static bool ChangeLiveRoomState(long liveid, int state)
        {
            return SystemConnection.OutFun<int>(conn => conn.Execute(@"UPDATE [DB_Live].[dbo].[LiveRoom]
                                                                           SET [State] = @state
                                                                         WHERE LiveId=@liveid", new { liveid = liveid, state = state })) > 0;
        }
    }
}
