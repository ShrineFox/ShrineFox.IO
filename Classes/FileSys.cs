using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShrineFox.IO
{
    public class FileSys
    {
        /// <summary>
        /// Copies a folder and all of its contents (including subfolders) to another destination.
        /// </summary>
        /// <param name="sourceFolder">The path of the directory to copy.</param>
        /// <param name="destFolder">The path of the directory to copy to.</param>
        public static void CopyDir(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder) && !File.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            // Get Files & Copy
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest, true);
            }

            // Get dirs recursively and copy files
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyDir(folder, dest);
            }
        }

        /// <summary>
        /// Deletes all empty subdirectories in a directory.
        /// </summary>
        /// <param name="folderPath">The path to the directory to delete empty subdirectories of.</param>
        public static void DeleteEmptySubdirs(string folderPath)
        {
            try
            {
                foreach (var subdir in Directory.GetDirectories(folderPath, "*.*", SearchOption.AllDirectories))
                    DeleteEmptySubdirs(subdir);

                if (Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).Count() == 0)
                    Directory.Delete(folderPath);
            } catch { }
        }

        /// <summary>
        /// Determines whether two files are identical.
        /// </summary>
        /// <param name="file1">The first file to compare.</param>
        /// <param name="file2">The second file to compare.</param>
        /// <returns></returns>
        public static bool AreFilesIdentical(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read);
            fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }

        public static string GetExtensionlessPath(string path)
        {
            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
        }

        /// <summary>
        /// Waits up to 20 seconds for a file to exist and become available to open.
        /// </summary>
        /// <param name="fullPath">The path to the file to wait for.</param>
        /// <returns></returns>
        public static FileStream WaitForFile(string fullPath, 
            FileMode mode = FileMode.Open, 
            FileAccess access = FileAccess.ReadWrite, 
            FileShare share = FileShare.None)
        {
            for (int numTries = 0; numTries < 10; numTries++)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fullPath, mode, access, share);
                    return fs;
                }
                catch (IOException)
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                    Thread.Sleep(2000);
                }
            }
            return null;
        }
    }
}
