using Niu.Live.Video.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace Niu.Live.Video.DataAccess
{
    public class LiveVideoAccess
    {

        public static bool CreateLive(VideoModel video)
        {
            var videoid = SystemConnection.OutFun<long>(conn =>
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();
                long id = 0;
                try
                {
                    id = conn.ExecuteScalar<long>(@"INSERT INTO [DB_Live].[dbo].[LiveVideo]
                                                           ([LiveId]
                                                           ,[UserId]
                                                           ,[UserName]
                                                           ,[Title]
                                                           ,[Description]
                                                           ,[HDLUrl]
                                                           ,[RTMPUrl]
                                                           ,[HLSUrl]
                                                           ,[StartTime]
                                                           ,[ObsCommand]
                                                           ,[RLiveId]
                                                           ,[RLiveChannel]
                                                           ,[RStreamId])
                                                     VALUES
                                                           (@LiveId
                                                           ,@UserId
                                                           ,@UserName
                                                           ,@Title
                                                           ,@Description
                                                           ,@HDLUrl
                                                           ,@RTMPUrl
                                                           ,@HLSUrl
                                                           ,@StartTime
                                                           ,@ObsCommand
                                                           ,@RLiveId
                                                           ,@RLiveChannel
                                                           ,@RStreamId); select @@identity ;", video, transaction);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }

                return id;
            });
            if (videoid > 0)
            {
                video.VideoId = videoid;
                return true;
            }

            return false;
        }

        public static bool CloseLive(string rstreamid, string obscommand)
        {
            return SystemConnection.OutFun<long>(conn => conn.Execute(@"UPDATE [DB_Live].[dbo].[LiveVideo]
                                                                           SET [StopTime] = GETDATE()
                                                                              ,[ObsCommand] = @ObsCommand
                                                                         WHERE RStreamId=@RStreamId", new { ObsCommand = obscommand, RStreamId = rstreamid })) > 0; ;
        }

        public static VideoModel GetVideoById(long videoid)
        {
            return SystemConnection.OutFun<VideoModel>(conn => conn.QueryFirst(@"SELECT [VideoId]
                                                                                  ,[LiveId]
                                                                                  ,[UserId]
                                                                                  ,[UserName]
                                                                                  ,[Title]
                                                                                  ,[Description]
                                                                                  ,[HDLUrl]
                                                                                  ,[RTMPUrl]
                                                                                  ,[HLSUrl]
                                                                                  ,[ReplayUrl]
                                                                                  ,[DeleteSign]
                                                                                  ,[Cover]
                                                                                  ,[StartTime]
                                                                                  ,[StopTime]
                                                                                  ,[ReplayTime]
                                                                                  ,[Weight]
                                                                                  ,[ChatRoomId]
                                                                                  ,[ObsCommand]
                                                                                  ,[OnlineCount]
                                                                                  ,[PlayCount]
                                                                                  ,[RLiveId]
                                                                                  ,[RLiveChannel]
                                                                                  ,[RStreamId]
                                                                              FROM [DB_Live].[dbo].[LiveVideo] where VideoId=@videoid", videoid));
        }

    }
}
