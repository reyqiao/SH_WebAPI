using Niu.Live.User.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.User.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Niu.Live.User.IModel.TokenManager.TokenUserInfo userInfo = new IModel.TokenManager.TokenUserInfo();
            userInfo.userId = 1;
            userInfo.tokenType = Niu.Live.User.IModel.TokenManager.tokenType.Formal;
            userInfo.nickName = "昵称";
            userInfo.type = Niu.Live.User.IModel.TokenManager.userType.pc;
            userInfo.status = 1;
            userInfo.channelId = 1;

            string userToken = string.Empty;

            //bool isSuccess = Niu.Live.User.Provider.TokenManager.TokenManager.GenerateUserToken(userInfo, out userToken);

            userToken = "pm07UI4sm-YSKdCBOTaejLI1eLeWDTr_4ORr9WcqGPSsbq04yI3su8aW-HAQoRk5";

            Niu.Live.User.Model.TokenUserInfo user = null;

            Niu.Live.User.Core.TokenManager.ValidateMobileUserToken(userToken, out user);

            Niu.Live.User.IModel.TokenManager.TokenUserInfo m = null;
            //Niu.Live.User.Provider.TokenManager.TokenManager.ValidateUserToken(userToken, out m);

            Console.ReadLine();
        }
    }
}
