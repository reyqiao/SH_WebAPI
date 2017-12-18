using Niu.Cabinet.Cryptography;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.LiveRoom.Utils.Photo
{
    public class PhotoConfig
    {
        public static readonly int MaxFilesNumber = 200;
        public static readonly string PhotoExtension = ".jpeg.jpg.gif.png";
        public static readonly DateTime SegmentationTime = new DateTime(2020, 9, 1, 0, 0, 0);
        public string urlRootFolder = string.Empty;
        public string saveRootFolder = string.Empty;
        public string photoDomain = string.Empty;
        public EnumManager.PhotoType type = EnumManager.PhotoType.None;
        public DateTime addTime = default(DateTime);
        public long photoID = 0;
        public string extension = string.Empty;
        public PhotoConfig(EnumManager.PhotoType configtype, long configphotoid, string configextension, DateTime configdate = default(DateTime))
        {
            type = configtype;
            addTime = configdate;
            photoID = configphotoid;
            extension = configextension;
            Cons_InitPhotoConfig();
        }

        #region   构造方法
        public void Cons_InitPhotoConfig()
        {
            if (type == EnumManager.PhotoType.None)
            {
                urlRootFolder = string.Empty;
                saveRootFolder = string.Empty;
                photoDomain = string.Empty;
                return;
            }

            if (addTime == default(DateTime)) addTime = DateTime.Now;


            saveRootFolder = ConfigurationManager.AppSettings["NewSaveRootFolder"];
            urlRootFolder = ConfigurationManager.AppSettings["NewRootFolder"];
            photoDomain = ConfigurationManager.AppSettings["NewPhotoDomain"];


        }
        private string Cons_GetSlices()
        {
            string slices = string.Empty;
            if (addTime == default(DateTime)) addTime = DateTime.Now;
            if (addTime.Ticks < SegmentationTime.Ticks)
            {
                if (type == EnumManager.PhotoType.PagePhoto)
                {
                    slices = (photoID % 10).ToString();
                }
                else
                {
                    slices = (photoID / MaxFilesNumber).ToString();
                }
            }
            else
            {
                slices = (photoID / MaxFilesNumber).ToString();
            }
            return slices;
        }
        //Url地址中的域名+目录+文件名
        public string Cons_GetUrl()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("https://");
            sb.Append(photoDomain + "/");
            if (urlRootFolder != "")
            {
                sb.Append(urlRootFolder + "/");
            }
            if (addTime.Ticks < SegmentationTime.Ticks && EnumManager.PhotoType.PagePhoto == type)
            {
                sb.Append(addTime.Year.ToString("0000") + "/" + addTime.Month.ToString("00") + "/");
            }
            else
            {
                sb.Append(addTime.Year.ToString("0000") + "/" + addTime.Month.ToString("00") + addTime.Day.ToString("00") + "/");
            }
            sb.Append(Cons_GetSlices() + "/");
            sb.Append(Cons_GetFileName());
            return sb.ToString();
        }


        //保存路径的目录+文件名
        public string Cons_GetFolderFilePath()
        {
            return System.IO.Path.Combine(Cons_GetFolder(), Cons_GetFileName());
        }


        //保存路径的目录
        public string Cons_GetFolder()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(saveRootFolder);
            if (addTime.Ticks < SegmentationTime.Ticks && EnumManager.PhotoType.PagePhoto == type)
            {
                sb.Append("\\" + addTime.Year.ToString("0000") + "\\" + addTime.Month.ToString("00") + "\\");
            }
            else
            {
                sb.Append("\\" + addTime.Year.ToString("0000") + "\\" + addTime.Month.ToString("00") + addTime.Day.ToString("00") + "\\");
            }
            sb.Append(Cons_GetSlices() + "\\");
            return sb.ToString();
        }


        //文件名
        public string Cons_GetFileName()
        {
            string prefix = string.Empty;
            if (addTime == default(DateTime)) addTime = DateTime.Now;
            if (addTime.Ticks < SegmentationTime.Ticks)
            {
                if (EnumManager.PhotoType.PagePhoto == type) prefix = "pp_";
                else prefix = "n_";
            }
            else
            {
                if (EnumManager.PhotoType.SitLogo == type) prefix = "sl_";
                else if (EnumManager.PhotoType.PagePhoto == type) prefix = "pp_";
                else if (EnumManager.PhotoType.LiveModule == type) prefix = "lm_";
                else if (EnumManager.PhotoType.None == type) prefix = "n_";
                else prefix = "n_";
            }
            return prefix + Cons_GenerateHashID() + extension;
        }


        public string Cons_GenerateHashID()
        {
            string unique = string.Empty;
            if (photoID == 0)
                unique = System.Guid.NewGuid().ToString();
            else
                unique = photoID.ToString();
            return MD5Service.Create(unique).ToLower();
        }
        #endregion
    }
}
