using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
namespace theRightDirection.Library.UnitTesting.Extensions
{
    /// <summary>
    /// Extension methods for the Microsoft .Net Assembly-class
    /// </summary>
    public static class AssemblyExtension
    {
        #region Public Extension Methods
        /// <summary>
        /// Finds a file next to assembly, for example the configuration file
        /// if the exception is suppressed the method returns string.empty
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="suppressFileNotFoundException">if set to <c>true</c> [suppress file not found exception].</param>
        /// <returns>
        /// path to the file
        /// </returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="FileNotFoundException">In case of the file is not found</exception>
        public static string FindFileNextToAssembly(this Assembly assembly, string fileName, bool suppressFileNotFoundException = false)
        {
            string uriString = assembly.CodeBase;          
            Uri uri = new Uri(uriString);
            string path = Path.Combine(Path.GetDirectoryName(uri.LocalPath), fileName);
            if (!File.Exists(path))
            {
                var message = string.Format("File is not found next to assembly: {0}", path);
                if (!suppressFileNotFoundException)
                {
                    throw new FileNotFoundException(message);
                }
                else
                {
                    path = string.Empty;
                }
            }
            return path;
        }
        #endregion
    }
}