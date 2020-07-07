using System;
using System.IO;
using System.Security.Cryptography;

namespace Launcher.Helpers
{
    public static class FileHelper
    {
        public static string ComputeHash(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}