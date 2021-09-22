using Newtonsoft.Json;
using System;
using System.IO;
using System.Timers;

namespace Client
{
    internal class App
    {
        private Timer _Timer;
        private readonly Config _config;

        private static void Main(string[] args)
        {
            var app = new App();
        }

        public App()
        {
            _config = TryLoadConfig("Config.json");
            SetTimer(_config.TimeStampForAutoCheckInSeconds);
            Console.ReadKey();
            _Timer.Stop();
            _Timer.Dispose();
        }

        private void SetTimer(double time)
        {
            _Timer = new Timer(time * 1000);
            _Timer.Elapsed += OnTimedEvent;
            _Timer.AutoReset = true;
            _Timer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            SynchronyzeFiles();
        }

        private void SynchronyzeFiles()
        {
            var pathsProcessor = new PathsProcessor(_config);

            pathsProcessor.GetPathsToFiles(out var pathsToFilesToSend, out var pathsToFilesToDelete);

            var clientNetworkProcessor = new ClientNetworkProcessor(_config, new LocalFakeServer());

            clientNetworkProcessor.TrySendData(pathsToFilesToSend, pathsToFilesToDelete);
        }

        private Config TryLoadConfig(string path)
        {
            if (File.Exists(path))
            {
                var jsonData = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<Config>(jsonData);
            }
            else
            {
                File.Create(path);
                Console.WriteLine("Config.json has been created, configure it and restart application.");
                Console.ReadKey();
                throw new Exception();
            }
        }
    }
}