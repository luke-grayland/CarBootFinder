using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using CarBootFinderAPI.Shared.Models.BlobStorage;
using Microsoft.AspNetCore.Http;

namespace CarBootFinderAPI.Shared.Services;

public class FileService
{
    private const string BlobContainer = "car-boot-finder-cover-images";
    private readonly BlobContainerClient _filesContainer;

    public FileService()
    {
        var credential = new StorageSharedKeyCredential(
            Environment.GetEnvironmentVariable("BlobStorageAccountName"),
            Environment.GetEnvironmentVariable("BlobStorageAccountKey"));
        
        var blobUri = Environment.GetEnvironmentVariable("BlobStorageUrl");

        if (blobUri == null) return;
        var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        _filesContainer = blobServiceClient.GetBlobContainerClient(BlobContainer);
    }

    public async Task<BlobResponseDto> UploadAsync(IFormFile blob)
    {
        var response = new BlobResponseDto();
        
        if (!FileTypeValidation(blob))
        {
            response.Status = "Invalid file type";
            response.Error = true;
            return response;
        }
        
        var fileName = SanitiseFileName(blob.FileName) + ".jpg";
        var client = _filesContainer.GetBlobClient(fileName);
        
        await using (var data = blob.OpenReadStream())
        {
            await client.UploadAsync(data);
        }

        response.Status = $"File Uploaded: {blob.FileName}";
        response.Error = false;
        response.Blob.Uri = client.Uri.AbsoluteUri;
        response.Blob.Name = client.Name;

        return response;
    }

    public async Task<BlobResponseDto> DeleteAsync(string blobFileName)
    {
        var file = _filesContainer.GetBlobClient(blobFileName);

        await file.DeleteAsync();

        return new BlobResponseDto()
        {
            Error = false,
            Status = $"File deleted: {blobFileName}"
        };
    }
    
    private static string SanitiseFileName(string fileName)
    {
        var sanitizedFileName = Path.GetFileNameWithoutExtension(fileName);
        return string.Join("_", sanitizedFileName.Split(Path.GetInvalidFileNameChars()));
    }
    
    private static bool FileTypeValidation(IFormFile file)
    {
        var allowedTypes = new List<string> { "image/jpeg", "image/png" };

        if (file != null && file.Length > 0)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();
            var fileType = GetFileType(fileBytes);
            return allowedTypes.Contains(fileType);
        }

        return false;
    }
    
    private static string GetFileType(byte[] fileBytes)
    {
        return fileBytes.Length switch
        {
            > 2 when fileBytes[0] == 0xFF && fileBytes[1] == 0xD8 => "image/jpeg",
            > 8 when fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && fileBytes[2] == 0x4E && fileBytes[3] == 0x47 &&
                     fileBytes[4] == 0x0D && fileBytes[5] == 0x0A && fileBytes[6] == 0x1A &&
                     fileBytes[7] == 0x0A => "image/png",
            _ => null
        };
    }

}
