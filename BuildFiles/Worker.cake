using System.IO;

//====================================================================
// Arguments
//====================================================================
var target = Argument<string>("target", "Clean");
var configuration = Argument<string>("configuration", "Release");
var outputDirectory = Argument<string>("workerOutput", "./Infrastructure/Ecommerce.Worker/bin/artifacts");
var dockerHubUser = Argument<string>("dockerHubUser", "smithhe95");
var imageTag = Argument<string>("imageTag", "latest");
var dockerFilePath = Argument<string>("dockerFilePath", "./Infrastructure/Ecommerce.Worker/Dockerfile");
var dockerWorkingDirectory = Argument<string>("dockerWorkingDirectory", "../");

//====================================================================
// Variables
//====================================================================
var projectDirectory = "../Infrastructure/Ecommerce.Worker/";
var testDirectory = "../Tests/Ecommerce.UnitTests/";


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

Task("Restore Tests")
    .Does(() => {
	    DotNetRestore(testDirectory);
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

Task("Build Tests")
    .IsDependentOn("Restore Tests")
    .IsDependentOn("Build")
    .Does(() => {
        DotNetBuild(testDirectory, new DotNetBuildSettings
        {
            NoRestore = true,
            Configuration = configuration
        });
    });

Task("Test")
    .IsDependentOn("Build Tests")
    .Does(() => {
        DotNetTest(testDirectory, new DotNetTestSettings
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = configuration
        });
    });

Task("Docker Build")
    .IsDependentOn("Test")
    .Does(() => {
        Context.Environment.WorkingDirectory = dockerWorkingDirectory;

        var imageName = $"{dockerHubUser}/ecommerce-worker:{imageTag}";

        var arguments = new ProcessArgumentBuilder()
        .Append("build")
        .Append($"--file {dockerFilePath}")
        .Append("-t")
        .Append(imageName)
        .Append(".");

        var exitCode = StartProcess("docker", arguments.Render());
        
        if (exitCode != 0) {
            throw new Exception("Docker image build failed!");
        }
        else {
            Information("Docker image built successfully: {0}:{1}", imageName, imageTag);
        }
    });

Task("Docker Push")
    .IsDependentOn("Docker Build")
    .Does(() => {
        Context.Environment.WorkingDirectory = dockerWorkingDirectory;

        var imageName = $"{dockerHubUser}/ecommerce-worker:{imageTag}";

        var pushArguments = new ProcessArgumentBuilder()
            .Append("push")
            .Append(imageName);
            
        var pushExitCode = StartProcess("docker", pushArguments.Render());

        if (pushExitCode != 0) {
            throw new Exception("Failed to push Docker image to Docker Hub!");
        }
        else {
            Information("Docker image pushed successfully: {0}", imageName);
        }
    });

//====================================================================
// Local Publish Task
//====================================================================
Task("Publish")
    .IsDependentOn("Clean")
    .IsDependentOn("Test")
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