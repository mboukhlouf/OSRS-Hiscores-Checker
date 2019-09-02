using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

using Runescape.OSRS;
using CheckingLib;
using System.Threading.Tasks;

namespace OSRS_Hiscores_Checker
{
    class Program
    {
        private static Config config;
        private static Queue<User> users;
        private static Queue<WebProxy> proxies;

        private static bool finished = false;
        private static int threadsFinished = 0;

        private static Speedometer speedometer;
        private static int totalUsers;
        private static int errorsCount = 0;

        private static Step step = new Step()
        {
            Url = "https://secure.runescape.com/m=hiscore_oldschool/index_lite.ws?player=%USERNAME%",
            Method = "GET",
            Headers = new Dictionary<string, string>()
                {
                    { "User-Agent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36" },
                    { "Accept", "*/*" },
                    { "Accept-Language", "en-US,en;q=0.9" }
                }
        };

        static void Main(string[] args)
        {
            if (!File.Exists("config.json"))
            {
                Console.WriteLine("Coudln't find config.json file.");
                Config config = new Config();
                File.WriteAllText("config.json", config.ToJson());
                Console.WriteLine("A config.json file was just created, please fill it and run the program again.");
                Console.Read();
                return;
            }

            String configJson = File.ReadAllText("config.json");
            config = Config.ParseFromJson(configJson);

            Console.WriteLine(config);

            // Parsing users
            users = new Queue<User>();
            String[] combos = File.ReadAllLines(config.InputFile);
            foreach (String combo in combos)
            {
                if (combo.Trim() != "")
                {
                    try
                    {
                        users.Enqueue(new User(combo));
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            totalUsers = users.Count;
            Console.WriteLine($"Found {users.Count} users.");
            // Parsing proxies   
            if (config.ProxiesFile != "")
            {
                proxies = new Queue<WebProxy>();
                String[] _proxies = File.ReadAllLines(config.ProxiesFile);

                foreach (String proxy in _proxies)
                {
                    if (proxy.Trim() != "")
                    {
                        try
                        {
                            proxies.Enqueue(ParseProxy(proxy));
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (proxies == null)
                Console.WriteLine($"No proxy will be used.");
            else
            {
                Console.WriteLine($"Found {proxies.Count} proxies.");
            }

            if (config.Threads > combos.Length)
            {
                config.Threads = combos.Length;
            }

            Task[] tasks = new Task[config.Threads];
            Console.WriteLine($"Starting {config.Threads} threads.");
            speedometer = new Speedometer();
            speedometer.Start();
            UpdateStatus();

            for (int i = 0; i < config.Threads; i++)
            {
                tasks[i] = new Task(CheckAsync);
                tasks[i].Start();
            }

            while (!finished)
            {
                Console.Read();
            }
        }

        private static Object obj = new Object();

        private static async void CheckAsync()
        {
            while (users.Count > 0)
            {
                Checker checker = new Checker();

                User user;
                WebProxy proxy = null;
                lock (obj)
                {
                    user = users.Dequeue();
                    if (proxies != null)
                    {
                        proxy = proxies.Dequeue();
                        proxies.Enqueue(proxy);
                    }
                }

                checker.Proxy = proxy;

                try
                {
                    checker.Variables["USERNAME"] = user.Username;
                    String text = (await checker.ExecuteAsync(step)).Body;
                    if(text.Contains("Page not found"))
                    {
                        errorsCount++;
                        lock(obj)
                            File.AppendAllText(config.ErrorsFile, user.Username + ", error: Page not found." + "\n");
                    }
                    else
                    {
                        user.ParseSkillsFromText(text);
                        lock (obj)
                            File.AppendAllText(config.OutputFile, user.ToString() + "\n");
                    }
                }
                catch (Exception e)
                {
                    lock (obj)
                    {
                        errorsCount++;
                        File.AppendAllText(config.ErrorsFile, user.Username + ", error: " + e.Message + "\n");
                    }
                    users.Enqueue(user);
                }
                lock (obj)
                    speedometer.Progress++;

                UpdateStatus();
            }
            lock (obj)
            {
                threadsFinished++;
                if (threadsFinished == config.Threads)
                {
                    finished = true;
                    Console.WriteLine("Done!");
                }
            }
        }

        private static void UpdateStatus()
        {
            Console.Title = $"Progress: {speedometer.Progress}/{totalUsers} users | Speed: {speedometer.GetProgressPerHour()} users/hour | Errors: {errorsCount}";
        }

        private static WebProxy ParseProxy(String str)
        {
            String[] parts = str.Trim().Split(":");
            if (parts.Length != 2)
                throw new Exception("Proxy not in format.");
            String host = parts[0];
            int port;
            try
            {
                port = int.Parse(parts[1]);
            }
            catch (Exception)
            {
                throw new Exception("Problem in port.");
            }
            return new WebProxy(host, port);
        }
    }
}
