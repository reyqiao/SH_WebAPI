using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Video.Redis
{
    public class RedisBase
    {
        private string host = string.Empty;
        private int port = 0;
        private int maxReadPoolSize = 1000;
        private int maxWritePoolSize = 500;
        private int defaultDb;

        private PooledRedisClientManager prcm;

        public RedisBase(string h, int p, int maxR, int maxW, int db)
        {
            host = h;
            port = p;
            maxReadPoolSize = maxR;
            maxWritePoolSize = maxW;
            defaultDb = db;
            CreateManager();
        }

        /// <summary>
        /// 创建链接池管理对象
        /// </summary>
        internal void CreateManager()
        {
            prcm = CreateRedisManager(
               new string[] { host + ":" + port },   //读写服务器
               new string[] { host + ":" + port },    //只读服务器
               defaultDb
            );
        }

        /// <summary>
        /// 创建Redis连接池管理对象
        /// </summary>
        internal PooledRedisClientManager CreateRedisManager(string[] readWriteHosts, string[] readOnlyHosts, int defaultDb)
        {
            //支持读写分离，均衡负载
            return new PooledRedisClientManager(readWriteHosts, readOnlyHosts, new RedisClientManagerConfig
            {
                //“写”链接池数
                MaxWritePoolSize = maxWritePoolSize,

                //“读”链接池数
                MaxReadPoolSize = maxReadPoolSize,

                AutoStart = true,
                DefaultDb = defaultDb
            });
        }
        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        public IRedisClient GetClient()
        {
            if (prcm == null)
                CreateManager();

            return prcm.GetClient();
        }

        public IRedisClient GetReadOnlyClient()
        {
            if (prcm == null)
                CreateManager();

            return prcm.GetReadOnlyClient();
        }



    }
}
