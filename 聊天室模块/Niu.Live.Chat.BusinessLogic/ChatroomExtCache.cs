using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Chat.BusinessLogic
{
    public class ChatroomExtCache
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="fromAccId"></param>
        /// <param name="sourceFromAccId"></param>
        /// <param name="id"></param>
        /// <param name="direction">1:取大于id  -1:小于id</param>
        /// <param name="size"></param>
        /// <param name="order">1:正序 -1:倒序</param>
        /// <returns></returns>
        public static List<string> ReplyMe(long roomId, string fromAccId, string sourceFromAccId, long id, int direction, int size = 20, int order = 1)
        {
            List<string> list = new List<string>();

            if (SyncChatLiveDetail.cacheDict != null && SyncChatLiveDetail.cacheDict.ContainsKey(roomId))
            {
                SortedDictionary<long, Tuple<long, long, string, int>> cacheDetailList = SyncChatLiveDetail.cacheDict[roomId];

                if (direction == 1)
                {
                    list = cacheDetailList.Where(t => t.Key > id && t.Value.Item1 == long.Parse(fromAccId) && t.Value.Item2 == long.Parse(sourceFromAccId)).Take(size).Select(t => t.Value.Item3).ToList();//取最新
                    if (order == -1)
                    {
                        list.Reverse();//SortedDictionary 默认正序 只有倒序再处理
                    }
                }
                else
                {
                    list = cacheDetailList.Where(t => t.Key < id && t.Value.Item1 == long.Parse(fromAccId) && t.Value.Item2 == long.Parse(sourceFromAccId)).OrderByDescending(t => t.Key).Take(size).Select(t => t.Value.Item3).ToList();//取历史
                    if (order == 1)
                    {
                        list.Reverse();//上一步被倒序 只有正序再处理
                    }
                }
            }

            if (direction == -1 && id > 0 && list.Count < 20)
            {
                //走数据库
                list = new List<string>();
                List<dynamic> dbList = Niu.Live.Chat.DataAccess.Chatroom.ChatroomMsgReplyMeDynamic(roomId, fromAccId, sourceFromAccId, id, direction, size, order);

                foreach (dynamic item in dbList)
                {
                    long item_queueId = Convert.ToInt64(item.id);
                    long item_fromAccId = Convert.ToInt64(item.fromAccId);
                    long item_sourceFromAccId = 0;
                    long.TryParse(item.sourceFromAccId, out item_sourceFromAccId);
                    string item_ext = item.ext;

                    list.Add(item_ext);
                }
            }

            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roomId"></param>
        /// <param name="fromAccId"></param>
        /// <param name="id"></param>
        /// <param name="direction">1:取大于id  -1:小于id</param>
        /// <param name="size"></param>
        /// <param name="order">1:正序 -1:倒序</param>
        /// <returns></returns>
        public static List<string> MasterSay(long userId, long roomId, string fromAccId, long id, int direction, int size = 20, int order = 1)
        {
            List<string> list = new List<string>();
            //不能为未登录用户
            if (userId <= 0) return list;

            if (SyncChatLiveDetail.cacheDict != null && SyncChatLiveDetail.cacheDict.ContainsKey(roomId))
            {
                SortedDictionary<long, Tuple<long, long, string, int>> cacheDetailList = SyncChatLiveDetail.cacheDict[roomId];

                if (direction == 1)
                {
                    list = cacheDetailList.Where(t => (t.Key > id && t.Value.Item1 == long.Parse(fromAccId)) || (t.Value.Item4 == -1 && t.Value.Item1 == userId)).Take(size).Select(t => t.Value.Item3).ToList();//取最新
                    if (order == -1)
                    {
                        list.Reverse();//SortedDictionary 默认正序 只有倒序再处理
                    }
                }
                else
                {
                    list = cacheDetailList.Where(t => t.Key < id && t.Value.Item1 == long.Parse(fromAccId)).OrderByDescending(t => t.Key).Take(size).Select(t => t.Value.Item3).ToList();//取历史
                    if (order == 1)
                    {
                        list.Reverse();//上一步被倒序 只有正序再处理
                    }
                }
            }

            if (direction == -1 && id > 0 && list.Count < 20)
            {
                //走数据库
                list = new List<string>();

                List<dynamic> dbList = Niu.Live.Chat.DataAccess.Chatroom.ChatroomMsgMasterDynamic(roomId, fromAccId, id, direction, size, order);

                foreach (dynamic item in dbList)
                {
                    long item_queueId = Convert.ToInt64(item.id);
                    long item_fromAccId = Convert.ToInt64(item.fromAccId);
                    long item_sourceFromAccId = 0;
                    long.TryParse(item.sourceFromAccId, out item_sourceFromAccId);

                    string item_ext = item.ext;

                    list.Add(item_ext);
                }

            }

            return list;
        }

        public static List<string> MasterSayH5(long roomId, long id, string fromAccId, int direction, int size = 20, int order = 1)
        {
            List<string> list = new List<string>();
            //不能为未登录用户
            //if (userId <= 0) return list;

            if (SyncChatLiveDetail.cacheDict != null && SyncChatLiveDetail.cacheDict.ContainsKey(roomId))
            {
                SortedDictionary<long, Tuple<long, long, string, int>> cacheDetailList = SyncChatLiveDetail.cacheDict[roomId];

                if (direction == 1)
                {
                    list = cacheDetailList.Where(t => (t.Key > id && t.Value.Item1 == long.Parse(fromAccId) && t.Value.Item4 != -1)).Take(size).Select(t => t.Value.Item3).ToList();//取最新
                    if (order == -1)
                    {
                        list.Reverse();//SortedDictionary 默认正序 只有倒序再处理
                    }
                }
                else
                {
                    list = cacheDetailList.Where(t => t.Key < id && t.Value.Item1 == long.Parse(fromAccId) && t.Value.Item4 != -1).OrderByDescending(t => t.Key).Take(size).Select(t => t.Value.Item3).ToList();//取历史
                    if (order == 1)
                    {
                        list.Reverse();//上一步被倒序 只有正序再处理
                    }
                }
            }

            if (direction == -1 && id > 0 && list.Count < 20)
            {
                //走数据库
                list = new List<string>();

                List<dynamic> dbList = Niu.Live.Chat.DataAccess.Chatroom.ChatroomMsgMasterDynamic(roomId, fromAccId, id, direction, size, order);

                foreach (dynamic item in dbList)
                {
                    long item_queueId = Convert.ToInt64(item.id);
                    long item_fromAccId = Convert.ToInt64(item.fromAccId);
                    long item_sourceFromAccId = 0;
                    long.TryParse(item.sourceFromAccId, out item_sourceFromAccId);

                    string item_ext = item.ext;

                    list.Add(item_ext);
                }

            }

            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="id"></param>
        /// <param name="direction">1:取大于id  -1:小于id</param>
        /// <param name="size"></param>
        /// <param name="order">1:正序 -1:倒序</param>
        /// <returns></returns>
        public static List<string> Detail(long userId, long roomId, long id, int direction, int size = 20, int order = 1, int ismanger = 0)
        {
            List<string> list = new List<string>();
            //不能为未登录用户
            if (userId <= 0) return list;

            if (SyncChatLiveDetail.cacheDict != null && SyncChatLiveDetail.cacheDict.ContainsKey(roomId))
            {
                SortedDictionary<long, Tuple<long, long, string, int>> cacheDetailList = SyncChatLiveDetail.cacheDict[roomId];
                if (direction == 1)
                {
                    list = cacheDetailList.Where(t => t.Key > id && (t.Value.Item4 == 0 || (t.Value.Item4 == -1 && t.Value.Item1 == userId))).Take(size).Select(t => t.Value.Item3).ToList();//取最新
                    if (order == -1)
                    {
                        list.Reverse();//SortedDictionary 默认正序 只有倒序再处理
                    }
                }
                else
                {
                    list = cacheDetailList.Where(t => t.Key < id && (t.Value.Item4 == 0 || (t.Value.Item4 == -1 && t.Value.Item1 == userId))).OrderByDescending(t => t.Key).Take(size).Select(t => t.Value.Item3).ToList();//取历史
                    if (order == 1)
                    {
                        list.Reverse();//上一步被倒序 只有正序再处理
                    }
                }
            }

            if (list.Count < 20)
            {
                //走数据库
                list = new List<string>();

                List<dynamic> dbList = Niu.Live.Chat.DataAccess.Chatroom.ChatroomMsgDetailDynamic(roomId, id, direction, size, order, ismanger);
                List<Tuple<long, long, long, string, int>> toCacheList = new List<Tuple<long, long, long, string, int>>();

                foreach (dynamic item in dbList)
                {
                    //long item_queueId = Convert.ToInt64(item.id);
                    //string item_fromAccId = item.fromAccId;
                    //string item_sourceFromAccId = item.sourceFromAccId;
                    //int item_auditFlag = item.niuguAuditFlag;
                    var t1 = item.sendtime;
                    string item_ext = item.ext;
                    item_ext = item_ext.Insert(1, string.Format("\"id\":{0},\"name\":\"{1}\",\"userlogo\":\"{2}\",\"addtime\":\"{3}\",\"ismanger\":\"{4}\",", item.id, item.name, item.userlog, ConvertDataTimeLong(t1), item.IsManger));
                    list.Add(item_ext);

                    //toCacheList.Add(Tuple.Create<long, long, long, string, int>(item_queueId, item_fromAccId, item_sourceFromAccId, item_ext, item_auditFlag));
                }

                //   SyncChatLiveDetail.SetChatLiveHistoryHandler(roomId, toCacheList);
            }

            return list;
        }
        public static long ConvertDataTimeLong(DateTime dt)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = dt.Subtract(dtStart);
            long timeStamp = toNow.Ticks;
            timeStamp = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
            return timeStamp;
        }

        public static List<string> DetailH5(long roomId, long id, int direction, int size = 20, int order = 1)
        {
            List<string> list = new List<string>();
            //不能为未登录用户
            //if (userId <= 0) return list;

            if (SyncChatLiveDetail.cacheDict != null && SyncChatLiveDetail.cacheDict.ContainsKey(roomId))
            {
                SortedDictionary<long, Tuple<long, long, string, int>> cacheDetailList = SyncChatLiveDetail.cacheDict[roomId];
                if (direction == 1)
                {
                    list = cacheDetailList.Where(t => t.Key > id && t.Value.Item4 == 0).Take(size).Select(t => t.Value.Item3).ToList();//取最新
                    if (order == -1)
                    {
                        list.Reverse();//SortedDictionary 默认正序 只有倒序再处理
                    }
                }
                else
                {
                    list = cacheDetailList.Where(t => t.Key < id && t.Value.Item4 == 0).OrderByDescending(t => t.Key).Take(size).Select(t => t.Value.Item3).ToList();//取历史
                    if (order == 1)
                    {
                        list.Reverse();//上一步被倒序 只有正序再处理
                    }
                }
            }

            if (direction == -1 && id > 0 && list.Count < 20)
            {
                //走数据库
                list = new List<string>();

                List<dynamic> dbList = Niu.Live.Chat.DataAccess.Chatroom.ChatroomMsgDetailDynamic(roomId, id, direction, size, order);
                List<Tuple<long, long, long, string, int>> toCacheList = new List<Tuple<long, long, long, string, int>>();

                foreach (dynamic item in dbList)
                {
                    long item_queueId = Convert.ToInt64(item.id);
                    long item_fromAccId = Convert.ToInt64(item.fromAccId);
                    long item_sourceFromAccId = 0;
                    long.TryParse(item.sourceFromAccId, out item_sourceFromAccId);
                    int item_auditFlag = 0;
                    int.TryParse(item.niuguAuditFlag.ToString(), out item_auditFlag);
                    string item_ext = item.ext;

                    list.Add(item_ext);

                    toCacheList.Add(Tuple.Create<long, long, long, string, int>(item_queueId, item_fromAccId, item_sourceFromAccId, item_ext, item_auditFlag));
                }

                SyncChatLiveDetail.SetChatLiveHistoryHandler(roomId, toCacheList);
            }

            return list;
        }

    }
}