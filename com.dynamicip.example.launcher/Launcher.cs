using System;
using System.Diagnostics;
using System.IO;

//
// This helper utility launches the web scraper inside a docker container. We use C# rather than
// shell scripts to make the utility cross-platform and easy-to-run from the IDE.
//
// This is NOT INTENDED to be used in production. Instead, you should use the docker image and
// parameters directly in your container orchestrator, e.g. Kubernetes, Marathon, AWS ECS, etc.
//
namespace com.dynamicip.example.launcher
{
    class Program
    {
        private static readonly string launcherDir = "com.dynamicip.example.launcher";

        static void Main(string[] args)
        {
            try 
            {
                RunProcess("docker", "build -t dynamicip-chrome-csharp .");
                RunProcess("docker", "run -v /dev/shm:/dev/shm dynamicip-chrome-csharp");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static string GetRepositoryDirectory()
        {
            var currentDir = Directory.GetCurrentDirectory();
            if (currentDir.EndsWith(launcherDir))
            {
                return Directory.GetParent(currentDir).FullName;
            }

            if (Directory.Exists(Path.Combine(currentDir, launcherDir)))
            {
                return currentDir;
            }

            throw new Exception($"Current directory must be either repository root or launcher project ({launcherDir})");
        }

        private static void RunProcess(string command, string arguments)
        {
            var processInfo = new ProcessStartInfo
            {
                CreateNoWindow   = true,
                FileName         = command,
                Arguments        = arguments,
                WorkingDirectory = GetRepositoryDirectory()
            };

            using (var process = new Process())
            {
                process.StartInfo = processInfo;
                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"Failed to run: {command} {arguments}\nPlease see error log above.");
                }
            }
        }
    }
}
