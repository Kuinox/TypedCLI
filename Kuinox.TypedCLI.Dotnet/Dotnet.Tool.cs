using CK.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuinox.TypedCLI.Dotnet
{
    public static partial class Dotnet
    {
        public static class Tool
        {
            public static Task<bool> Install( IActivityMonitor m, string packageName, string workingDirectory = "", bool global = false, string? addSource = null,
                string? configFile = null, string? framework = null, Verbosity? verbosity = null, string? version = null )
                => CLIRunner.RunAsync( m, "dotnet", new List<string?>()
                {
                    "tool install",
                    packageName,
                    global ? "-g" : null,
                    "--add-source ".Arg( addSource ),
                    "--configfile ".Arg( configFile ),
                    "--framework ".Arg( framework ),
                    "-v ".Arg( GetVerbosityString( verbosity ) ),
                    "--version ".Arg( version )
                }, workingDirectory );

            public static Task<bool> Install( IActivityMonitor m, string packageName, string toolpath, string workingDirectory = "", string? addSource = null,
                string? configFile = null, string? framework = null, Verbosity? verbosity = null, string? version = null )
                => CLIRunner.RunAsync( m, "dotnet", new List<string?>()
                {
                    "tool install",
                    packageName,
                    "--toolpath ".Arg(toolpath),
                    "--add-source ".Arg( addSource ),
                    "--configfile ".Arg( configFile ),
                    "--framework ".Arg( framework ),
                    "-v ".Arg( GetVerbosityString( verbosity ) ),
                    "--version ".Arg( version )
                }, workingDirectory );

            public class ToolInfo
            {
                internal ToolInfo( string cKli, string version, string commands )
                {
                    PackageId = cKli;
                    Version = version;
                    Commands = commands;
                }
                public string PackageId { get; }
                public string Version { get; }
                public string Commands { get; }
            }

            static IEnumerable<ToolInfo> ReadToolListOutput( IEnumerable<string> input )
                => input.Select( s =>
                {
                    var splits = s.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
                    if( splits.Length != 3 ) throw new InvalidDataException( "Could not parse the output." );
                    return new ToolInfo( splits[0], splits[1], splits[2] );
                } );

            public static async Task<IEnumerable<ToolInfo>?> List( IActivityMonitor m )
            {
                (int code, IEnumerable<string> output) = await CLIRunner.RunAndGetLinesOutput( m, "dotnet", new List<string?>()
                {
                    "tool list --global",
                } );
                if( code != 0 ) return null;
                return ReadToolListOutput( output.Skip( 2 ) );
            }

            public static async Task<IEnumerable<ToolInfo>?> List( IActivityMonitor m, string workingDirectory = "" )
            {
                (int code, IEnumerable<string> output) = await CLIRunner.RunAndGetLinesOutput( m, "dotnet", new List<string?>()
                {
                    "tool list"
                }, workingDirectory );
                if( code != 0 ) return null;
                return ReadToolListOutput( output.Skip( 2 ) );
            }

            public static async Task<IEnumerable<ToolInfo>?> ListAt( IActivityMonitor m, string toolpath )
            {
                (int code, IEnumerable<string> output) = await CLIRunner.RunAndGetLinesOutput( m, "dotnet", new List<string?>()
                {
                    "tool list --tool-path", toolpath
                } );
                if( code != 0 ) return null;
                return ReadToolListOutput( output.Skip( 2 ) );
            }

            public static Task<bool> Restore( IActivityMonitor m, string workingDirectory = "", string? addSource = null, string? configFile = null,
                string? toolmanifest = null, bool disableParallel = false, bool ignoreFailedSource = false, bool noCache = false, Verbosity? verbosity = null )
                => CLIRunner.RunAsync( m, "dotnet", new List<string?>()
                {
                    "tool restore",
                    "--add-source ".Arg( addSource ),
                    "--configfile ".Arg( configFile ),
                    "--tool-manifest ".Arg(toolmanifest),
                    "-v ".Arg( GetVerbosityString( verbosity ) ),
                    disableParallel ? "--disable-parallel" : null,
                    ignoreFailedSource ? "--ignore-failed-sources" : null,
                    noCache ? "--no-cache" : null
                }, workingDirectory );

            public static Task<bool> Run( IActivityMonitor m, string commandName, string args = "", string workingDirectory = "" )
                => CLIRunner.RunAsync( m, "dotnet", new List<string?>()
                {
                    "tool run",
                    commandName,
                    args
                }, workingDirectory );
        }
    }
}
