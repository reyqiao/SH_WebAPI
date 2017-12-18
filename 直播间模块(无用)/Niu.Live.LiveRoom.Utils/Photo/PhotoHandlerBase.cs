using Niu.Cabinet.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using System.IO;
using System.Drawing;

namespace Niu.Live.LiveRoom.Utils.Photo
{
    public class PhotoHandlerBase
    {

        public static bool Update(PhotoItem photoItem, string userIP, long userID)
        {
            return PhotoAccess.Update(photoItem.PhotoID, photoItem.HashID, userID, (int)photoItem.PhotoType, photoItem.Width, photoItem.Height, photoItem.Extension, userIP, photoItem.AddTime);
        }


        protected static bool Add(PhotoItem photoItem, string userIP, long userID, out long photoID)
        {
            bool result = false;
            string errorMessage = string.Empty;
            photoID = 0;

            if (PhotoAccess.Add(photoItem.HashID, userID, (int)photoItem.PhotoType, photoItem.Width, photoItem.Height, photoItem.Extension, userIP, photoItem.AddTime, out photoID, out errorMessage) && photoID > 0)
            {
                result = true;
            }
            return result;
        }



        public List<PhotoItem> Upload(HttpFileCollection fileCollect, EnumManager.PhotoType photoType, string userIP, long userID)
        {
            List<PhotoItem> result = new List<PhotoItem>();
            if (fileCollect != null && fileCollect.Count > 0)
            {
                for (int i = 0; i < fileCollect.Count; i++)
                {
                    HttpPostedFile postedFile = fileCollect[i];
                    string extension = System.IO.Path.GetExtension(postedFile.FileName).ToLower();
                    if (!string.IsNullOrEmpty(extension) && PhotoConfig.PhotoExtension.IndexOf(extension) >= 0)
                    {
                        long photoID = 0;
                        if (PhotoAccess.GetUnique(userID, (int)photoType, userIP, out photoID) && photoID > 0)
                        {
                            PhotoItem photoItem = SavePhoto(postedFile, photoID);

                            if (photoItem != null && photoItem.Url.Length > 0)
                            {
                                if (Update(photoItem, userIP, userID))
                                {
                                    result.Add(photoItem);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }


        public virtual PhotoItem SavePhoto(HttpPostedFile postedFile, long photoID)
        {
            DateTime addTime = DateTime.Now;
            Stream stream = postedFile.InputStream;
            Bitmap original = new Bitmap(stream);
            int width = original.Width;
            int height = original.Height;
            string extension = System.IO.Path.GetExtension(postedFile.FileName).ToLower();

            PhotoConfig pc = new PhotoConfig(EnumManager.PhotoType.NewsPhoto, photoID, extension, addTime);
            try
            {
                if (!System.IO.Directory.Exists(pc.Cons_GetFolder())) { System.IO.Directory.CreateDirectory(pc.Cons_GetFolder()); }
                postedFile.SaveAs(pc.Cons_GetFolderFilePath());
                PhotoItem photoItem = new PhotoItem(EnumManager.PhotoType.NewsPhoto, photoID, pc.Cons_GenerateHashID(), extension, width, height, addTime);
                return photoItem;
            }
            catch { }

            return null;
        }



        protected static ImageCodecInfo GetEncoderInfo(string extensionName)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo info in encoders)
                if (info.FilenameExtension.ToLower().IndexOf(extensionName.ToLower()) >= 0)
                    return info;
            return null;
        }


        protected static EncoderParameters GetEncoderParameters(long value = 60)
        {
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, value);
            return encoderParams;
        }

    }

    public class PhotoAccess
    {
        private static string DB_Live = ConnectionString.Get("DB_LiveConnection", string.Empty);
        internal static bool Update(long photoID, string hashID, long userID, int type, int width, int height, string extension, string ip, DateTime addTime)
        {
            using (IDbConnection conn = new SqlConnection(DB_Live))
            {
                var para = new DynamicParameters();
                para.Add("@PhotoID", photoID);
                para.Add("@HashID", hashID);
                para.Add("@UserID", userID);
                para.Add("@Type", type);
                para.Add("@Width", width);
                para.Add("@Height", height);
                para.Add("@Extension", extension);
                para.Add("@IP", ip);
                para.Add("@AddTime", addTime);
                return conn.Execute("Photo_Update", para, commandType: CommandType.StoredProcedure) > 0;
            }
        }

        internal static bool Add(string hashID, long userID, int type, int width, int height, string extension, string ip, DateTime addTime, out long photoID, out string errorMessage)
        {
            photoID = 0;
            errorMessage = "";
            using (IDbConnection conn = new SqlConnection(DB_Live))
            {
                var para = new DynamicParameters();
                para.Add("@HashID", hashID);
                para.Add("@UserID", userID);
                para.Add("@Type", type);
                para.Add("@Width", width);
                para.Add("@Height", height);
                para.Add("@Extension", extension);
                para.Add("@IP", ip);
                para.Add("@AddTime", addTime);
                para.Add("@PhotoID", direction: ParameterDirection.Output);
                if (conn.Execute("Photo_Add", para, commandType: CommandType.StoredProcedure) > 0)
                {
                    photoID = para.Get<long>("@PhotoID");
                    return true;
                }
                return false;
            }
        }

        internal static bool GetUnique(long userID, int type, string ip, out long photoID)
        {
            photoID = 0;
            using (IDbConnection conn = new SqlConnection(DB_Live))
            {
                var para = new DynamicParameters();
                para.Add("@UserID", userID);
                para.Add("@Type", type);
                para.Add("@IP", ip);
                para.Add("@PhotoID", direction: ParameterDirection.Output);
                if (conn.Execute("Photo_GetUnique", para, commandType: CommandType.StoredProcedure) > 0)
                {
                    photoID = para.Get<long>("@PhotoID");
                    return true;
                }
                return false;
            }
        }
    }
}
