using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Framework.Services.FileServices
{
    public class FileServiceFactory
    {
        private readonly IWebHostEnvironment _env;
        private readonly IServiceProvider _serviceProvider;


        public FileServiceFactory(
            IWebHostEnvironment env,
            IServiceProvider serviceProvider
        )
        {
            _env = env;
            _serviceProvider = serviceProvider;
        }


        public IFileService GetFileService()
        {
            if (!_env.IsDevelopment())
            {
                return (IFileService)_serviceProvider.GetService(typeof(MinioService))!;
            }
            else
            {
                return (IFileService)_serviceProvider.GetService(typeof(S3FileService))!;
            }
        }
    }
}