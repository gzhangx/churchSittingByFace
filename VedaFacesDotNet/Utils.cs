using System;
using System.Text;
using System.Security.Cryptography;
namespace VedaFacesDotNet
{
    public class Utils
    {
        public static String doMd5(String source)
        {
            using (var md5Hash = MD5.Create())
            {
                var sbytes = Encoding.UTF8.GetBytes(source);
                var hbytes = md5Hash.ComputeHash(sbytes);

                return BitConverter.ToString(hbytes).Replace("-", string.Empty);                
            }
        }
    }
}
