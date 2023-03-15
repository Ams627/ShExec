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
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    throw new Exception("You must supply a filename to pass to ShellExecute");
                }

                var appKeys = @"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";

                var appDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (var key in RegLib.GetSubKeyNames(appKeys))
                {
                    // get app name:
                    var k = Path.GetFileNameWithoutExtension(key);
                    var ke = Path.GetFileName(key);

                    // get app path:
                    var value = RegLib.GetDefaultValue(key);
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
    }
}
