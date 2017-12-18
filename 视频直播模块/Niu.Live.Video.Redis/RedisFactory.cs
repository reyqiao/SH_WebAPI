using Niu.Cabinet.Config;
using Niu.Cabinet.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Video.Redis
{
    public enum ServerItemType
    {
        AppSettings,
        Value
    }

    public enum ServerConfig
    {
        Default,
        ReadWrite,
        ReadOnly,
        Port,
        MaxWrite,
        MaxRead
    }

    public enum ServerPermissions
    {
        //当前机房
        Default,
        ReadWrite,
        ReadOnly
    }
    public class RedisFactory
    {
        private static Dictionary<string, RedisBase> DefaultInstanceList = new Dictionary<string, RedisBase>();
        private static Dictionary<string, RedisBase> ReadWriteInstanceList = new Dictionary<string, RedisBase>();
        private static Dictionary<string, RedisBase> ReadOnlyInstanceList = new Dictionary<string, RedisBase>();

        private static object DefaultLock = new object();
        private static object ReadWriteLock = new object();
        private static object ReadOnlyLock = new object();

        protected static Dictionary<string, Dictionary<ServerConfig, string>> RedisConfigCollections = new Dictionary<string, Dictionary<ServerConfig, string>>();

        public static RedisBase GetRedisInstance(ServerPermissions server, string serverHost, int db)
        {
            if (!RedisConfigCollections.ContainsKey(serverHost)) return null;

            string instance = string.Format("{0}:{1}:{2}", server.ToString(), serverHost, db);

            if (server == ServerPermissions.Default)
            {
                lock (DefaultLock)
                {
                    if (DefaultInstanceList.ContainsKey(instance))
                    {
                        return DefaultInstanceList[instance];
                    }
                    else
                    {
                        string host = RedisConfigCollections[serverHost][ServerConfig.Default].ToString();
                        int port = int.Parse(RedisConfigCollections[serverHost][ServerConfig.Port].ToString());
                        int maxReadPoolSize = int.Parse(RedisConfigCollections[serverHost][ServerConfig.MaxRead].ToString());
                        int maxWritePoolSize = int.Parse(RedisConfigCollections[serverHost][ServerConfig.MaxWrite].ToString());

                        RedisBase redis = new RedisBase(host, port, maxReadPoolSize, maxWritePoolSize, db);
                        DefaultInstanceList.Add(instance, redis);
                        return redis;
                    }
                }
            }
            if (server == ServerPermissions.ReadWrite)
            {
                lock (ReadWriteLock)
                {
                    if (ReadWriteInstanceList.ContainsKey(instance))
                    {
                        return ReadWriteInstanceList[instance];
                    }
                    else
                    {
                        string host = RedisConfigCollections[serverHost][ServerConfig.ReadWrite].ToString();
                        int port = int.Parse(RedisConfigCollections[serverHost][ServerConfig.Port].ToString());
                        int maxReadPoolSize = int.Parse(RedisConfigCollections[serverHost][ServerConfig.MaxRead].ToString());
                        int maxWritePoolSize = int.Parse(RedisConfigCollections[serverHost][ServerConfig.MaxWrite].ToString());

                        RedisBase redis = new RedisBase(host, port, maxReadPoolSize, maxWritePoolSize, db);
                        ReadWriteInstanceList.Add(instance, redis);
                        return redis;
                    }
                }
            }
            if (server == ServerPermissions.ReadOnly)
            {
                lock (ReadOnlyLock)
                {
                    if (ReadOnlyInstanceList.ContainsKey(instance))
                    {
                        return ReadOnlyInstanceList[instance];
                    }
                    else
                    {
                        string host = RedisConfigCollections[serverHost][ServerConfig.ReadOnly].ToString();
                        int port = int.Parse(RedisConfigCollections[serverHost][ServerConfig.Port].ToString());
                        int maxReadPoolSize = int.Parse(RedisConfigCollections[serverHost][ServerConfig.MaxRead].ToString());
                        int maxWritePoolSize = int.Parse(RedisConfigCollections[serverHost][ServerConfig.MaxWrite].ToString());

                        RedisBase redis = new RedisBase(host, port, maxReadPoolSize, maxWritePoolSize, db);
                        ReadOnlyInstanceList.Add(instance, redis);
                        return redis;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 写入配置在只读之前
        /// </summary>
        /// <param name="serverHost">用于获取RedisBase实例的别名</param>
        /// <param name="defaultHostKey">AppSettings Key 默认Redis服务器域名</param>
        /// <param name="writeHostKey">AppSettings Key 读写Redis服务器域名</param>
        /// <param name="readHostKey">AppSettings Key 只读Redis服务器域名</param>
        /// <param name="portKey">AppSettings Key Redis实例端口号</param>
        /// <param name="maxWritePoolSizeKey">AppSettings Key 写链接池数</param>
        /// <param name="maxReadPoolSizeKey">AppSettings Key 读链接池数</param>
        /// <returns></returns>
        public static bool AddRedisConfig(string serverHost, ServerItemType itemType, string defaultHostKey, string writeHostKey, string readHostKey, string portKey, string maxWritePoolSizeKey, string maxReadPoolSizeKey)
        {
            bool result = false;
            string defaultHost = string.Empty;
            string writeHost = string.Empty;
            string readHost = string.Empty;
            string port = string.Empty;
            string maxReadPoolSize = string.Empty;
            string maxWritePoolSize = string.Empty;

            if (itemType == ServerItemType.AppSettings)
            {
                defaultHost = AppSetting.AppSettingString(defaultHostKey);
                writeHost = AppSetting.AppSettingString(writeHostKey);
                readHost = AppSetting.AppSettingString(readHostKey);
                port = AppSetting.AppSettingString(portKey);
                maxWritePoolSize = AppSetting.AppSettingString(maxWritePoolSizeKey, "500");
                maxReadPoolSize = AppSetting.AppSettingString(maxReadPoolSizeKey, "1000");
            }
            if (itemType == ServerItemType.Value)
            {
                defaultHost = defaultHostKey;
                writeHost = writeHostKey;
                readHost = readHostKey;
                port = portKey;
                maxWritePoolSize = string.IsNullOrEmpty(maxWritePoolSizeKey) ? "500" : maxWritePoolSizeKey;
                maxReadPoolSize = string.IsNullOrEmpty(maxReadPoolSizeKey) ? "1000" : maxReadPoolSizeKey;
            }

            if (!string.IsNullOrEmpty(serverHost) && !string.IsNullOrEmpty(defaultHost) && !string.IsNullOrEmpty(writeHost) && !string.IsNullOrEmpty(readHost) && !string.IsNullOrEmpty(port))
            {
                if (!RedisConfigCollections.ContainsKey(serverHost))
                {
                    Dictionary<ServerConfig, string> redisConfig = new Dictionary<ServerConfig, string>();
                    redisConfig.Add(ServerConfig.Default, defaultHost);
                    redisConfig.Add(ServerConfig.ReadWrite, writeHost);
                    redisConfig.Add(ServerConfig.ReadOnly, readHost);
                    redisConfig.Add(ServerConfig.Port, port);
                    redisConfig.Add(ServerConfig.MaxWrite, maxWritePoolSize);
                    redisConfig.Add(ServerConfig.MaxRead, maxReadPoolSize);

                    RedisConfigCollections.Add(serverHost, redisConfig);
                    result = true;
                }
            }
            
            return result;
        }




        static RedisFactory()
        {



        }
    }
}
