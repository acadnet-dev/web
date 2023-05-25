using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.S3
{
    public class S3Object : S3ObjectStat
    {
        public string ContentType { get; set; } = default!;
        public Stream Content { get; set; } = default!;
    }
}