using System.Text;

namespace EasePass.Extensions
{
    /// <summary>
    /// Includes every Extension for the <see cref="byte"/>[]
    /// </summary>
    internal static class ByteArrayExtension
    {
        /// <summary>
        /// Converts the given <paramref name="data"/> to a <see cref="string"/>
        /// </summary>
        /// <param name="data">The Value, which should be converted</param>
        /// <returns>Returns the <paramref name="data"/> as <see cref="string"/></returns>
        public static string ConvertToString(this byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append((char)data[i]);
            }
            return sb.ToString();
        }
    }
}
