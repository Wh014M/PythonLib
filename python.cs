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
            
                return 32;
            else
                return 64;
        }

        // Load & Extract portable python
        private string LoadInterpreter()
        {
            // Path
            string pythonDownloadUrl = $"https://github.com/oswjk/portablepython/releases/download/release-4/Python-{version}-{architecture}.zip";
            string pythonInstallPath = Path.GetTempPath();
            string pythonArchivePath = pythonInstallPath + $"\\python-{version}-{architecture}.zip";
            string interpreter = pythonInstallPath + $"\\Python-{version}-{architecture}\\python.exe";
            // If python already installed to temp dir
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
                try {
                    client.DownloadFile(pythonDownloadUrl, pythonArchivePath);
                } catch (WebException error) {
                    // If error 404
                    if (error.Message.Contains("404"))
                        throw new FileNotFoundException("The specified version of python was not found.");
                    // If other error
                    else
                        throw new WebException(error.Message);
                    
                }
            }
            // Extract python
            ZipFile.ExtractToDirectory(pythonArchivePath, pythonInstallPath);
            // Clean
            File.Delete(pythonArchivePath);
            // Return path
            return interpreter;
        }

        // Delete interpreter
        public void DeleteInterpreter()
        {
            string interpreter = LoadInterpreter();
            string pythonDir = Path.GetDirectoryName(interpreter);
            Directory.Delete(pythonDir, recursive: true);
        }


        // Install requirements from file.
        public void InstallRequirements(string file)
        {
            // If requirements file not exists
            if(!File.Exists(file))
                throw new FileNotFoundException($"Requirements file {file} not found!");
            
            
            // Run pip install command
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
                    throw new Exception($"Failed to install requirements, exit code: {process.ExitCode}");
            }
        }

        // Run python script
        public output Run(string script, string args = "")
        {
            // If script not exists
            if (!File.Exists(script))
                throw new FileNotFoundException($"Script {script} not found!");
            

            // Run script
            using (var process = new Process())
            {
                // Process info
                ProcessStartInfo StartInfo = new ProcessStartInfo
                {
                    FileName = this.LoadInterpreter(),
                    WorkingDirectory = Path.GetDirectoryName(script),
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
                // Return output
                return output;
            }
        }
    
    


    }
}
