# AutoPyMagic
:star: Automatic python and dependency installer on computer. For C#

``` C#
namespace Application
{
    class Program
    {
        static void Main()
        {
            // If python is not installed in system
            if (!python.magic.isInstalled())
                // We install it.
                python.magic.install("3.8.0"); // 3.5.0, 3.6.0, 3.7.0, 3.8.0
                
            // Install all modules from file
            python.magic.installRequirements("requirements.txt");
        }
    }
}

```
