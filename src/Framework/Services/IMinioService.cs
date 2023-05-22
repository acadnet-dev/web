using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minio;

namespace Framework.Services
{
    public interface IMinioService
    {
        MinioClient GetClient();
    }
}