﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Threading;

namespace Microsoft.Web.LibraryManager.Test
{
    internal static class TestUtils
    {
        public static void DeleteDirectoryWithRetries(string projectFolder)
        {
            int retries = 3;

            while (retries > 0)
            {
                try
                {
                    DeleteDirectory(projectFolder);
                }
                catch
                {
                }

                Thread.Sleep(10);
                retries--;
            }
        }

        public static void DeleteDirectory(string projectFolder)
        {
            if (!Directory.Exists(projectFolder))
            {
                return;
            }

            string[] files = Directory.GetFiles(projectFolder);
            string[] dirs = Directory.GetDirectories(projectFolder);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(projectFolder, true);

        }
        
    }
}
