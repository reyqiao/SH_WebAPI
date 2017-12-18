using Niu.Live.Video.DataAccess;
using Niu.Live.Video.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Niu.Live.Video.BusinessLogic
{
    public static class LiveMonitor
    {

        public static bool IsRun = false;

        public static void AddLive(string videoid, DateTime datetime)
        {
            VideoLiveRedis.AddLiveMonitor(videoid, datetime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        private static object l = new object();
        public static void Start()
        {
            if (IsRun)
                return;

            lock (l)
            {
                Task.Factory.StartNew(() => { run(); });
                IsRun = true;
            }
        }


        private static List<string> stopscids = new List<string>();
        private static void run()
        {
            //LogRecord.writeLogsingle("LiveMonitor服务启动");
            try
            {
                while (true)
                {
                    stopscids.Clear();

                    var statsMap = VideoLiveRedis.GetAllLiveMonitor();
                    foreach (var live in statsMap)
                    {
                        var ticks = (DateTime.Now - DateTime.Parse(live.Value)).Seconds;
                        if (ticks > 30)//30秒
                        {
                            stopscids.Add(live.Key);
                            Task.Factory.StartNew(videoid =>
                            {//关闭视频
                                VideoLive.Instance.CloseLive(long.Parse(videoid.ToString()));
                            }, live.Key);
                        }
                    }
                    stopscids.ForEach(k => VideoLiveRedis.RemoveLiveMonitorByScid(k));

                    Thread.Sleep(30000);
                }
            }
            catch (Exception ex)
            {
                IsRun = false;
                //LogRecord.writeLogsingle(ex.Message);
            }
            //LogRecord.writeLogsingle("LiveMonitor服务退出");
        }
    }
}
