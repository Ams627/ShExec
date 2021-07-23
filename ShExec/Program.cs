using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace ShExec
{
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    throw new Exception("You must supply a filename to pass to ShellExecute");
                }
                var psi = new ProcessStartInfo
                {
                    FileName = args[0],
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
