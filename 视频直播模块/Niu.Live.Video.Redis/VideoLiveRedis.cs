using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Video.Redis
{
    public class VideoLiveRedis
    {
        public static string serverHost { get; set; }
        public static string slaveserverHost { get; set; }
        /// <summary>
        /// 读写RedisBase
        /// </summary>
        public static RedisBase liveRedisReadWrite { get; set; }
        /// <summary>
        /// 只读RedisBase
        /// </summary>
        private static RedisBase liveRedisReadOnly { get; set; }

        static VideoLiveRedis()
        {
            serverHost = "LiveRedisNiuguwang";

            RedisFactory.AddRedisConfig(serverHost, ServerItemType.AppSettings, "LiveRedisServerIP", "LiveRwRedisServerIP", "LiveRoRedisServerIP", "LiveRedisServerPort", "LiveRedisMaxWritePoolSize", "LiveRedisMaxReadPoolSize");

            liveRedisReadWrite = RedisFactory.GetRedisInstance(ServerPermissions.ReadWrite, serverHost, 7);
            liveRedisReadOnly = RedisFactory.GetRedisInstance(ServerPermissions.ReadOnly, serverHost, 7);
        }

        

        private static string LiveMonitor = "livemonitor";
        public static void AddLiveMonitor(string scid, string datetime)
        {
            using (IRedisClient rc = liveRedisReadWrite.GetClient())
            {
                rc.SetEntryInHash(LiveMonitor, scid, datetime);
            }
        }

        public static Dictionary<string, string> GetAllLiveMonitor()
        {
            using (IRedisClient rc = liveRedisReadWrite.GetClient())
            {
                return rc.GetAllEntriesFromHash(LiveMonitor);
            }
        }

        public static void RemoveAllLiveMonitor()
        {
            using (IRedisClient rc = liveRedisReadWrite.GetClient())
            {
                rc.Remove(LiveMonitor);
            }
        }

        public static void RemoveLiveMonitorByScid(string scid)
        {
            using (IRedisClient rc = liveRedisReadWrite.GetClient())
            {
                rc.RemoveEntryFromHash(LiveMonitor, scid);
            }
        }


      
    }
}
