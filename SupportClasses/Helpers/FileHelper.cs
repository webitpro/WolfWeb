using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using System.Drawing;
using WebIT.Lib;
using System.Drawing.Imaging;

namespace WebIT.Temp
{
    public class PostedFile
    {
        //////////
        // VARS //
        //////////       

        protected HttpPostedFileBase file = null;
        protected ModelStateDictionary model = null;
        protected string key = null;

        protected UrlHelper urlHelper = null;

        ////////////////
        // PROPERTIES //
        ////////////////

        public bool IsRequired { get; protected set; } //is file required
        public string Name { get; protected set; } //file name
        public bool HasFile
        {
            get
            {
                return (file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(file.FileName));
            }
        }
        protected bool IsValidForUpload { get; set; }
        public string SavePath { get; protected set; } //save path       
        public string OriginalPath { get; set; } //original path for previously saved file
        public string SavedName { get; set; }

        public bool IsSaved { get; protected set; } //is file saved on the disk


        /////////////////
        // CONSTRUCTOR //
        /////////////////

        /// <summary>
        /// File Helper constructor
        /// </summary>
        /// <param name="postedFileBase">uploaded file</param>
        /// <param name="modelName">model name/id</param>
        /// <param name="modelState"></param>
        public PostedFile(HttpPostedFileBase postedFileBase, string modelName, ModelStateDictionary modelState)
        {
            file = postedFileBase;
            key = modelName;
            model = modelState;

            ResetProps();

            HttpContextWrapper contextWrapper = new HttpContextWrapper(HttpContext.Current);
            RequestContext reqContext = new RequestContext(contextWrapper, RouteTable.Routes.GetRouteData(contextWrapper));
            urlHelper = new UrlHelper(reqContext);
        }
        public PostedFile()
        {
        }
        /////////////
        // METHODS //
        /////////////

        /// <summary>
        /// Validate file extension
        /// override for different file types
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateExtension()
        {
            return true;
        }



        /// <summary>
        /// Validate if file exists, file content lenght, file name and file extension
        /// </summary>
        /// <param name="required">Is file required</param>
        /// <returns></returns>
        public void ValidateForUpload(bool required)
        {
            IsRequired = required;

            bool bValid = true;
            bool bHasFile = false;
            #region check if file exists and set file name
            if (file != null)
            {
                bHasFile = true;
                //check for file name and set property "Name"
                if (!String.IsNullOrEmpty(file.FileName))
                {
                    Name = file.FileName;
                }
            }
            else
            {
                bHasFile = false;
            }
            #endregion

            #region check if file is required or not and validate content length, and file name
            if (IsRequired)
            {
                //file is required
                if (!bHasFile || file.ContentLength == 0 || string.IsNullOrEmpty(Name))
                {
                    bValid = false;
                    if (file == null)
                    {
                        model.AddModelError(key, "File is missing");
                    }
                    else
                    {
                        if (file.ContentLength == 0)
                        {
                            model.AddModelError(key, "File has zero length");
                        }
                        if (string.IsNullOrEmpty(Name))
                        {
                            model.AddModelError(key, "File name is missing");
                        }
                    }
                }
            }
            else
            {
                //file is not required
                if (bHasFile)
                {
                    if (file.ContentLength == 0 || string.IsNullOrEmpty(Name))
                    {
                        bValid = false;
                        if (file.ContentLength == 0)
                        {
                            model.AddModelError(key, "File has zero length");
                        }
                        if (string.IsNullOrEmpty(Name))
                        {
                            model.AddModelError(key, "File name is missing");
                        }
                    }
                }
            }
            #endregion

            #region if everything is valid, check for extension
            if (bValid && bHasFile)
            {
                if (!ValidateExtension())
                {
                    model.AddModelError(key, "Invalid file type");
                    bValid = false;
                }
            }
            #endregion

            IsValidForUpload = bValid && bHasFile;

        }

        /// <summary>
        /// Save File
        /// </summary>
        /// <param name="fileName"> file name with extension to be saved</param>
        /// <returns>bool (IsSaved), read SavePath after successful saving to get DB value</returns>
        public bool Save(string fileName)
        {
            if (IsValidForUpload)
            {
                SavedName = Path.GetFileNameWithoutExtension(fileName) + "-" + DateTime.Now.Ticks.ToString() + "-" + new Random().Next(1, 100).ToString() + Path.GetExtension(fileName);
                try
                {
                    file.SaveAs(urlHelper.UploadPath(SavedName));
                    IsSaved = true;
                    SavePath = "Content/data/" + SavedName;
                }
                catch (Exception ex)
                {
                    model.AddModelError(key, ex.Message);
                    IsSaved = false;
                }
            }
            else
            {
                IsSaved = false;
            }

            return IsSaved;
        }

        /// <summary>
        /// save image to a disk
        /// </summary>
        /// <param name="src"></param>
        /// <param name="fileName">including extension</param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public bool Save(Bitmap src, string fileName, long quality, ImageFormat f)
        {
            SavedName = Path.GetFileNameWithoutExtension(fileName) + "-" + DateTime.Now.Ticks.ToString() + "-" + new Random().Next(1, 100).ToString() + Path.GetExtension(fileName);

            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo codec = Utils.Image.GetEncoderInfo(f);
            if (codec == null)
            {
                IsSaved = false;
                return IsSaved;
            }

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            try
            {
                string path = urlHelper.UploadPath(SavedName);
                src.Save(path, codec, encoderParams);
                IsSaved = true;
                SavePath = "Content/data/" + SavedName;
                src.Dispose();
            }
            catch (Exception ex)
            {
                model.AddModelError(key, ex.Message);
                IsSaved = false;
            }

            

            return IsSaved;
        }

