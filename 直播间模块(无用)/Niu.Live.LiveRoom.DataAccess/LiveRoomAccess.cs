using Niu.Live.LiveRoom.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Niu.Live.LiveRoom.DataAccess
{
    public class LiveRoomAccess
    {
        public static LiveRoomModel GetLiveRoom(long liveid)
        {
            return SystemConnection.OutFun<LiveRoomModel>((conn) =>
             {
                 var sql = @"SELECT [LiveId]
                            ,[ChannelId]
                            ,[Title]
                            ,[KeyWord]
                            ,[Description]
                            ,[LogoUrl]
                            ,[OpenType],[Tel],[Pop],[PopUrl]
                        FROM [DB_Live].[dbo].[LiveRoom] where LiveId=@liveid";
                 return conn.QueryFirstOrDefault<LiveRoomModel>(sql, new { liveid = liveid });
             });
        }

        public static bool AddLiveRoom(LiveRoomModel liveroom)
        {
            return SystemConnection.OutFun((conn) =>
            {
                var sql = @"INSERT INTO [DB_Live].[dbo].[LiveRoom]
                               ([ChannelId]
                               ,[Title]
                               ,[KeyWord]
                               ,[Description]
                               ,[LogoUrl]
                               ,[OpenType],[Tel],[Pop],[PopUrl],[UserId],[AccId])
                         VALUES
                               (@channelid
                               ,@title
                               ,@keyword
                               ,@description
                               ,@logourl
                               ,@opentype,@Tel,@Pop,@PopUrl,@UserId,@AccId)";
                return conn.Execute(sql, new DynamicParameters(liveroom)) > 0;
            });

        }

        public static bool UpdateLiveRoom(LiveRoomModel liveroom)
        {
            return SystemConnection.OutFun((conn) =>
            {
                var sql = @"UPDATE [DB_Live].[dbo].[LiveRoom]
                               SET [ChannelId] = @channelid
                                  ,[Title] =@title
                                  ,[KeyWord] = @keyword
                                  ,[Description] = @description
                                  ,[LogoUrl] = @logourl
                                  ,[OpenType] = @opentype
                                  ,[Tel] = @Tel
                                  ,[Pop] = @Pop
                                  ,[PopUrl] = @PopUrl
                             WHERE LiveId=@liveid";
                return conn.Execute(sql, new { liveid = liveroom.LiveId, channelid = liveroom.ChannelId, title = liveroom.Title, keyword = liveroom.KeyWord, description = liveroom.Description, logourl = liveroom.LogoUrl, opentype = liveroom.OpenType }) > 0;
            });
        }
        #region room setting

        public static LiveRoomSetting FindOneSetting(long liveId)
        {
            return SystemConnection.OutFun<LiveRoomSetting>(con => con.Query<LiveRoomSetting>(@"SELECT   [Marking] ,[Audit] ,[Visitor] FROM [DB_Live].[dbo].[LiveRoomSetting] where [LiveRoomId]=@liveId", new { liveId }).FirstOrDefault());
        }
        public static bool InsertRoomSetting(LiveRoomSetting model)
        {
            return SystemConnection.OutFun<int>(con => con.Execute(@"
                    IF NOT EXISTS(SELECT * FROM [dbo].[LiveRoomSetting] WHERE [LiveRoomId]=@LiveRoomId)
                    BEGIN
                    INSERT INTO [DB_Live].[dbo].[LiveRoomSetting]
                               ([LiveRoomId]
                               ,[Marking]
                               ,[Audit]
                               ,[Visitor])
                                VALUES (@LiveRoomId,@Marking,@Audit,@Visitor)
                    END", new DynamicParameters(model))) > 0;
        }
        public static bool UpdateRoomSetting(LiveRoomSetting model)
        {
            return SystemConnection.OutFun<int>(con => con.Execute(@"UPDATE [DB_Live].[dbo].[LiveRoomSetting] SET 
                [Marking]=@Marking,[Audit]=@audit,[Visitor]=@Visitor Where [LiveRoomId]=@liveroomId", new DynamicParameters(model))) > 0;
        }
        #endregion
        #region 直播间admin
        public static bool AddAdmin(LiveRoomAdmin model)
        {

            return SystemConnection.OutFun<int>(con =>
             {
                 con.Open();
                 //con.BeginTransaction();
                 IDbTransaction ts = con.BeginTransaction();
                 try
                 {
                     var liveId = con.ExecuteScalar<long>(@" INSERT INTO [DB_Live].[dbo].[LiveRoom]
                               ([ChannelId] ) VALUES
                               (@ChannelId); SELECT @@IDENTITY", new DynamicParameters(model), ts);
                     model.LiveId = liveId;
                     var temp = con.Execute(@" INSERT INTO [DB_Live].[dbo].[LiveRoomAdmin]
                               ([LiveId]
                               ,[UserId]
                               ,[AdminType]
                               ,[AccId],[ChannelId]) VALUES(@LiveId,@UserId,@AdminType,@AccId,@ChannelId)", new DynamicParameters(model), ts);
                     ts.Commit();
                     return temp;
                 }
                 catch (Exception)
                 {
                     ts.Rollback();
                     return 0;
                 }
                 finally
                 {
                     con.Close();
                 }
             }) > 0;
        }
        public static bool IsAdmin(dynamic model)
        {
            return SystemConnection.OutFun<int>(con => con.Execute(@"
                            SELECT COUNT (1) FROM LIVEROOMADMIN WHERE ACCID=@ACCID AND LIVEID=@LIVEID", new DynamicParameters(model))) > 0;
        }
        public static IEnumerable<LiveRoomAdmin> GetRoomAdminList(long liveId)
        {
            return SystemConnection.OutFun<IEnumerable<LiveRoomAdmin>>(con => con.Query<LiveRoomAdmin>(@"
                    SELECT * FROM  DBO.LIVEROOMADMIN WHERE LIVEID=@LIVEID", new { liveId }));
        }
        #endregion

    }
}
