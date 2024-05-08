using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dt.File.MinioConfig
{
    public record MinioConfiguration(string endpoint, string accessKey, string secretKey, string rootFolderName, string bucketName, string location);
}
