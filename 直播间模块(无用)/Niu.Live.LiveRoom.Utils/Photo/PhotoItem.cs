using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niu.Live.LiveRoom.Utils.Photo
{
    public class PhotoItem
    {
        public string UrlRootFolder { get; private set; }


        public string SaveRootFolder { get; private set; }


        public string PhotoDomain { get; private set; }



        public EnumManager.PhotoType PhotoType { get; private set; }



        public DateTime AddTime { get; private set; }



        public string HashID { get; private set; }



        public string Extension { get; private set; }



        public string SaveFilePath { get; private set; }



        public long PhotoID { get; set; }
        public string Url { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public PhotoItem(EnumManager.PhotoType photoType, long photoID, string hashID, string extension, int width, int height, DateTime addTime)
        {
            PhotoConfig pc = new PhotoConfig(photoType, photoID, extension, addTime);
            UrlRootFolder = pc.urlRootFolder;
            SaveRootFolder = pc.saveRootFolder;
            PhotoDomain = pc.photoDomain;
            PhotoType = photoType;
            PhotoID = photoID;
            HashID = hashID;
            Extension = extension;
            Width = width;
            Height = height;
            AddTime = addTime;
            string _fileName = pc.Cons_GetFileName();
            string _saveFolderPath = pc.Cons_GetFolder();
            Url = pc.Cons_GetUrl();
            SaveFilePath = pc.Cons_GetFolderFilePath();
        }
    }
}
