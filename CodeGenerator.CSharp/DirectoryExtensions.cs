﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace LateBindingApi.CodeGenerator.CSharp
{
    public static class DirectoryExtensions
    {
        public static void CopyTo(string source, string target, bool overwiteFiles = true)
        {
            CopyTo(new DirectoryInfo(source), new DirectoryInfo(target), overwiteFiles);
        }

        public static void CopyTo(this DirectoryInfo source, string target, bool overwiteFiles = true)
        {
            CopyTo(source, new DirectoryInfo(target), overwiteFiles);
        }

        public static void CopyTo(this DirectoryInfo source, DirectoryInfo target, bool overwiteFiles = true)
        {
            if (!source.Exists) return;
            if (!target.Exists) target.Create();

            Parallel.ForEach(source.GetDirectories(), (sourceChildDirectory) =>
                CopyTo(sourceChildDirectory, new DirectoryInfo(Path.Combine(target.FullName, sourceChildDirectory.Name))));

            foreach (var sourceFile in source.GetFiles())
                sourceFile.CopyTo(Path.Combine(target.FullName, sourceFile.Name), overwiteFiles);
        }
    }

    public static class DirectoryEx
    {
        public static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
