using Niu.Live.Video.Core.Utils;
using Niu.Live.Video.DataAccess;
using Niu.Live.Video.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.Video.Core.Live
{
    public class ZegoLive
    {
        private const string host = " https://webapi.zego.im/";
        private const string appid = "1366426152";
        private const string secret = "608c93604540ab164c28739b31095cd5";


        //  private const string appid = "434408520";
        //app secret: 0xda,0x54,0xde,0x4d,0x50,0x83,0x57,0x69,0x60,0x83,0xab,0x3d,0x39,0x86,0x76,0x49,0x93,0xba,0xe5,0x7e,0x27,0x48,0x01,0xd2,0x6a,0x4e,0xac,0x18,0xe6,0x63,0x17,0xfb
        //  private const string secret = "002e45d366b46b66e9a3a75912a57bbe";

        private string getToken()
        {
            var addr = host + "cgi/token";
            var para = new Dictionary<string, string>();
            para.Add("appid", appid);
            para.Add("secret", secret);

            var respdata = WebUtility.Get(addr, para);

            var data = JsonSerialize.Deserialize(respdata);
            if (data.code != 0)
                return getToken();

            return data.data.access_token;
        }


        public VideoModel CreateLive(long userid, string username = "公共直播", string title = "公共直播间", string desc = "公共直播间", long liveid = 0)
        {
            Niu.Cabinet.Logging.LogRecord log = new Cabinet.Logging.LogRecord(Niu.Cabinet.Config.AppSetting.AppSettingString("logpath"));
           
            var addr = host + "cgi/create-live?access_token=" + getToken();
            var para = new Dictionary<string, string>();
            para.Add("title", title);
            para.Add("id_name", userid.ToString());
            para.Add("nick_name", username);
            para.Add("term_type", "Windows");
            para.Add("net_type", "有线");

            var respdata = WebUtility.Post(addr, para);
            var jsondata = JsonSerialize.Deserialize(respdata);
            log.WriteSingleLog("video.txt", string.Format("创建直播调用第三方接口 ，{0}", jsondata));
            var code = jsondata.code;
            if (code != 0)
            {
                return null;
            }
            var live_channel = jsondata.data.live_channel;
            var live_id = jsondata.data.live_id;
            var stream_id = jsondata.data.stream_id;

            var hls_url = jsondata.data.play[0].hls_url[0];
            var hdl_url = jsondata.data.play[0].hdl_url[0];
            var publish_url = jsondata.data.publish_url;

            var video = new VideoModel();
            video.LiveId = liveid;
            video.UserId = userid;
            video.UserName = username;
            video.Title = title;
            video.Description = desc;
            video.StartTime = DateTime.Now;
            video.ObsCommand = "toRun";

            video.HDLUrl = hdl_url;
            video.RTMPUrl = publish_url;
            video.HLSUrl = hls_url;

            video.RLiveId = live_id;
            video.RLiveChannel = live_channel;
            video.RStreamId = stream_id;

            if (!LiveVideoAccess.CreateLive(video))
                return null;
            return video;


        }

        public bool CloseLive(string rstreamid, string obscommand)
        {
            var addr = host + "cgi/close-live?access_token=" + getToken();
            var para = new Dictionary<string, string>();
            para.Add("stream_id", rstreamid);
            var respdata = WebUtility.Post(addr, para);
            var jsondata = JsonSerialize.Deserialize(respdata);

            if (jsondata.code != 0)
            {
                return false;
                //LogRecord.writeLogsingle(string.Format("调用即构服务关闭失败，用户{0}，SCID{1}，数据{2}", userid, scid, respdata));
            }

            if (!LiveVideoAccess.CloseLive(rstreamid, obscommand))
                return false;

            return true;

        }

        public static readonly ZegoLive Instance = new ZegoLive();
    }
}
