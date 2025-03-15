using System;

namespace EasePass.Helper.FileSystem
{
    /// <summary>
    /// Includes common Methods for Files
    /// </summary>
    internal static class FileHelper
    {
        /// <summary>
        /// Gets the Extension of the given <paramref name="fileName"/>
        /// </summary>
        /// <param name="fileName">The File, which extension should be returned</param>
        /// <returns>Returns the Extension of the given <paramref name="fileName"/>. The Filename begins with <see langword="."/></returns>
        public static ReadOnlySpan<char> GetExtension(ReadOnlySpan<char> fileName)
        {
            int index = fileName.LastIndexOf('.');
            if (index == -1)
                return ReadOnlySpan<char>.Empty;

            return fileName.Slice(index);
        }

        /// <summary>
        /// Checks if the given <paramref name="fileName"/> has the specified <paramref name="extension"/>
        /// </summary>
        /// <param name="fileName">The Filename, which includes the extension</param>
        /// <param name="extension">The Extension which should be equal to the of the <paramref name="fileName"/></param>
        /// <param name="comparison">The Comparison method, which should be used</param>
        /// <returns>Returns <see langword="true"/> if the <paramref name="fileName"/> has the same extension</returns>
        public static bool HasExtension(ReadOnlySpan<char> fileName, ReadOnlySpan<char> extension, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return GetExtension(fileName).Equals(extension, comparison);
        }
    }
}
