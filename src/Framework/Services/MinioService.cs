using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Minio;

namespace Framework.Services
{
    public class MinioService : IMinioService
    {
        private readonly IOptions<MinioSettings> _minioSettings;

        public MinioService(IOptions<MinioSettings> minioSettings)
        {
            _minioSettings = minioSettings;
        }

        public MinioClient GetClient()
        {
            return new MinioClient()
                .WithEndpoint(_minioSettings.Value.Endpoint)
                .WithCredentials(_minioSettings.Value.AccessKey, _minioSettings.Value.SecretKey)
                .WithSSL()
                .Build();
        }
    }
}