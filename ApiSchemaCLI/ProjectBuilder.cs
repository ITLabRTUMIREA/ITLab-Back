using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ApiSchemaCLI
{
    class ProjectBuilder
    {
        private readonly string workDir;
        public ProjectBuilder(string workDir)
        {
            this.workDir = workDir;
        }
        public int Build()
        {
            var proccess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "publish --no-restore --self-contained --runtime win7-x64",
                    WorkingDirectory = workDir
                }
            };
            proccess.Start();
            proccess.WaitForExit();
            return proccess.ExitCode;
        }
    }
}
