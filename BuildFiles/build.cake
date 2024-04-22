string build = Argument<string>("build", "All");
string target = Argument<string>("target", "Test");
string configuration = Argument<string>("configuration", "Debug");
string dockerWorkingDirectory = Argument<string>("dockerWorkingDirectory", "../");
string environmentArg = Argument<string>("environmentArg", "Development");

string apiOutput = Argument<string>("apiOutput", "../Api/Ecommerce.Api/bin/artifacts");
string workerOutput = Argument<string>("workerOutput", "../Infrastructure/Ecommerce.Worker/bin/artifacts");


Dictionary<string, string> args = new Dictionary<string, string>();

//====================================================================
// API args and build script call
//====================================================================
if (build.Equals("All") || build.Equals("API"))
{
	args.Clear();
	args.Add("target", target);
	args.Add("configuration", configuration);
	args.Add("apiOutput", apiOutput);
	args.Add("dockerWorkingDirectory", dockerWorkingDirectory);
	CakeExecuteScript("./API.cake", new CakeSettings { Arguments = args });
}


//====================================================================
// Blazor UI args and build script call
//====================================================================
if (build.Equals("All") || build.Equals("UI")) 
{
	if (target.Equals("Test"))
	{
    	target = Argument<string>("target", "Build");
	}

	args.Clear();
	args.Add("target", target);
	args.Add("configuration", configuration);
	args.Add("dockerWorkingDirectory", dockerWorkingDirectory);
	args.Add("environmentArg", environmentArg);
	CakeExecuteScript("./UI.cake", new CakeSettings { Arguments = args });
}

//====================================================================
// Worker args and build script call
//====================================================================
if (build.Equals("All") || build.Equals("Worker"))
{
	args.Clear();
	args.Add("target", target);
	args.Add("configuration", configuration);
	args.Add("workerOutput", workerOutput);
	args.Add("dockerWorkingDirectory", dockerWorkingDirectory);
	CakeExecuteScript("./Worker.cake", new CakeSettings { Arguments = args });
}