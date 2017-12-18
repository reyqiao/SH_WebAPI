using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
namespace Niu.Live.Chat.BusinessLogic
{
    public class PhotoHandler
    {
        private static readonly int MaxFilesNumber = 200;
        private static readonly string PhotoExtension = ConfigurationManager.AppSettings["PhotoExtension"];
        private static readonly string RootFolder = ConfigurationManager.AppSettings["RootFolder"];
        private static readonly string SaveRootFolder = ConfigurationManager.AppSettings["SaveRootFolder"];
        private static readonly string PhotoDomain = ConfigurationManager.AppSettings["PhotoDomain"];


        public static bool AddPhoto(long userId, string extension, int width, int height, out long photoId, out string saveFolder, out string saveFileName, out string saveFilePath, out string url)
        {
            bool result = false;
            photoId = 0;
            saveFileName = string.Empty;
            saveFolder = string.Empty;
            saveFilePath = string.Empty;
            url = string.Empty;
            string hashId =DateTime.Now.Ticks.ToString();
            DateTime addtime = new DateTime();
            string ip = Niu.Cabinet.Web.ExtensionMethods.RequestExtensions.ClientIp(System.Web.HttpContext.Current.Request);
            if (Niu.Live.Chat.DataAccess.PhotoAccess.AddPhoto(userId, hashId, width, height, extension, ip, out photoId, out addtime) && photoId > 0)
            {
                saveFolder = GetSaveFolderPath(photoId, addtime);
                saveFileName = GetFileName(hashId, extension);
                saveFilePath = GetSaveFilePath(saveFolder, saveFileName);
                if (Niu.Live.Chat.DataAccess.PhotoAccess.UpdateSaveFilePath(photoId, saveFilePath))
                {
                    url = "https://" + PhotoDomain + GetSaveFolderPath_url(photoId, addtime).Replace('\\', '/') + saveFileName;
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="files"></param>
        /// <returns>
        /// result 0 未处理 -1 失败 1 成功
        /// code 
        /// </returns>
        public static ResultData<PhotoItemBase> Upload(long userId, HttpFileCollectionBase files)
        {
            ResultData<PhotoItemBase> resultData = new ResultData<PhotoItemBase>();

            string directory = string.Empty;//保存目录
            string fileName = string.Empty;//上传文件名
            string savePath = string.Empty;//保存路径

            string url = string.Empty;

            if (files != null && files.Count > 0)
            {
                string extensionName = string.Empty;
                long photoId = 0L;
                // 支持多文件上传（如果为flash提供支持，其实是不用for循环的，只接收一个文件上传，flash多线程请求该接口）。
                for (int index = 0; index < files.Count; index++)
                {
                    url = string.Empty;
                    savePath = string.Empty;
                    extensionName = System.IO.Path.GetExtension(files[index].FileName).ToLower();
                    if (PhotoExtension.IndexOf(extensionName) >= 0)
                    {
                        try
                        {
                            Stream stream = files[index].InputStream;
                            Bitmap original = new Bitmap(stream);
                            int oriW = original.Width;//原始宽度
                            int oriH = original.Height;//原始高度
                            int width = original.Width;//目标宽度
                            int height = original.Height;//目标高度
                            double proportion = (double)width / (double)height;

                            if (AddPhoto(userId, extensionName, width, height, out photoId, out directory, out fileName, out savePath, out url))
                            {
                                if (false == System.IO.Directory.Exists(directory))
                                {
                                    System.IO.Directory.CreateDirectory(directory);
                                }

                                //设置图片保存质量
                                ImageCodecInfo codecInfos = GetEncoderInfo(".jpg");
                                EncoderParameters encoderParams = GetEncoderParameters();
                                original.Save(savePath);
                                original.Dispose();
                                //#region 缩小图片
                                //Bitmap destination = new Bitmap(width, height, PixelFormat.Format16bppRgb555);
                                //Graphics graphics = Graphics.FromImage(destination);
                                //graphics.InterpolationMode = InterpolationMode.Default;
                                //graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                //graphics.DrawImage(original, new Rectangle(0, 0, width, height), new Rectangle(offsetX, offsetY, oriW, oriH), GraphicsUnit.Pixel);
                                //destination.Save(savePath, codecInfos, encoderParams);
                                //graphics.Dispose();
                                //destination.Dispose();
                                //#endregion
                                resultData.data = new List<PhotoItemBase>();
                                resultData.data.Add(new PhotoItemBase { photoId = photoId, url = url, width = oriW, height = oriH });
                                resultData.message += "完成";
                                resultData.result = 1;
                                resultData.code = 0;

                            }
                            else
                            {
                                resultData.message += "图片数据入库失败";
                                resultData.result = 0;
                                resultData.code = 31;
                            }
                        }
                        catch (Exception ex)
                        {
                            Niu.Cabinet.Logging.LogRecord _log = new Cabinet.Logging.LogRecord(Niu.Cabinet.Config.AppSetting.AppSettingString("", string.Empty));
                            _log.WriteSingleLog("upload.txt", ex.ToString());
                            resultData.message += ex.ToString();
                            resultData.result = 0;
                            resultData.code = 21;
                        }
                    }
                    else
                    {
                        resultData.message += "不是允许的扩展名";
                        resultData.result = 0;
                        resultData.code = 11;
                    }
                }
            }
            else
            {
                resultData.message = "无上传文件数据";
                resultData.result = 0;
                resultData.code = 11;
            }
            return resultData;
        }


        public static string GetHashID(string timestamp)
        {
            return Niu.Cabinet.Cryptography.MD5Service.Create(timestamp).ToUpper();
        }

        /// <summary>
        /// 只是目录
        /// </summary>
        /// <param name="photoId"></param>
        /// <param name="addTime"></param>
        /// <returns></returns>
        public static string GetSaveFolderPath(long photoId, DateTime addTime)
        {
            //string serverPath = System.Web.HttpContext.Current.Server.MapPath("./" + SaveRootFolder);
            StringBuilder sb = new StringBuilder();
            sb.Append(SaveRootFolder);
            sb.Append("\\" + addTime.Year.ToString("0000") + "\\" + addTime.Month.ToString("00") + addTime.Day.ToString("00") + "\\");
            sb.Append((photoId / MaxFilesNumber).ToString() + "\\");
            return sb.ToString();
        }
        public static string GetSaveFolderPath_url(long photoId, DateTime addTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\\" + addTime.Year.ToString("0000") + "\\" + addTime.Month.ToString("00") + addTime.Day.ToString("00") + "\\");
            sb.Append((photoId / MaxFilesNumber).ToString() + "\\");
            return sb.ToString();
        }
        public static string GetSaveFilePath(string folderPath, string fileName)
        {
            return System.IO.Path.Combine(folderPath, fileName);
        }

        public static string GetFileName(string hashId, string extension)
        {
            return "S_" + hashId + extension;
        }

        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        public static ImageCodecInfo GetEncoderInfo(string extensionName)
        {
            ImageCodecInfo[] codecInfos = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo info in codecInfos)
                if (info.FilenameExtension.ToLower().IndexOf(extensionName.ToLower()) >= 0)
                    return info;
            return null;
        }


        public static EncoderParameters GetEncoderParameters()
        {
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60L);
            return encoderParams;
        }
        public class PhotoItemBase
        {
            public long photoId { get; set; }
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }

        }
        public class ResultData<T>
        {
            public string message { get; set; }
            public int result { get; set; }
            public int code { get; set; }
            public List<T> data { get; set; }
            //resultData.message += "图片数据入库失败";
            //                     resultData.result = 0;
            //                     resultData.code = 31;
        }
    }
}