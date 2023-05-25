using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.S3;

namespace Framework.Services
{
    public interface IFileService
    {
        Task UploadFileAsync(S3Object s3object);

        Task<S3Object?> DownloadFileAsync(string bucketName, string fileName);

        Task DeleteFileAsync(string bucketName, string fileName);

        Task CreateBucketAsync(string bucketName);

        Task<ICollection<S3ObjectStat>> GetFilesInBucketAsync(string bucketName);
    }
}