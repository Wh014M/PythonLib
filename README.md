# AutoPyMagic
:star: Automatic python and dependency installer on computer. For C#

``` C#
using System;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {

            // Load python 3.8.0
            var python = new Python.interpreter (
                version: "3.8.0"
            );

            // Install requirements from file
            python.InstallRequirements("requirements.txt");

            // Execute script
            var output = python.Run("script.py", "argv1 argv2 argv3");

            // Show output
            Console.WriteLine("\n" +
                $"OUTPUT:\n" +
                $"ExitCode : {output.ExitCode} \n" +
                $"Stdout   : {output.Stdout}   \n" +
                $"Stderr   : {output.Stderr}   \n"
            );

            // Wait
            Console.ReadLine();

        }
    }
}
```
