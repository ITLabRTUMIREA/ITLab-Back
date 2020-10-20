using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using _build.shared;
using Nuke.Common;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Publish);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Packs version postfix")]
    readonly string PackVersionPostfix;

    [Parameter("Build id")]
    readonly string BuildId = "local build";

    [Solution] 
    readonly Solution Solution;

    readonly AbsolutePath deployTarget = RootDirectory / "deploy" / "ITLab-Back";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(deployTarget);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Publish => _ => _
        .DependsOn(Compile, Clean)
        .Triggers(FillBiuldInfo)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(Solution.GetProject("BackEnd"))
                .SetOutput(deployTarget)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });
    Target FillBiuldInfo => _ => _
        .Requires(() => BuildId)
        .Executes(() =>
        {
            System.IO.File.WriteAllText(deployTarget / "build.json", JsonSerializer.Serialize(new BuildInformation
            {
                BuildDateString = DateTimeOffset.UtcNow,
                BuildId = BuildId
            }));
        });
    Target PackDb => _ => _
        .Requires(() => PackVersionPostfix)
        .Executes(() =>
        {
            Solution.GetProjects("Database$|Models$|Extensions$")
                .ForEach(project =>
                {
                    DotNetPack(p => p
                        .SetProject(project.Path)
                        .SetConfiguration(Configuration)
                        .SetOutputDirectory("packs")
                        .SetVersion($"1.0.0-CI-{PackVersionPostfix}"));
                });
        });
}
