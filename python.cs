/* 
       >
       | Author    : LimerBoy
       | Name      : AutoPyMagic
       | Github    : https://github.com/LimerBoy
       >

*/


using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.IO.Compression;

namespace Python
{

    // Output
    internal sealed class output
    {
        public int ExitCode;
        public string Stdout;
        public string Stderr;
    }

    // Python
    internal sealed class interpreter
    {

        public string version;
        public int architecture;
        // Constructor
        public interpreter(string version)
        {
            this.version = version;
            this.architecture = GetArchitecture();
            this.LoadInterpreter();
        }

        // Get cpu architecture
        private int GetArchitecture()
        {
            if (Microsoft.Win32.Registry.LocalMachine
                .OpenSubKey(@"HARDWARE\Description\System\CentralProcessor\0")
                .GetValue("Identifier")
                .ToString()
                .Contains("x86"))
            {
                return 32;
            } else {
                return 64;
            }
        }

        // Load & Extract portable python
        private string LoadInterpreter()
        {
            // Path
            string pythonDownloadUrl = $"https://github.com/oswjk/portablepython/releases/download/release-4/Python-{version}-{architecture}.zip";
            string pythonInstallPath = Path.GetTempPath();
            string pythonArchivePath = pythonInstallPath + $"\\python-{version}-{architecture}.zip";
            string interpreter = pythonInstallPath + $"\\Python-{version}-{architecture}\\python.exe";
            // If python installed to temp dir
            if (File.Exists(interpreter))
                return interpreter;
            // SSL
            ServicePointManager.SecurityProtocol = (
                SecurityProtocolType.Ssl3  |
                SecurityProtocolType.Tls   |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12
            );
            // Download python installer
            using (var client = new WebClient())
            {
                Console.WriteLine("Downloading...");
                try {
                    client.DownloadFile(pythonDownloadUrl, pythonArchivePath);
                } catch (WebException error) {
                    // If error 404
                    if (error.Message.Contains("404"))
                    {
                        Console.WriteLine("The specified version of python was not found.");
                        Environment.Exit(1);
                        // If other error
                    } else {
                        Console.WriteLine(error.Message);
                        Environment.Exit(1);
                    }
                }
            }
            // Extract python
            Console.WriteLine("Extracting...");
            ZipFile.ExtractToDirectory(pythonArchivePath, pythonInstallPath);
            // Clean
            Console.WriteLine("Cleaning...");
            File.Delete(pythonArchivePath);
            // Return path
            return interpreter;
        }

        // Delete interpreter
        public void DeleteInterpreter()
        {
            Console.WriteLine("Deleting interpreter...");
            string interpreter = LoadInterpreter();
            string pythonDir = Path.GetDirectoryName(interpreter);
            Directory.Delete(pythonDir, recursive: true);
            Console.WriteLine("Interpreter removed");
        }


        // Install requirements from file.
        public void InstallRequirements(string file)
        {
            // If requirements file not exists
            if(!File.Exists(file)) {
                Console.WriteLine($"Requirements file {file} not found!");
                return;
            }

            // Run pip install command
            Console.WriteLine("Installing requirements...");
            using (var process = new Process())
            {
                // Process info
                ProcessStartInfo StartInfo = new ProcessStartInfo
                {
                    FileName = this.LoadInterpreter(),
                    Arguments = $"-m pip install -r {file}",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                // Start process
                process.StartInfo = StartInfo;
                process.Start();
                process.WaitForExit();
                // If error
                if (process.ExitCode != 0)
                    Console.WriteLine($"Failed to install requirements, exit code: {process.ExitCode}");
            }
        }

        // Run python script
        public output Run(string script, string args = "")
        {
            // If script not exists
            if (!File.Exists(script))
            {
                Console.WriteLine($"Script {script} not found!");
                return null;
            }

            // Run script
            Console.WriteLine($"Running script {script}...");
            using (var process = new Process())
            {
                // Process info
                ProcessStartInfo StartInfo = new ProcessStartInfo
                {
                    FileName = this.LoadInterpreter(),
                    Arguments = $"{script} {args}",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                // Start process
                process.StartInfo = StartInfo;
                process.Start();
                process.WaitForExit();
                // Read
                var output = new output();
                output.Stdout = process.StandardOutput.ReadToEnd();
                output.Stderr = process.StandardError.ReadToEnd();
                output.ExitCode = process.ExitCode;
                Console.WriteLine($"Script {script} stopped working.");
                // Return output
                return output;
            }
        }
    
    


    }
}
