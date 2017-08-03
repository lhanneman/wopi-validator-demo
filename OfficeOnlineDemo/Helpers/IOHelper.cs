using System.IO;

namespace OfficeOnlineDemo.Helpers
{
    public class IOHelper
    {
        public static byte[] StreamToBytes(Stream stream)
        {
            stream.Position = 0;
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static string Utf7Encode(string value)
        {
            return System.Text.Encoding.UTF7.GetString(System.Text.Encoding.ASCII.GetBytes(value));
        }
    }
}