# Summary
A simple Cake build extension to increment the chosen index on AssemblyVersion and AssemblyFileVersion during the build process

# Usage
In the build script, add a reference to the tool:

```csharp
#r "tools/AssemblyUtilTool/BuildVersionExtension.dll"
/*
  This assumes that you have the DLL in a directory called AssemblyUtilTool under the tools directory
  the tools directory itself is a sub-directory of the directory containing the build script

  e.g. if the build.cake file is in C:\Projects\MyApp\build, then the DLL needs to be under
  C:\Projects\MyApp\build\tools\AssemblyUtilTool 
*/
```

Next, create a Cake Task to call the custom script alias exposed by this DLL:

```csharp
// MyApp.Service is a project that builds an EXE - this EXE will be versioned
var versionFileDir = Directory("../Source/MyApp.Service/Properties/").ToString() + "/AssemblyInfo.cs"; 

Task("Update-AssemblyInfo")
    .IsDependentOn("Restore-NuGet-Packages") // Restore-NuGet-Packages is an upstream task
    .Does(() =>
{
    // This call will increment the build part of AssemblyVersion and AssemblyFileVersion
    // E.g. version before: 1.0.0.0, version after: 1.0.1.0
    IncrementVersionNumber(versionFileDir, 3);
});
```

# References
Adapted from [this](http://www.codeproject.com/Articles/31236/How-To-Update-Assembly-Version-Number-Automaticall) work by Sergiy Korzh 