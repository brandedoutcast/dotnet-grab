using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Grab
{
    static class Parser
    {
        static IProvider Provider;
        internal static void Parse(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            ShowVersion();
            RunProvider(args);
        }

        static void RunProvider(string[] packages)
        {
            Provider = new NuGetProvider();
            var Downloads = Provider.DownloadPackages(packages)
                                    .Select(async p =>
                                    {
                                        var (name, isSuccess, msg) = await p;
                                        var color = isSuccess ? ConsoleColor.Green : ConsoleColor.Red;
                                        Console.ForegroundColor = color;
                                        Console.WriteLine($"   {name} - {msg}");
                                    }).ToArray();
            Task.WaitAll(Downloads);
            Console.ResetColor();
            Console.WriteLine();
        }

        static void ShowVersion()
        {
            var versionString = Assembly.GetEntryAssembly()
                                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                        .InformationalVersion
                                        .ToString();

            Console.WriteLine($"{Constants.APP_NAME} v{versionString}");
            Console.WriteLine("----------------");
        }

        static void ShowHelp()
        {
            ShowVersion();
            Console.WriteLine($"\nUsage: {Constants.APP_NAME} [packages...]");

            Console.WriteLine("\npackages:");
            Console.WriteLine("   list of packages names (@ optional versions) to download");

            Console.WriteLine("\nEx:");
            Console.WriteLine($"   {Constants.APP_NAME} newtonsoft.json newtonsoft.json@6.0.7 entityframework@6.2.0");
            Console.WriteLine("\n   newtonsoft.json @ 12.0.3 - downloaded");
            Console.WriteLine("   newtonsoft.json @ 6.0.7 - downloaded");
            Console.WriteLine("   entityframework @ 6.2.0 - downloaded");
            
            Console.WriteLine("\npackages are saved under 'packages' in current directory");
            Console.WriteLine();
        }
    }
}