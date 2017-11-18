#load "./buildconfig.cake"

var config = BuildConfig.Create(Context, BuildSystem);

Information("SrcDir: " + config.SrcDir);
Information("OutDir: " + config.OutDir);
Information("SemVer: " + config.SemVer);
Information("IsDefaultBranch: " + config.IsDefaultBranch);
Information("BuildVersion: " + config.BuildVersion);
Information("BuildProfile: " + config.BuildProfile);
Information("IsTeamCityBuild: " + config.IsTeamCityBuild);

Task("Default")
    .IsDependentOn("InitOutDir")
    .IsDependentOn("Restore")
    .IsDependentOn("AssemblyVersion")
    .IsDependentOn("Build");

Task("CI")
    .IsDependentOn("Default")
    .IsDependentOn("Pack");
/********************************************/
Task("InitOutDir").Does(() => {
    EnsureDirectoryExists(config.OutDir);
    CleanDirectory(config.OutDir);
});

Task("Restore").Does(() => {
    foreach(var sln in GetFiles(config.SrcDir + "*.sln")) {
        NuGetRestore(sln);
    }
});

Task("AssemblyVersion").Does(() => {
    var file = config.SrcDir + "GlobalAssemblyVersion.cs";
    var info = ParseAssemblyInfo(file);
    CreateAssemblyInfo(file, new AssemblyInfoSettings {
        Version = config.BuildVersion,
        InformationalVersion = config.SemVer
    });
});
    
Task("Build").Does(() => {
    foreach(var sln in GetFiles(config.SrcDir + "*.sln")) {
        MSBuild(sln, new MSBuildSettings {
            Verbosity = Verbosity.Minimal,
            ToolVersion = MSBuildToolVersion.VS2017,
            Configuration = config.BuildProfile,
            PlatformTarget = PlatformTarget.MSIL
        }
        .SetMSBuildPlatform(MSBuildPlatform.Automatic)
        .WithTarget("Rebuild")
        .WithWarningsAsError());
    }
});

Task("Pack").Does(() => {
    NuGetPack(GetFiles(config.SrcDir + "*.nuspec"), new NuGetPackSettings {
        Version = config.SemVer,
        BasePath = config.SrcDir,
        OutputDirectory = config.OutDir,
        Properties = new Dictionary<string, string>
        {
            {"Configuration", config.BuildProfile}
        }
    });
});

RunTarget(config.Target);