using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ApiSchemaCLI
{
    class ProjectBuilder
    {
        public int Build()
        {
            var proccess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "publish",
                    WorkingDirectory = @"C:\Users\maksa\source\repos\LaboratoryWorkControl\BackEnd"
                }
            };
            proccess.Start();
            proccess.WaitForExit();
            return proccess.ExitCode;
        }
    }
}
