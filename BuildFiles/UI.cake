//====================================================================
// Arguments
//====================================================================
var target = Argument<string>("target", "Clean");
var configuration = Argument<string>("configuration", "Release");

//====================================================================
// Variables
//====================================================================
var projectDirectory = "../UI/Ecommerce.UI";
var outputDirectory = "../UI/Ecommerce.UI/artifacts";

//====================================================================
// Build and Test Tasks
//====================================================================
Task("Clean")
    .Does(() => {
        CleanDirectory(outputDirectory);
    });;

Task("Restore")
    .Does(() => {
        DotNetRestore(projectDirectory);
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetBuild(projectDirectory, new DotNetBuildSettings
        {
            NoRestore = true,
            Configuration = configuration
        });
    });

//====================================================================
// Local Publish Task
//====================================================================
Task("Publish")
    .IsDependentOn("Clean")
    .Does(() => {
        DotNetPublish(projectDirectory, new DotNetPublishSettings
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = configuration,
            OutputDirectory = outputDirectory
        });
    });

Console.WriteLine($"Running Target: {target}");
Console.WriteLine("================================================================================");
RunTarget(target);