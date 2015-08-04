using System;
using System.IO;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NuSurvey.MVC.Resources;

namespace NuSurvey.MVC.Services
{
    public interface IBlobStoargeService
    {
        void UploadPhoto(int id, byte[] img);
        byte[] GetPhoto(int id, string type);
    }

    public class BlobStoargeService : IBlobStoargeService
    {
        private readonly IPictureService _pictureService;
        private readonly CloudBlobContainer _container;

        public BlobStoargeService(IPictureService pictureService)
        {
            _pictureService = pictureService;

            var storageConnectionString =
            string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                  CloudConfigurationManager.GetSetting("AzureStorageAccountName"),
                  CloudConfigurationManager.GetSetting("AzureStorageKey"));

            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            _container = blobClient.GetContainerReference("healthykidsphotos");
            _container.CreateIfNotExists();
            _container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Off });
        }

        public void UploadPhoto(int id, byte[] img)
        {
            var thumb = _pictureService.MakeThumbnail(img);
            var directorThumb = _pictureService.MakeDirectorThumbnail(img);
            var displayWithWater = _pictureService.MakeDisplayImage(img, true);
            var displayNoWater = _pictureService.MakeDisplayImage(img);

            var blobOriginal = _container.GetBlockBlobReference(string.Format("{0}_{1}.jpg", id, Resource.Original));
            blobOriginal.UploadFromByteArray(img, 0, img.Length);

            var blobThumb = _container.GetBlockBlobReference(string.Format("{0}_{1}.jpg", id, Resource.Thumb));
            blobThumb.UploadFromByteArray(thumb, 0, thumb.Length);

            var blobDirectorThumb = _container.GetBlockBlobReference(string.Format("{0}_{1}.jpg", id, Resource.DirectorThumb));
            blobDirectorThumb.UploadFromByteArray(directorThumb, 0, directorThumb.Length);

            var blobWater = _container.GetBlockBlobReference(string.Format("{0}_{1}.jpg", id, Resource.Water));
            blobWater.UploadFromByteArray(displayWithWater, 0, displayWithWater.Length);

            var blobNoWater = _container.GetBlockBlobReference(string.Format("{0}_{1}.jpg", id, Resource.PDF));
            blobNoWater.UploadFromByteArray(displayNoWater, 0, displayNoWater.Length);
        }

        public byte[] GetPhoto(int id, string type)
        {
            byte[] contents = null;
            try
            {
                //Get file from blob storage and populate the contents
                var blob = _container.GetBlockBlobReference(string.Format("{0}_{1}.jpg", id, type));
                using (var stream = new MemoryStream())
                {
                    blob.DownloadToStream(stream);
                    using (var reader = new BinaryReader(stream))
                    {
                        stream.Position = 0;
                        contents = reader.ReadBytes((int)stream.Length);
                    }
                }
            }
            catch (Exception)
            {
                return contents;
            }


            return contents;
        }
    }
}