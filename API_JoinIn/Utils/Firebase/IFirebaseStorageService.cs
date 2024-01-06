namespace API_JoinIn.Utils.Firebase
{
    public interface IFirebaseStorageService
    {
        public Task<string> UploadImageToFirebase(Stream imageStream, string imageName);
        public string GetImageDownloadUrl(string imageName);
        public bool DeleteImage(string filePath);
    }
}
