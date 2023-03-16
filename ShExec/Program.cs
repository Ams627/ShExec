using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WPath;

namespace ShExec
{
    internal record App(string AppName, string Path);

    internal class Program
    {
        private const string AppKeys = @"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    PrintUsageAndExit();
                }
                var normalArgs = args.Where(x => x[0] != '-').ToArray();
                var optionArgs = args.Where(x => x[0] == '-').SelectMany(x => x.Skip(1)).ToHashSet();



                var appDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (var key in RegLib.GetSubKeyNames(AppKeys))
                {
                    // get app name:
                    var k = Path.GetFileNameWithoutExtension(key);
                    var ke = Path.GetFileName(key);

                    // get app path:
                    var value = RegLib.GetDefaultValue(key);
                    appDict[k] = value;
                }

                if (optionArgs.Contains('l'))
                {
                    foreach (var entry in appDict)
                    {
                        Console.WriteLine($"{entry.Key} -> {entry.Value}");
                    }

                    Environment.Exit(0);
                }

                if (optionArgs.Contains('f'))
                {
                    foreach (var entry in appDict)
                    {
                        Console.WriteLine($"{entry.Key.ToLower()} {{");
                        Console.WriteLine($"    shexec {entry.Key.ToLower()}");
                        Console.WriteLine("}");
                    }

                    Environment.Exit(0);
                }

                if (!appDict.TryGetValue(args[0], out string appPath))
                {
                    appPath = args[0];
                }

                var psi = new ProcessStartInfo
                {
                    FileName = appPath,
                    UseShellExecute = true,
                    Arguments = string.Join("U+0020", args.Skip(1))
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine($"{progname} Error: {ex.Message}");
            }

        }

        private static void PrintUsageAndExit()
        {
            Console.WriteLine($"Shexec: start Windows programs from git-bash");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("    shexec <program> [<arguments>]");
            Console.WriteLine("    shexec -l");
            Console.WriteLine("    shexec -f");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine($"    -l list all programs in {AppKeys}");
            Console.WriteLine($"    -f generate bash functions for all programs in {AppKeys}");
        }
    }
}
