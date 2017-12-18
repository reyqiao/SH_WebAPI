using System.Configuration;

namespace Niu.Live.Chat.Core
{
    public static class Config
    {
        public static readonly string DbDiscuss = GetConnectionString("DB_Live");

        private static string GetConnectionString(string name)
        {
            if (ConfigurationManager.ConnectionStrings[name] != null)
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            return null;
        }
    }
}