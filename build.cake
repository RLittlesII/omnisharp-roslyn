// ADDINS

// TOOLS
// #tool "xunit.runner.console"

// ARGUMENTS
var target = Argument("target", "Default");
var configuration = Argument("configuration","Release");

// ENVIRONMENT
var isAppVeyorBuild = AppVeyor.IsRunningOnAppVeyor;
var isUnix = Machine.IsUnix();
var buildEnvironment = EnvironmentVariable("TRAVIS_OS_NAME");
var homeDirectory = EnvironmentVariable("HOME");
var dnxRuntime = "1.0.0-rc2-16444";
var workingDirectory = System.IO.Directory.GetCurrentDirectory();

// VARIABLES
var artifactsFolder = Directory("../artifacts");
var targetFrameworks = new[] { "dnx451", "dnxcore50" };
var configurations = new[] { target };
var solution = "../src/*";
// var solution = File("../omnisharp.json");

// FUNCTIONS
Action<string> SetRuntime = (runtime) =>
{
    var processSettings = new ProcessSettings
    {
        Arguments = new ProcessArgumentBuilder().Append("use {0} -r {1}", dnxRuntime, runtime)
    };
    Information("Setting dnx run time: {0}", runtime);
    using(var process = StartAndReturnProcess("dnvm.sh", processSettings))
    {
        process.WaitForExit();
    }
};

Action<string, string> Test = (project, runtime) =>
{
    var processSettings = new ProcessSettings
    {
        Arguments = new ProcessArgumentBuilder().Append(string.Format("test -parallel none", MakeAbsolute(Directory(project))))
    };
    
    SetRuntime(runtime);
    
    Information("Setting working directory: {0}", MakeAbsolute(Directory(project)));
    System.IO.Directory.SetCurrentDirectory(project);
    
    Information("Running tests on project: {0}", project);
    using(var process = StartAndReturnProcess("dnx", processSettings))
    {
        process.WaitForExit();
    }
    Information("Setting working directory: {0}", workingDirectory);
    System.IO.Directory.SetCurrentDirectory(workingDirectory);
};

Action<string, string, string, string, string> Publish = (project, version, runtime, destination, tar) => 
{
    // Set the dnvm version and runtime
    
    // run the dnu publish --configuration Release --no-source --quiet --runtime active --out $_dest
    var dnuBuildSettings = new DNUBuildSettings
    {
        Frameworks = new[] { runtime },
        Configurations = new[] { target },
        OutputDirectory = destination,
        Quiet = true
    };
    
    DNUBuild(project, dnuBuildSettings);
    
    //TODO: Zip Files
};

Action<string> Pack = (project) =>
{
    var dnuPackSettings = new DNUPackSettings
    {
        Frameworks = targetFrameworks,
        Configurations = configurations,
        OutputDirectory = string.Format("{0}/nuget", artifactsFolder),
        Quiet = true
    };
    
    Information("DNU Restore: {0}", project);
    DNURestore(project);
    
    Information("DNU Pack: {0}", project);
    DNUPack(project, dnuPackSettings);
};

// TASKS
Task("Environment")
    .Does(() =>
    {
        // Check for Omnisharp environment
        // build array of needed variables
        // Install Omnisharp environment
        // OMNISHARP_VERSION
        Information("Setting Environment Variable");
        
        // SetEnvironmentVariable("TESTCAKE", "SUCCESS!!!", EnvironmentVariableTarget.Machine);
        // var env = EnvironmentVariable("TESTCAKE");
        // Information(env);
    });

Task("Download")
    .Does(() =>
    {
        // Download Dependencies
        // dnvm
        // xunit runner
        
        Information("Downloading the dnvm installation script");
        if(isUnix)
        {
            DownloadFile("https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.sh", "./dnvminstall.sh");
        }
    });

Task("Install")
    .IsDependentOn("Download")
    .Does(() => 
    {
        // Install the dnx
        StartAndReturnProcess("./dnvminstall.sh");
        
        // Install the dnu
        
        // Install mono run time
    });
    
Task("Clean")
    //.IsDependentOn("Install")
    .IsDependentOn("Download")
    .Does(() => 
    {
        // CleanDirectories(artifactsFolder);
    });
    
Task("Restore")
    //.IsDependentOn("Install")
    .IsDependentOn("Download")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        NuGetRestore(solution);
    });

Task("Test")
    //.IsDependentOn("Download")
    //.IsDependentOn("Install")
    //.IsDependentOn("Clean")
    //.IsDependentOn("Restore")
    .Does(() =>
    {       
        // Get project.json files from tests directory
        // foreach over project array
        // deserialize to dynamic type
        // execute test
        
        foreach(var folder in System.IO.Directory.GetDirectories("./tests"))
        {
            Test(folder, "coreclr");
            Test(folder, "mono");
        }
    });
    
Task("Build")
    .Does(() =>
    {
        
    });

Task("Patch")
    .Does(() => 
    {
        
    });

Task("Package")
    .IsDependentOn("Test")
    .Does(() => 
    {
        
    });

Task("Publish")
    .IsDependentOn("Test")
    .Does(() => 
    {
        
    });

Task("Default")
    .IsDependentOn("Build")
    .Does(() => 
    {
        
    });
    
RunTarget(target);