using XSystem.Security.Cryptography;

namespace DeployFiles
{
    class Utils
    {
        public static string GetSHA1HashFromFile(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                using (SHA1Managed sha = new SHA1Managed())
                {
                    byte[] checksum = sha.ComputeHash(stream);
                    string sendCheckSum = BitConverter.ToString(checksum).Replace("-", string.Empty);
                    return sendCheckSum;
                }
            }
        }
    }
}
