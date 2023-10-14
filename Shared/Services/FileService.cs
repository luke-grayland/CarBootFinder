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
    private readonly string _storageAccount = "apiazureuploads";
    private readonly string _key = "xxx";
    private readonly BlobContainerClient _filesContainer;

    public FileService()
    {
        var credential = new StorageSharedKeyCredential(_storageAccount, _key);
        var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
        var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        _filesContainer = blobServiceClient.GetBlobContainerClient("files");
    }

    public async Task<List<BlobDto>> ListASync()
    {
        var files = new List<BlobDto>();

        await foreach (var file in _filesContainer.GetBlobsAsync())
        {
            var uri = _filesContainer.Uri.ToString();
            var name = file.Name;
            var fullUri = $"{uri}/{name}";
            
            files.Add(new BlobDto()
            {
                Uri = fullUri,
                Name = name,
                ContentType = file.Properties.ContentType
            });
        }
        return files;
    }

    public async Task<BlobResponseDto> UploadAsync(IFormFile blob)
    {
        var response = new BlobResponseDto();
        var client = _filesContainer.GetBlobClient((blob.FileName));

        await using (Stream? data = blob.OpenReadStream())
        {
            await client.UploadAsync(data);
        }

        response.Status = $"File Uploaded: {blob.FileName}";
        response.Error = false;
        response.Blob.Uri = client.Uri.AbsoluteUri;
        response.Blob.Name = client.Name;

        return response;
    }

    public async Task<BlobDto?> DownloadAsync(string blobFilename)
    {
        var file = _filesContainer.GetBlobClient(blobFilename);

        if (await file.ExistsAsync())
        {
            var data = await file.OpenReadAsync();
            var blobContent = data;

            var content = await file.DownloadContentAsync();

            var name = blobFilename;
            var contentType = content.Value.Details.ContentType;

            return new BlobDto()
            {
                Content = blobContent,
                Name = name,
                ContentType = contentType
            };
        }

        return null;
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
}
