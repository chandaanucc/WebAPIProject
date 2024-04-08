using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace InfrastructureLayer.Interface
{
    public interface IGoogleDriveService
    {
        Task<string?> UploadFileAsync (IFormFile file, string filename);

        Task<byte[]> DownloadFileAsync(string fileId);
        //Task DeleteFileAsync(string fileId);
    }
}