using CK.Core;
using CK.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kuinox.TypedCLI.Dotnet
{
    public static partial class Dotnet
    {
        public static class Sln
        {
            /// <returns>true if success, false otherwise.</returns>
            public static Task<bool> New( IActivityMonitor m, string workingDirectory = "", string? slnName = null )
                => CLIRunner.RunAsync( m, "dotnet", new string?[]
                {
                    "new sln",
                    "-n ".Arg(slnName)
                }, workingDirectory );

            public static async Task<IEnumerable<string>?> List( IActivityMonitor m, string workingDirectory = "", string? slnName = null )
            {
                (int exitCode, IEnumerable<string> output) = await CLIRunner.RunAndGetLinesOutput( m, "dotnet", new[] { "sln list", slnName }, workingDirectory );
                if( exitCode != 0 ) return null;
                return output.Skip( 2 );
            }

            public static Task<bool> Add( IActivityMonitor m, IEnumerable<string> projectsPath, string workingDirectory = "", string? slnName = null, string? solutionFolder = null, bool inRoot = false )
            {
                List<string?> args = new()
                {
                    "sln",
                    slnName,
                    "add",
                    "-s ".Arg( solutionFolder ),
                    inRoot ? "--in-root" : null
                };
                args.AddRange( projectsPath );
                return CLIRunner.RunAsync( m, "dotnet", args, workingDirectory );
            }

            public static Task<bool> Add( IActivityMonitor m, string projectsToAdd, string workingDirectory = "", string? slnName = null, string? solutionFolder = null, bool inRoot = false )
                => Add( m, new string[] { projectsToAdd }, workingDirectory, slnName, solutionFolder, inRoot );

            public static Task<bool> Remove( IActivityMonitor m, IEnumerable<string> projectsPath, string workingDirectory = "", string? slnName = null )
            {
                List<string?> args = new()
                {
                    "sln",
                    slnName,
                    "remove"
                };
                args.AddRange( projectsPath );
                return CLIRunner.RunAsync( m, "dotnet", args, workingDirectory );
            }

            public static Task<bool> Remove( IActivityMonitor m, string projectPath, string workingDirectory = "", string? slnName = null )
                => Remove( m, new[] { projectPath }, workingDirectory, slnName );
        }
    }
}
