using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Client
{
    internal static class PathsProcessor
    {
        public static void GetPathsToFiles(out string[] pathsToFilesToSend, out string[] pathsToFilesToDelete)
        {
            var cache = LoadCache(Program.Config.CachePath);

            var filesToSend = FindFilesToSend(Program.Config.PathesToData, cache);

            var pooledCache = ConcatCache(cache, filesToSend);

            SaveCache(pooledCache, Program.Config.CachePath);

            pathsToFilesToSend = filesToSend.Keys.ToArray();

            pathsToFilesToDelete = FindFilesToDelete(cache);
        }

        private static Dictionary<string, long> LoadCache(string path)
        {
            if (File.Exists(path))
            {
                var jsonData = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<Dictionary<string, long>>(jsonData);
            }
            else
            {
                return null;
            }
        }

        private static void SaveCache(Dictionary<string, long> cache, string path)
        {
            var json = JsonConvert.SerializeObject(cache);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, json);
        }

        private static Dictionary<string, long> FindFilesToSend(string[] pathesToData, Dictionary<string, long> cache)
        {
            var filesToSend = new Dictionary<string, long>();

            foreach (var coreDirectoryPath in pathesToData)
            {
                var subdirs = Directory.GetDirectories(coreDirectoryPath);
                List<string> filePathes = new List<string>();

                foreach (var subdir in subdirs)
                {
                    try
                    {
                        Directory.GetFiles(subdir, "**", SearchOption.AllDirectories).ToList().ForEach(filePathes.Add);
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }
                }
                foreach (var filePath in filePathes)
                {
                    try
                    {
                        var fileStream = new FileStream(filePath, FileMode.Open);

                        if (cache != null && cache.ContainsKey(filePath))
                        {
                            if (cache[filePath] != fileStream.Length)
                            {
                                filesToSend.Add(filePath, fileStream.Length);
                            }
                        }
                        else
                        {
                            filesToSend.Add(filePath, fileStream.Length);
                        }
                        fileStream.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }
                }
                
            }

            return filesToSend;
        }

        private static string[] FindFilesToDelete(Dictionary<string, long> cache)
        {
            if (cache != null)
            {
                var filesToDelete = new List<string>();

                foreach (var file in cache.Keys.ToList())
                {
                    if (!File.Exists(file))
                    {
                        filesToDelete.Add(file);
                        cache.Remove(file);
                    }
                }
                return filesToDelete.ToArray();
            }
            return null;
        }

        private static Dictionary<string, long> ConcatCache(Dictionary<string, long> cache, Dictionary<string, long> filesToSend)
        {
            if (cache != null)
            {
                return cache.Concat(filesToSend.Where(x => !cache.Keys.Contains(x.Key))).ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                return filesToSend;
            }
        }
    }
}