using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using InfrastructureLayer.Interface;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly DriveService _driveService;

        public GoogleDriveService()
        {
            string jsonKeyFilePath = "D:\\Documents\\dotnetfiles\\LayerProject\\layers\\tables\\villa\\InfrastructureLayer\\Credentials\\credentials.json";

            GoogleCredential credential;
            using (var stream = new FileStream(jsonKeyFilePath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(DriveService.ScopeConstants.Drive);
            }

            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "villa"
            });
        }

        public async Task<string?> UploadFileAsync(IFormFile file, string filename)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = filename
                
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = file.OpenReadStream())
            {
                request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
                request.Fields = "id";
                await request.UploadAsync();
            }

            return request.ResponseBody?.Id;
        }

        public async Task<byte[]> DownloadFileAsync(string fileId)
        {
            MemoryStream stream = new MemoryStream();

            try
            {
                var request = _driveService.Files.Get(fileId);
                await request.DownloadAsync(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                string errorMessage = "An error occurred while downloading the file: " + ex.Message;
                // Log the error
                //_logger.LogError(errorMessage);
                // You might also want to handle the exception differently depending on your application requirements
                throw new Exception(errorMessage, ex); // Rethrow the exception with a more descriptive message
            }
            finally
            {
                stream.Close();
            }
    }
}

}
