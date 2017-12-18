using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Niugu.Common;
using Niu.Cabinet.Logging;

namespace Niu.Live.Chat.BusinessLogic
{
    public class SyncChatLiveDetail
    {
        //key:roomId, value{ key:id, value:fromAccId, sourceFromAccId, ext,auditFlag }
        public static Dictionary<long, SortedDictionary<long, Tuple<long, long, string, int>>> cacheDict = new Dictionary<long, SortedDictionary<long, Tuple<long, long, string, int>>>();

        //roomId,queueId
        public static Dictionary<long, SortedSet<long>> deleteCacheDict = new Dictionary<long, SortedSet<long>>();

        class LongSortDesc : IComparer<long> { public int Compare(long x, long y) { return x.CompareTo(y) * -1; } }


        public static Tuple<long, long, string, int> GetCacheChatLiveDetail(long roomId, long queueId)
        {
            if (cacheDict != null && cacheDict.ContainsKey(roomId) && cacheDict[roomId].ContainsKey(queueId))
            {
                return cacheDict[roomId][queueId];
            }
            else
            {
                return null;
            }
        }


        private static object _locker = new Object();
        public static long _prevLogID = 0;

        public static DateTime _prevHandleModifyLogTime = DateTime.Now;
        public static DateTime _lastHandleModifyLogTime = DateTime.Now;
        public static DateTime _nextHandleModifyLogTime = DateTime.Now;
        public static LogRecord _log = new LogRecord("logpath");
        public static void SyncChatLiveModifyLogHandler()
        {

            _log.WriteSingleLog("SyncChatLiveModifyLogHandler", "start");
            DateTime nowTime = DateTime.Now;
            if (nowTime > _nextHandleModifyLogTime)
            {
                lock (_locker)
                {
                    if (nowTime > _nextHandleModifyLogTime)
                    {
                        _prevHandleModifyLogTime = _lastHandleModifyLogTime;
                        _lastHandleModifyLogTime = nowTime;
                        _nextHandleModifyLogTime = nowTime.AddSeconds(3);

                        Task.Factory.StartNew(() =>
                        {
                            _log.WriteSingleLog("SyncChatLiveModifyLogHandler", "task:" + _prevLogID);

                            //logID,operate,id,roomId,fromAccId,sourceFromAccId,ext
                            List<dynamic> logDetail = Niu.Live.Chat.DataAccess.Chatroom.ChatroomMsgLogListByLogID(_prevLogID);

                            foreach (dynamic item in logDetail)
                            {
                                long logId = 0;
                                int operate = 0;
                                long queueId = 0;
                                long roomId = 0;
                                long fromAccId = 0;
                                long sourceFromAccId = 0;
                                int auditFlag = 0;
                                string ext = string.Empty;

                                try
                                {

                                    logId = Convert.ToInt64(item.logId);
                                    operate = Convert.ToInt32(item.operate);
                                    queueId = Convert.ToInt64(item.id);
                                    roomId = Convert.ToInt64(item.roomId);
                                    fromAccId = Convert.ToInt64(item.fromAccId);
                                    sourceFromAccId = Convert.ToInt64(item.sourceFromAccId);
                                    auditFlag = Convert.ToInt32(item.niuguAuditFlag);
                                    ext = item.ext;

                                    if (string.IsNullOrEmpty(ext) || logId == 0) continue;

                                    if (!cacheDict.ContainsKey(roomId)) cacheDict.Add(roomId, new SortedDictionary<long, Tuple<long, long, string, int>>());
                                    if (!deleteCacheDict.ContainsKey(roomId)) deleteCacheDict.Add(roomId, new SortedSet<long>(new LongSortDesc()));

                                    if (operate == 0)
                                    {
                                        //添加
                                        if (cacheDict[roomId].ContainsKey(queueId))
                                        {
                                            cacheDict[roomId][queueId] = Tuple.Create<long, long, string, int>(fromAccId, sourceFromAccId, ext, auditFlag);
                                        }
                                        else
                                        {
                                            cacheDict[roomId].Add(queueId, Tuple.Create<long, long, string, int>(fromAccId, sourceFromAccId, ext, auditFlag));
                                        }
                                    }
                                    if (operate == 1)
                                    {
                                        //删除
                                        if (cacheDict[roomId].ContainsKey(queueId))
                                        {
                                            cacheDict[roomId].Remove(queueId);
                                        }

                                        if (!deleteCacheDict[roomId].Contains(queueId))
                                        {
                                            deleteCacheDict[roomId].Add(queueId);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string error = ex.ToString();
                                    _log.WriteSingleLog("SyncChatLiveModifyLogHandler", error);
                                }

                                if (logId > _prevLogID) _prevLogID = logId;
                            }
                            _log.WriteSingleLog("SyncChatLiveModifyLogHandler", "task finish:" + _prevLogID);
                        });
                    }
                }
            }
            _log.WriteSingleLog("SyncChatLiveModifyLogHandler", "exit");
        }



        public static void InitChatLiveModifyLogHandler()
        {
            List<long> roomIdList = Niu.Live.Chat.DataAccess.Chatroom.ChatroomGetMasterAll();//取所有roomId;

            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = 4;//指定使用的硬件线程数为1

            Parallel.ForEach(roomIdList, options, roomId =>
            {

                try
                {
                    List<dynamic> detailList = Niu.Live.Chat.DataAccess.Chatroom.ChatroomMsgDetailLast(roomId);

                    SortedDictionary<long, Tuple<long, long, string, int>> sortedDetailList = new SortedDictionary<long, Tuple<long, long, string, int>>();

                    foreach (dynamic detail in detailList)
                    {
                        long fromAccId = 0;
                        long.TryParse(detail.fromAccId, out fromAccId);
                        long sourceFromAccId = 0;
                        long.TryParse(detail.sourceFromAccId, out sourceFromAccId);
                        int auditFlag = 0;
                        string ext = detail.ext;

                        if (string.IsNullOrEmpty(ext)) continue;

                        Tuple<long, long, string, int> t = Tuple.Create<long, long, string, int>(fromAccId, sourceFromAccId, ext, auditFlag);
                        sortedDetailList.Add(detail.id, t);
                    }

                    if (cacheDict.ContainsKey(roomId))
                    {
                        cacheDict[roomId] = sortedDetailList;
                    }
                    else
                    {
                        cacheDict.Add(roomId, sortedDetailList);
                    }

                    System.Threading.Thread.Sleep(1);

                }
                catch (Exception e)
                {
                    string error = e.Message.ToString();
                    _log.WriteSingleLog("SyncChatLiveDetial", roomId + ":" + error);
                }
            });


        }



        public static void SetChatLiveHistoryHandler(long roomId, List<Tuple<long, long, long, string, int>> list)
        {

            if (cacheDict != null)
            {
                if (!cacheDict.ContainsKey(roomId)) cacheDict.Add(roomId, new SortedDictionary<long, Tuple<long, long, string, int>>());

                //id fromAccId sourceFromAccId ext
                foreach (Tuple<long, long, long, string, int> t in list)
                {

                    //添加
                    if (cacheDict[roomId].ContainsKey(t.Item1))
                    {
                        cacheDict[roomId][t.Item1] = Tuple.Create<long, long, string, int>(t.Item2, t.Item3, t.Item4, t.Item5);
                    }
                    else
                    {
                        cacheDict[roomId].Add(t.Item1, Tuple.Create<long, long, string, int>(t.Item2, t.Item3, t.Item4, t.Item5));
                    }

                }
            }

        }


    }
}