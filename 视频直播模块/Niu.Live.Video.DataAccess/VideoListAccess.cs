using Niu.Live.Video.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Niu.Live.Video.DataAccess
{
    public class VideoListAccess
    {
        public static IEnumerable<VideoModel> ListByLiveId(long liveid, int index, int size)
        {
            return SystemConnection.OutFun<IEnumerable<VideoModel>>(conn =>
            {
                var para = new DynamicParameters();
                para.Add("@Index", index);
                para.Add("@Size", size);
                para.Add("@LiveId", liveid);

                return conn.Query<VideoModel>("LiveVideo_ListGetPage", para, commandType: CommandType.StoredProcedure);
            });
        }

        public static VideoModel GetVideo(long videoid)
        {
            return SystemConnection.OutFun<VideoModel>(conn => conn.QueryFirst<VideoModel>(@"SELECT  [VideoId]
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

        public static bool UpdateVideo(VideoModel video)
        {
            return SystemConnection.OutFun<int>(conn => conn.Execute(@"UPDATE [DB_Live].[dbo].[LiveVideo]
                                                                       SET [Title] =@Title
                                                                          ,[Description] = @Description
                                                                     WHERE VideoId=@VideoId", video)) > 0;
        }
        public static dynamic GetUser(long userid)
        {
            return SystemConnection.OutFun<dynamic>(conn => conn.QueryFirst<dynamic>(@"SELECT top 1  [ID]
                                  ,[UserId]
                                  ,[memberid]
                                  ,[mobile]
                                  ,[accesstoken]
                                  ,[refreshtoken]
                              FROM [dbo].[VideoLiveUser] "));
        }
        public static string AddVideo(Dictionary<string, string> video)
        {
            var table = "[dbo].[VideoLiveVideo]";
            var it = new StringBuilder();
            var iv = new StringBuilder();
            var parameters = new List<SqlParameter>();
            foreach (var item in video)
            {
                it.AppendFormat("[{0}],", item.Key);
                iv.AppendFormat("@{0},", item.Key);
                parameters.Add(new SqlParameter() { ParameterName = "@" + item.Key, Value = item.Value });
            }

            var sql = string.Format("INSERT INTO  {0} ({1})  VALUES({2}) ; select @@identity ;", table, it.ToString().TrimEnd(','), iv.ToString().TrimEnd(','));
            var para = parameters.ToArray();
            return SystemConnection.OutFun<string>(con => con.Execute(sql, para).ToString());

        }

        public static bool CloseVideo(string scid)
        {
            return SystemConnection.OutFun<bool>(conn => conn.Execute(string.Format("update [LiveVideo] set stoptime=GETDATE() where scid='{0}'", scid)) > 0);
        }


        public static bool AddAndUpdateUser(string userid, string mobile, string memberid)
        {
            var sql = string.Format(@"IF EXISTS(SELECT 1
                                              FROM   [VideoLiveUser]
                                              WHERE  UserId = {0}
                                                     AND memberid = '{2}')
                                      BEGIN
                                          UPDATE [VideoLiveUser]
                                          SET    memberid = '{2}',
                                                 mobile = '{1}'
                                          WHERE  UserId = {0}
                                      END
                                    ELSE
                                      BEGIN
                                          INSERT INTO [VideoLiveUser]
                                                      (memberid,
                                                       mobile,
                                                       UserId)
                                          VALUES     ('{2}',
                                                      '{1}',
                                                      {0})
                                      END 
                                    ", userid, mobile, memberid);
            return SystemConnection.OutFun<bool>(con => con.Execute(sql) > 0);
        }
        public static bool UpdateUser(long userid, Dictionary<string, string> dictionary)
        {
            var table = " VideoLiveUser ";
            var where = "UserId =" + userid;
            var set = new StringBuilder();
            foreach (var item in dictionary)
            {
                set.AppendFormat("[{0}]='{1}',", item.Key, item.Value);
            }
            return SystemConnection.OutFun<bool>(con => con.Execute(string.Format(" update {0} set {1} where {2}", table, set.ToString().TrimEnd(','), where)) > 0);
        }
        public static dynamic GetLiveLeftJoinLiveVideoByLiveId(int liveid)
        {
            var sql = string.Format(@"SELECT m.UserID,
       m.ID       liveId,
       v.ID       videoId,
       m.VideoChatRoom,
       v.scid     streamId,
       v.memberid liveChannel,
       m.[type],
       v.playCount,
       v.onlineCount,
       nc.announcement,
       CASE
         WHEN [LiveSwitch] = 1 THEN 1
         WHEN [IsVideo] = 1 THEN 2
         ELSE 0
       END        LiveType,
       CASE isvideo
         WHEN 1 THEN v.title
         ELSE m.LiveName
       END        showtitle,
       v.m3u8url,
       m.[BusinessType],
       vi.BgBzImg,
       (SELECT TOP 1 [UserID]
  FROM [dbo].[User_Role_Live] where LiveID=m.ID and DeleteSign=0 ) AssistantId
FROM   [dbo].[GraphicLiveMain] m
       LEFT JOIN [dbo].[VideoLiveVideo] v
              ON m.ID = v.mainId
                 AND v.stopTime IS NULL
       left join [dbo].[VideoImage] vi
              on m.UserID=vi.UserID
       left join [dbo].[NeteaseImChatroom] nc
              on nc.roomId=m.VideoChatRoom
WHERE  m.ID ={0}
ORDER  BY v.ID DESC ", liveid);
            return SystemConnection.OutFun<dynamic>(con => con.Query<dynamic>(sql).FirstOrDefault());
        }


        public static dynamic GetVideo(string scid)
        {
            return SystemConnection.OutFun<dynamic>(con => con.Query<dynamic>(string.Format("SELECT obscommand,stopTime  FROM [dbo].[VideoLiveVideo] where scid='{0}' ", scid)).FirstOrDefault());
        }

        public static bool UpdateReply(string arg1, DateTime arg2, DateTime arg3, string arg4)
        {
            return SystemConnection.OutFun<bool>(con =>
                con.Execute(string.Format("update [dbo].[LiveVideo] set replayurl='{0}',stopTime='{1}',startTime='{2}', obscommand='toClose'  where RStreamId='{3}' ", arg1, arg2, arg3, arg4))
                > 0);

        }
        public static dynamic GetLiveVideo(string scid)
        {

            //         m.ID       liveId,
            //v.ID       videoId,
            //m.VideoChatRoom,
            //v.scid     streamId,
            //v.memberid liveChannel,
            //m.[type],
            //v.playCount,
            //v.onlineCount,
            //nc.announcement,
            return SystemConnection.OutFun<dynamic>(con => con.Query<dynamic>(@" if exists( SELECT  top 1  * FROM [dbo].[LiveVideo] where LiveId=9072833 and stoptime is null order by VideoId desc)
                                 SELECT  top 1  Title,Description,HLSUrl,rtmpUrl FROM [dbo].[LiveVideo] where LiveId=9072833 and stoptime is null order by VideoId desc
                                 else
                                 SELECT  top 10 Title,Description, replayUrl as HLSUrl,rtmpUrl FROM [dbo].[LiveVideo] where LiveId=9072833 and replayUrl is not null  order by VideoId desc"));
        }

    }
}
