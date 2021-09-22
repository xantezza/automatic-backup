using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Client
{
    internal class PathsProcessor
    {
        private Config _config;
        private readonly Dictionary<string, long> _cache;

        public PathsProcessor(Config config)
        {
            _config = config;
            _cache = TryLoadCache(_config.CachePath);
        }

        public void GetPathsToFiles(out string[] pathsToFilesToSend, out string[] pathsToFilesToDelete)
        {
            var filesToSend = TryFindFilesToSend(_config.PathesToData, _cache);

            var pooledCache = TryConcatCache(_cache, filesToSend);

            //SaveCache(pooledCache, cachePath);

            pathsToFilesToSend = filesToSend.Keys.ToArray();

            pathsToFilesToDelete = TryFindFilesToDelete(_cache);
        }

        private Dictionary<string, long> TryLoadCache(string path)
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

        private void SaveCache(Dictionary<string, long> cache, string path)
        {
            var json = JsonConvert.SerializeObject(cache);
            File.WriteAllText(path, json);
        }

        private Dictionary<string, long> TryFindFilesToSend(string[] pathesToData, Dictionary<string, long> cache)
        {
            var filesToSend = new Dictionary<string, long>();

            foreach (var directoryPath in pathesToData)
            {
                try
                {
                    foreach (var filePath in Directory.GetFiles(directoryPath, "**", SearchOption.AllDirectories))
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
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }
            }

            return filesToSend;
        }

        private string[] TryFindFilesToDelete(Dictionary<string, long> cache)
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

        private Dictionary<string, long> TryConcatCache(Dictionary<string, long> cache, Dictionary<string, long> filesToSend)
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