# TypedCLI
Simple wrapper around various CLI.

Currently supporting most of the dotnet CLI commands, except msbuild related stuff.
Supporting a few important npm commands.

# Documentation
Almost none currently. The api match 1 to 1 the CLI, if not, it's a bug.
PR are welcome :p.

# The code quality
Depends of the taste:  
```csharp
public static Task<bool> RunScript( IActivityMonitor m, string command, string? args = null, string workingDirectory = "", bool silent = false )
    => CLIRunner.RunAsync( m, "npm", new string?[]
    {
        "run-script",
        silent ? "--silent" : null,
        command,
        "-- ".Arg(args)
    }, workingDirectory );
```
At least it's concise and go the the point. 
