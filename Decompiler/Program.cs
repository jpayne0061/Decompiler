using System;
using System.Diagnostics;
using System.IO;

namespace Decompiler
{
    class Program
    {
        static readonly string OUT_PUT_DIRECTORY = Directory.GetCurrentDirectory();

        static readonly string EMPTY_CS_PROJ_CONTENTS = "<Project Sdk=\"Microsoft.NET.Sdk\">" +  
                                                        "\r\n<PropertyGroup>\r\n<TargetFramework>net5.0</TargetFramework>" + 
                                                        "\r\n</PropertyGroup>\r\n</Project>";

        static readonly string JUST_DECOMPILE_EXE_PATH = @"C:\Program Files (x86)\Progress\JustDecompile\Libraries\JustDecompile.exe";

        static void Main(string[] args)
        {
            string fullAssemblyPath = string.Empty;
            
            if(string.IsNullOrWhiteSpace(fullAssemblyPath))
            {
                throw new Exception("You must specify the location of the assembly you would like to create a project for");
            }
            
            string projectName = Path.GetFileNameWithoutExtension(fullAssemblyPath);

            //create folder to contain new project
            string newProjectDirectory = @$"{OUT_PUT_DIRECTORY}\{projectName}";
            Directory.CreateDirectory(newProjectDirectory);

            //run telerik just decompile command to generate .cs files inside of our newly created directory
            string decompileArgs = @$"/c /out:{newProjectDirectory} /target:{fullAssemblyPath}";
            ExecuteCommand(decompileArgs);


            //create .csproj file so we can add it to a solution
            string csProjFileName = $"{newProjectDirectory}\\{projectName}.csproj";
            File.WriteAllText(csProjFileName, EMPTY_CS_PROJ_CONTENTS);
            
            Console.WriteLine($"You can find your generated project here: {csProjFileName}");
        }

        static void ExecuteCommand(string args)
        {
            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.FileName = JUST_DECOMPILE_EXE_PATH;
            startinfo.Arguments = args;
            Process process = new Process();
            process.StartInfo = startinfo;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            string response = "";

            while (!process.StandardOutput.EndOfStream)
            {
                response += "\r\n" + process.StandardOutput.ReadLine();
            }

            process.WaitForExit();
        }

        
    }
}
