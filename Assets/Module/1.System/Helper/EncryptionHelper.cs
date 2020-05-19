using System.IO;
using System.Security.Cryptography;

namespace Module
{
    public static class EncryptionHelper
    {
        public static string GetMD5(string filePath)
        {
            byte[] retVal;
            using (FileStream file = new FileStream(filePath, FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                retVal = md5.ComputeHash(file);
            }
            return retVal.ToHex("x2");
        }
        
    }
}