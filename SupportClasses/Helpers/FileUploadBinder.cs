using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;

namespace WebIT.Temp
{
    public class FileUploadBinder : IModelBinder
    {
        public static void RegisterTypes()
        {
            FileUploadBinder binder = new FileUploadBinder();
            ModelBinders.Binders[typeof(PostedFile)] = binder;
            ModelBinders.Binders[typeof(CompressedFile)] = binder;
            ModelBinders.Binders[typeof(ImageFile)] = binder;
            ModelBinders.Binders[typeof(AudioFile)] = binder;
            ModelBinders.Binders[typeof(VideoFile)] = binder;
            ModelBinders.Binders[typeof(DocumentFile)] = binder;
            ModelBinders.Binders[typeof(DataFile)] = binder;
        }

        public object BindModel(ControllerContext controllerCtx, ModelBindingContext bindingCtx)
        {
            if (controllerCtx == null)
            {
                throw new ArgumentNullException("controllerCtx");
            }
            if (bindingCtx == null)
            {
                throw new ArgumentNullException("bindingCtx");
            }

            Type uploadingFileType = typeof(PostedFile);
            Type modelType = bindingCtx.ModelType;

            if (modelType.Equals(uploadingFileType) || modelType.IsSubclassOf(uploadingFileType))
            {
                HttpPostedFileBase file = controllerCtx.HttpContext.Request.Files[bindingCtx.ModelName];

                return ChooseFileOrNull(file, bindingCtx);
            }

            return null;

        }

        internal static object ChooseFileOrNull(HttpPostedFileBase rawFile, ModelBindingContext bindingCtx)
        {
            HttpPostedFileBase file = rawFile;
            if (rawFile == null)
            {
                file = null;
            }
            else if (rawFile.ContentLength == 0 && String.IsNullOrEmpty(rawFile.FileName))
            {
                file = null;
            }

            ConstructorInfo constructor = bindingCtx.ModelType.GetConstructor(new Type[] { typeof(HttpPostedFileBase), typeof(string), typeof(ModelStateDictionary) });
            return constructor.Invoke(new object[] { file, bindingCtx.ModelName, bindingCtx.ModelState });
        }
    }
}