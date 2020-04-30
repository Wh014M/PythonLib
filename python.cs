/* 
       >
       | Author    : LimerBoy
       | Name      : AutoPyMagic
       | Github    : https:github.com/LimerBoy
       >

       * EXAMPLE USAGE:
       * 
       if (!python.magic.isInstalled())
           python.magic.install("3.8.0");

       python.magic.installRequirements("requirements.txt");

*/


using System;
using System.IO;
using System.Net;
using System.Diagnostics;


namespace python
{

    internal sealed class magic
    {


        /*
         * Download and install python to system.
         * Versions available:
         * 3.5.0
         * 3.6.0
         * 3.7.0
         * 3.8.0
         */
        public static void install(string version)
        {
            // If python is installed
            if (isInstalled())
            {
                Console.WriteLine("python is already installed in the system");
                return;
            }
            // SSL
            ServicePointManager.SecurityProtocol = (
                SecurityProtocolType.Ssl3  |
                SecurityProtocolType.Tls   |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12
            );
            // Path
            string pythonDownloadUrl = $"https://www.python.org/ftp/python/{version}/python-{version}.exe";
            string pythonInstallerPath = Path.GetTempPath() + "\\python.exe";
            string pythonInstallPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\python";
            // Download python installer
            using (var client = new WebClient())
            {
                Console.WriteLine($"Downloading python {version}...");
                try {
                    client.DownloadFile(pythonDownloadUrl, pythonInstallerPath);
                } catch (WebException error) {
                    // If error 404
                    if (error.Message.Contains("404")) {
                        Console.WriteLine("The specified version of python was not found.");
                        return;
                    // If other error
                    } else {
                        Console.WriteLine("Bad internet connection. Try again.");
                        return;
                    }
                }
            }
            // Run python installer
            using (var process = new Process())
            {
                Console.WriteLine("Installing...");
                // Process info
                ProcessStartInfo StartInfo = new ProcessStartInfo
                {
                    FileName = pythonInstallerPath,
                    Arguments = $"/quiet TargetDir={pythonInstallPath} PrependPath=1 Include_test=0 Include_pip=1 AssociateFiles=1 InstallAllUsers=0",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                // Start process
                process.StartInfo = StartInfo;
                process.Start();
                process.WaitForExit();
                // Show info
                if (process.ExitCode == 0)
                    Console.WriteLine($"Python-{version} is installed.");
                else
                    Console.WriteLine("Failed to install, exit code: " + process.ExitCode);

            }
            // Remove downloaded installer
            Console.WriteLine("Cleaning...");
            File.Delete(pythonInstallerPath);
        }

        // Install requirements from file.
        public static void installRequirements(string file)
        {
            // Run pip install command
            using (var process = new Process())
            {
                // Process info
                ProcessStartInfo StartInfo = new ProcessStartInfo
                {
                    FileName = "python.exe",
                    Arguments = $"-m pip install -r {file}",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                // Start process
                process.StartInfo = StartInfo;
                process.Start();
                process.WaitForExit();
                // Return values
                if (process.ExitCode == 0)
                    Console.WriteLine($"All dependencies are installed from the {file} file.");
                else
                    Console.WriteLine("Failed to install requirements, exit code: " + process.ExitCode);
            }
        }
    
    

        // Check if python is installed.
        public static bool isInstalled()
        {
            // Run python --help command
            using (var process = new Process())
            {
                // Process info
                ProcessStartInfo StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c python.exe --version",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                // Start process
                process.StartInfo = StartInfo;
                process.Start();
                process.WaitForExit();
                // Return values
                if (process.ExitCode == 0)
                    return true;
                else
                    return false;
            }
        }

        



    }
}
