using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWBNetworkingPackage
{
    public class AbnormalDirectoryHandler
    {
        public static void CreateDirectory(string filepath)
        {
            if (filepath.StartsWith("/"))
            {
                Directory.CreateDirectory(filepath);
            }
            else
            {
                filepath = filepath.Replace('\\', '/');
                string[] directories = filepath.Split('/');
                string directory = "";
                for (int i = 0; i < directories.Length; i++)
                {
                    if (directory.EndsWith(":"))
                    {
                        directory = directory + Path.DirectorySeparatorChar + directories[i];
                    }
                    else
                    {
                        directory = Path.Combine(directory, directories[i]);
                    }
                }

                Directory.CreateDirectory(directory);
            }
        }

        public static void CreateDirectoryFromFile(string filepath)
        {
            filepath = filepath.Replace('\\', '/');
            string[] directories = filepath.Split('/');
            string directory = "";
            for (int i = 0; i < directories.Length - 1; i++)
            {
                if (directory.EndsWith(":"))
                {
                    directory = directory + Path.DirectorySeparatorChar + directories[i];
                }
                else
                {
                    directory = Path.Combine(directory, directories[i]);
                }
            }

            Directory.CreateDirectory(directory);
        }
    }
}