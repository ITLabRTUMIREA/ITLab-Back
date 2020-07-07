var target = Argument("target", "PublishBe");
var configuration = Argument("configuration", "Release");

var bepublishDir = "deploy/ITLab-Back";
var beProject = "BackEnd/BackEnd.csproj";

Setup(ctx =>
{
   CleanDirectory(bepublishDir);
});

Teardown(ctx =>
{
});


Task("RestoreSolution")
   .Does(() => 
{
   DotNetCoreRestore();
});

Task("BuildBe")
   .IsDependentOn("RestoreSolution")
   .Does(() =>
{
   var settings = new DotNetCoreBuildSettings
   {
      Configuration = configuration
   };
   DotNetCoreBuild(beProject, settings);
});

Task("PublishBe")
   .IsDependentOn("BuildBe")
   .Does(() =>
{
   var settings = new DotNetCorePublishSettings
   {
      Configuration = configuration,
      OutputDirectory = bepublishDir
   };

   DotNetCorePublish(beProject, settings);
});

RunTarget(target);