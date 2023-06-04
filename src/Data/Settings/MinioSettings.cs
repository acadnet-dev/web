using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Settings
{
    public class S3Settings
    {
        public string Endpoint { get; set; } = default!;
        public string AccessKey { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public bool WithSSL { get; set; } = default!;
    }
}