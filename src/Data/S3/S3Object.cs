using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.S3
{
    public class S3Object
    {
        public string BucketName { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public Stream Content { get; set; } = default!;
    }
}