        /// <summary>
        /// Clean up properties and files uploaded by not saved to the database
        /// </summary>
        public void Cleanup(bool bDeleteFile = false)
        {
            if (bDeleteFile)
            {
                if (File.Exists(urlHelper.UploadPath(OriginalPath)))
                {
                    try
                    {
                        //delete uploaded file because database is not updated
                        File.Delete(urlHelper.UploadPath(OriginalPath));
                    }
                    catch (Exception ex)
                    {
                        model.AddModelError(key, ex.Message);
                    }
                }
            }

            //reset properties
            ResetProps();
        }

        /// <summary>
        /// Reset all properties to their default values
        /// </summary>
        protected void ResetProps()
        {
            IsRequired = true;
            Name = null;
            IsValidForUpload = false;
            SavePath = null;
            OriginalPath = null;
            SavedName = "";
            IsSaved = false;

        }

    }


    public class CompressedFile : PostedFile
    {
        public CompressedFile(HttpPostedFileBase file, string modelName, ModelStateDictionary modelState)
            : base(file, modelName, modelState)
        {
        }

        protected override bool ValidateExtension()
        {
            return Utils.FileType.IsCompressedFile(Path.GetExtension(this.file.FileName));
        }
    }

    public class ImageFile : PostedFile
    {
        private Bitmap img = null;
        ImageFormat format = null;

        public ImageFile(HttpPostedFileBase file, string modelName, ModelStateDictionary modelState)
            : base(file, modelName, modelState)
        {
            if (file != null)
            {
                img = new Bitmap(this.file.InputStream);
                string ext = Path.GetExtension(file.FileName);
                format = GetImageFormatFromExtension(ext);
            }
        }

        private string GetExtensionFromImageFormat(ImageFormat f)
        {
            string ext = "";
            if (f == ImageFormat.Jpeg)
            {
                ext = ".jpg";
            }
            else if (f == ImageFormat.Gif)
            {
                ext = ".gif";
            }
            else if (f == ImageFormat.Png)
            {
                ext = ".png";
            }
            else if (f == ImageFormat.Tiff)
            {
                ext = ".tiff";
            }
            return ext;
        }
        private ImageFormat GetImageFormatFromExtension(string ext)
        {
            ImageFormat f = null;
            switch (ext)
            {
                case ".jpg":
                    f = ImageFormat.Jpeg;
                    break;
                case ".jpeg":
                    f = ImageFormat.Jpeg;
                    break;
                case ".gif":
                    f = ImageFormat.Gif;
                    break;
                case ".png":
                    f = ImageFormat.Png;
                    break;
                case ".tiff":
                    f = ImageFormat.Tiff;
                    break;
            }
            return f;
        }

        protected override bool ValidateExtension()
        {
            return Utils.FileType.IsImageFile(Path.GetExtension(this.file.FileName));
        }

        public Size? GetDimensions()
        {
            if (img != null)
            {
                return new Size(img.Width, img.Height);
            }
            return null;
        }


        public bool Save(string fileNameWithoutExtension, Size? dims, bool resizeAndCrop)
        {
            bool? success = null;

            Size dimensions = new Size(0, 0);
            if (dims.HasValue)
            {
                dimensions = (Size)dims;
            }

            if ((img.Width != dimensions.Width && dimensions.Width > 0) || (img.Height != dimensions.Height && dimensions.Height > 0))
            {
                Bitmap resizedImage = resizeAndCrop ? Utils.Image.ResizeAndCrop(img, dimensions.Width, dimensions.Height) : Utils.Image.ResizeImage(img, dimensions.Width, dimensions.Height);
                img.Dispose();
                success = base.Save(resizedImage, fileNameWithoutExtension + GetExtensionFromImageFormat(format), 80, format);
                
            }

            if (!success.HasValue)
            {
                return base.Save(fileNameWithoutExtension + GetExtensionFromImageFormat(format));
            }
            else
            {
                return (bool)success;
            }
        }

    }

    public class AudioFile : PostedFile
    {
        public AudioFile(HttpPostedFileBase file, string modelName, ModelStateDictionary modelState)
            : base(file, modelName, modelState)
        {
        }

        protected override bool ValidateExtension()
        {
            return Utils.FileType.IsAudioFile(Path.GetExtension(this.file.FileName));
        }
    }

    public class VideoFile : PostedFile
    {
        public VideoFile(HttpPostedFileBase file, string modelName, ModelStateDictionary modelState)
            : base(file, modelName, modelState)
        {

        }

        protected override bool ValidateExtension()
        {
            return Utils.FileType.IsVideoFile(Path.GetExtension(this.file.FileName));
        }
    }
    
    public class DocumentFile : PostedFile
    {
        public DocumentFile(HttpPostedFileBase file, string modelName, ModelStateDictionary modelState)
            : base(file, modelName, modelState)
        {
        }

        protected override bool ValidateExtension()
        {
            return Utils.FileType.IsDocumentFile(Path.GetExtension(this.file.FileName));
        }
    }

    public class DataFile : PostedFile
    {
        public DataFile(HttpPostedFileBase file, string modelName, ModelStateDictionary modelState)
            : base(file, modelName, modelState)
        {

        }

        protected override bool ValidateExtension()
        {
            return Utils.FileType.IsDataFile(Path.GetExtension(this.file.FileName));
        }
    }

}