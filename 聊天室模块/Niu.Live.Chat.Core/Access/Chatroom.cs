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
    public class Chatroom
    {
        static string DB_LiveConnection = ConnectionString.Get("DB_Live", String.Empty);



        public static Im_Chatroom_Msg ChatroomGetMsg(string msgId)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = "SELECT * FROM [dbo].[NeteaseImMsgQueue] WHERE msgId=@msgId";
                return conn.Query<Im_Chatroom_Msg>(sql, new { msgId = msgId }).SingleOrDefault();
            }
        }


        public static int ChatroomInsert(long roomId, string creator, string name, bool valid, string announcement, string broadcastUrl, string ext)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                Im_Chatroom chatroom = new Im_Chatroom() { roomid = roomId, creator = creator, name = name, valid = valid, announcement = announcement, broadcasturl = broadcastUrl, ext = ext };
                string sql = "INSERT INTO [dbo].[NeteaseImChatroom]([roomId],[creator],[name],[valid],[announcement],[broadcastUrl],[ext])VALUES(@roomId,@creator,@name,@valid,@announcement,@broadcasturl,@ext)";
                return conn.Execute(sql, chatroom);
            }
        }



        public static int ChatroomUpdate(long roomId, string name, string announcement, string broadcastUrl, string ext)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = "UPDATE [dbo].[NeteaseImChatroom] SET [name] = @name, [announcement] = @announcement, [broadcastUrl] = @broadcastUrl, [ext] = @ext WHERE [roomId]=@roomId";
                return conn.Execute(sql, new { roomId = roomId, name = name, announcement = announcement, broadcastUrl = broadcastUrl, ext = ext });
            }
        }



        public static int ChatroomToggleCloseStat(long roomId, string operatorAccId, string valid, long niuguAdmin)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = @"UPDATE [dbo].[NeteaseImChatroom] SET [valid] = @valid WHERE [roomId]=@roomid
                              INSERT INTO [dbo].[NeteaseImChatroomStatLog]([roomId],[operator],[valid],[niuguAdmin])VALUES(@roomId,@operatorAccId,@valid,@niuguAdmin)
                              ";
                return conn.Execute(sql, new { roomId = roomId, operatorAccId = operatorAccId, valid = valid, niuguAdmin = niuguAdmin });
            }
        }

        public static Im_Chatroom ChatroomGet(long roomId)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = "SELECT *  FROM [dbo].[NeteaseImChatroom] where roomId=@roomId";
                return conn.Query<Im_Chatroom>(sql, new { roomId = roomId }).SingleOrDefault();
            }
        }

        public static Im_Chatroom ChatroomGetByAccId(string accId)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = "SELECT * FROM [dbo].[NeteaseImChatroom] where creator=@accId";
                return conn.Query<Im_Chatroom>(sql, new { accId = accId }).SingleOrDefault();
            }
        }


        public static int ChatroomSetMemberRole(long roomId, string operatorAccId, string target, string opt, string optvalue, string notifyExt = "", long niuguAdmin = 0)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = @"IF EXISTS(SELECT * FROM [dbo].[NeteaseImChatroomRole] WHERE [roomId]=@roomid AND [target]=@target)
                               BEGIN
                                    
                                    UPDATE [dbo].[NeteaseImChatroomRole] SET opt=@opt, optvalue=@optvalue WHERE [roomId]=@roomid AND [target]=@target

                               END
                               ELSE
                               BEGIN

                                    INSERT INTO [dbo].[NeteaseImChatroomRole]([roomId],[operator],[target],[opt],[optValue])VALUES(@roomId,@operatorAccId,@target,@opt,@optValue)

                               END
                               INSERT INTO [dbo].[NeteaseImChatroomRoleLog]([roomId],[operator],[target],[opt],[optvalue],[notifyExt],[niuguAdmin])VALUES(@roomId,@operatorAccId,@target,@opt,@optvalue,@notifyExt,@niuguAdmin)
                               ";
                return conn.Execute(sql, new { roomId = roomId, operatorAccId = operatorAccId, target = target, opt = opt, optvalue = optvalue, notifyExt = notifyExt, niuguAdmin = niuguAdmin });
            }
        }



        public static int ChatroomTemporaryMute(long roomId, string operatorAccId, string target, long muteDuration, string needNotify, string notifyExt = "", long niuguAdmin = 0)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = @"INSERT INTO [dbo].[NeteaseImChatroomMuteUserLog]([roomId],[operator],[target],[muteDuration],[needNotify],[notifyExt],[niuguAdmin])VALUES(@roomId,@operatorAccId,@target,@muteDuration,@needNotify,@notifyExt,@niuguAdmin)";
                return conn.Execute(sql, new { roomId = roomId, operatorAccId = operatorAccId, target = target, muteDuration = muteDuration, needNotify = needNotify, notifyExt = notifyExt, niuguAdmin = niuguAdmin });
            }
        }
        /// <summary>
        /// 将聊天室整体禁言
        /// </summary>
        /// <returns></returns>
        public static int ChatroomMute(long roomId, string operatorAccId, string mute = "false", string needNotify = "", string notifyExt = "")
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = @"INSERT INTO [dbo].[NeteaseImChatroomMuteLog]([roomId],[operator],[mute],[needNotify],[notifyExt],[CreateTime])VALUES(@roomId,@operatorAccId,@mute,@needNotify,@notifyExt,getdate())";
                return conn.Execute(sql, new { roomId = roomId, operatorAccId = operatorAccId, mute = mute, needNotify = needNotify, notifyExt = notifyExt });
            }
        }
        /// <summary>
        /// 加入黑名单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int BlockUser(dynamic model)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                var pars = new DynamicParameters(model);
                string sql = @"INSERT INTO [DB_Live].[dbo].[NeteaseImUserBlock]([RoomId],[AccId],[Target],[TargetName],[AddTime]) VALUES (@RoomId,@AccId,@Target,@TargetName,GETDATE()) ";
                return conn.Execute(sql, pars);
            }
        }
        /// <summary>
        /// 查询一个是否在黑名单中
        /// </summary>
        /// <param name="model">{roomid="",target=""}</param>
        /// <returns></returns>
        public static bool QueryBlock(dynamic model)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                var pars = new DynamicParameters(model);
                string sql = @" SELECT COUNT(*) FROM [dbo].[NeteaseImUserBlock] WHERE [roomId]=@roomId AND [target]=@target ";
                return conn.Query<int>(sql, pars).FirstOrDefault() > 0 ? true : false;
            }
        }
        /// <summary>
        /// 取黑名单列表
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> QueryBlockList(long roomId)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = @" SELECT roomId=9072833, Target as Id,TargetName UserName FROM [dbo].[NeteaseImUserBlock] WHERE [roomId]=@roomid  ";
                return conn.Query<dynamic>(sql, new { roomId });
            }
        }
        /// <summary>
        /// 取当前房间禁言列表
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public static IEnumerable<string> QueryMuteList(long roomId)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = @" SELECT Target FROM [dbo].[NeteaseImChatroomMuteUserLog] WHERE [roomId]=@roomid and DATEADD(S,muteDuration, niuguOperatorTime )<GETDATE() ";
                return conn.Query<string>(sql, new { roomId });
            }
        }
        /// <summary>
        /// 加入禁言
        /// </summary>
        /// <returns></returns>
        public static bool AddMuteUser(dynamic model)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                var pars = new DynamicParameters(model);
                string sql = @"INSERT INTO [DB_Live].[dbo].[NeteaseImChatroomMuteUserLog]([RoomId],[operator],[Target]) VALUES (9072833,@AccId,@Target) ";
                return conn.Execute(sql, pars) > 0;
            }
        }
        /// <summary>
        /// 移除黑名单
        /// </summary>
        /// <param name="model">参数为{roomid="",target=""}</param>
        /// <returns></returns>
        public static int RemoveBlockUser(dynamic model)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                var pars = new DynamicParameters(model);
                string sql = @"IF EXISTS(SELECT * FROM [dbo].[NeteaseImUserBlock] WHERE [roomId]=@roomid AND [target]=@target)
                    BEGIN
                            DELETE FROM [DB_LIVE].[DBO].[NeteaseImUserBlock] WHERE [roomId]=@roomid AND [target]=@target
                    END";
                return conn.Execute(sql, pars);
            }
        }

        public static int ChatroomUpdateColumn(long roomId, dynamic parameters)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                List<string> list = new List<string>();

                var dict = (IDictionary<string, object>)parameters;

                foreach (KeyValuePair<string, object> kv in dict)
                {
                    list.Add(string.Format("[{0}] = @{0}", kv.Key));
                }

                dict["roomId"] = roomId;

                string sql = string.Format("UPDATE [dbo].[NeteaseImChatroom] SET {0} WHERE accid=@accid", string.Join(",", list));
                return conn.Execute(sql, (object)parameters);
            }
        }
        public static long ChatroomSendMsg(string msgId, int msgType, long roomId, string fromAccId, int resendFlag, string attach, string ext, int niuguAuditFlag = 0, int IsMager = 0, int roleId = 0, long sourceId = 0, string sourceMsgId = "", string sourceFromAccId = "", bool niuguLiveMaster = true, int niuguRoomType = 0, string niuguUserIp = "")
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = "NeteaseImMsgQueue_SendMsg";

                var p = new DynamicParameters();

                p.Add("@msgId", msgId, dbType: DbType.AnsiStringFixedLength, size: 32);
                p.Add("@msgType", msgType, dbType: DbType.Int32);
                p.Add("@roomId", roomId, dbType: DbType.Int64);
                p.Add("@fromAccId", fromAccId, dbType: DbType.AnsiString, size: 50);
                p.Add("@resendFlag", resendFlag, dbType: DbType.Int32);
                p.Add("@attach", attach, dbType: DbType.String, size: 2048);
                p.Add("@ext", ext, dbType: DbType.String, size: 4000);
                p.Add("@sourceId", sourceId, dbType: DbType.Int64);
                p.Add("@sourceMsgId", sourceMsgId, dbType: DbType.AnsiStringFixedLength, size: 32);
                p.Add("@sourceFromAccId", sourceFromAccId, dbType: DbType.AnsiString, size: 50);
                p.Add("@niuguLiveMaster", niuguLiveMaster, dbType: DbType.Boolean);
                p.Add("@niuguRoomType", niuguRoomType, dbType: DbType.Int32);
                p.Add("@niuguAuditFlag", niuguAuditFlag, dbType: DbType.Int32);
                p.Add("@niuguSendTime", DateTime.Now, dbType: DbType.DateTime);
                p.Add("@niuguUserIp", niuguUserIp, dbType: DbType.AnsiString, size: 20);
                p.Add("@IsManger", IsMager, dbType: DbType.Int32);
                p.Add("@RoleId", roleId, dbType: DbType.Int32);
                p.Add("@id", dbType: DbType.Int64, direction: ParameterDirection.Output);
                conn.Execute(sql, p, commandType: CommandType.StoredProcedure);

                return p.Get<long>("@id");
            }
        }
        /// <summary>
        /// 消息列表
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="audit"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> GetMsgList(int index = 1, int pageSize = 20, int audit = 0)
        {
            using (IDbConnection conn = new SqlConnection(DB_LiveConnection))
            {
                string sql = "NeteaseImMsgQueue_GetList";

                var p = new DynamicParameters();
                p.Add("@Index", index, dbType: DbType.Int32);
                p.Add("@Size", pageSize, dbType: DbType.Int32);
                p.Add("@audit", audit, dbType: DbType.Int32);
                return conn.Query<dynamic>(sql, p, commandType: CommandType.StoredProcedure);
            }
        }

    }
}