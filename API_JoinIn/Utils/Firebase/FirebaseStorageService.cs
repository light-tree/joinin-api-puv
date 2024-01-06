using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System.Security.Policy;
using System;

namespace API_JoinIn.Utils.Firebase
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly string _credentialFilePath;
        private readonly string _bucketName;

        public FirebaseStorageService(IConfiguration configuration)
        {
            _credentialFilePath = configuration.GetValue<string>("Firebase:CredentialFilePath");
            _bucketName = configuration.GetValue<string>("Firebase:BucketName");
        }

        public async Task<string> UploadImageToFirebase(Stream imageStream, string imageName)
        {
            var credential = GoogleCredential.FromFile(_credentialFilePath);
            var storageClient = StorageClient.Create(credential);
            var obj = await storageClient.UploadObjectAsync(_bucketName, imageName, null, imageStream);
            return obj.MediaLink;
        }

        public string GetImageDownloadUrl(string imageName)
        {
            var credential = GoogleCredential.FromFile(_credentialFilePath);
            var storageClient = StorageClient.Create(credential);

            var storageObject = storageClient.GetObject(_bucketName, imageName);
            return storageObject.MediaLink;
        }
        public bool DeleteImage( string filePath)
        {
            try
            {
                var credential = GoogleCredential.FromFile(_credentialFilePath);
                var storageClient = StorageClient.Create(credential);
                // get image name
                var fileName = GetObjectNameFromImageUrl(filePath);
                storageClient.DeleteObject(_bucketName, fileName);
                return true;
            } catch(Exception ex) {
                return false;
            }
        }

     
        public string GetObjectNameFromImageUrl(string imageUrl)
        {
            Uri uri = new Uri(imageUrl);
            string[] segments = uri.Segments;
            string objectName = segments[segments.Length - 1];       
            if (objectName.EndsWith("/"))
            {
                objectName = objectName.Remove(objectName.Length - 1);
            }

            return objectName;
        }
    }
}
