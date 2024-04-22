string build = Argument<string>("build", "All");
string target = Argument<string>("target", "Test");
string configuration = Argument<string>("configuration", "Debug");
var dockerWorkingDirectory = Argument<string>("dockerWorkingDirectory", "../");
var apiOutput = Argument<string>("apiOutput", "../Api/Ecommerce.Api/bin/artifacts");
var environmentArg = Argument<string>("environmentArg", "Development");

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