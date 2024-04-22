//====================================================================
// Arguments
//====================================================================
var target = Argument<string>("target", "Clean");
var configuration = Argument<string>("configuration", "Release");
var dockerHubUser = Argument<string>("dockerHubUser", "smithhe95");
var imageTag = Argument<string>("imageTag", "latest");
var dockerFilePath = Argument<string>("dockerFilePath", "./UI/Ecommerce.UI/Dockerfile");
var dockerWorkingDirectory = Argument<string>("dockerWorkingDirectory", "../");
var environmentArg = Argument<string>("environmentArg", "Development");

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

Task("Docker Build")
    .IsDependentOn("Build")
    .Does(() => {
        Context.Environment.WorkingDirectory = dockerWorkingDirectory;

        var imageName = $"{dockerHubUser}/ecommerce-ui:{imageTag}";

        var arguments = new ProcessArgumentBuilder()
        .Append("build")
        .Append($"--file {dockerFilePath}")
        .Append($"")
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

        var imageName = $"{dockerHubUser}/ecommerce-ui:{imageTag}";

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