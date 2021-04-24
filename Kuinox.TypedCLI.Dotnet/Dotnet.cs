using CK.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuinox.TypedCLI.Dotnet
{
    public static partial class Dotnet
    {
        static string? Arg( this string @this, string? argVal ) => argVal is null ? null : @this + argVal;

        public enum Verbosity
        {
            Quiet,
            Minimal,
            Normal,
            Detailed,
            Diagnostic
        }

        static string? GetVerbosityString( Verbosity? verbosity )
        {
            if( !verbosity.HasValue ) return null;
            return verbosity.Value switch
            {
                Verbosity.Quiet => "q",
                Verbosity.Minimal => "m",
                Verbosity.Normal => "n",
                Verbosity.Detailed => "d",
                Verbosity.Diagnostic => "diag",
                _ => throw new ArgumentOutOfRangeException( nameof( verbosity ), verbosity, "Invalid verbosity level." )
            };
        }

        public static Task<bool> Build( IActivityMonitor m, string workingDirectory = "", string? projectOrSolution = null,
            string? configuration = null, string? framework = null, bool force = false, bool noDependencies = false,
            bool noIncremental = false, bool noRestore = false, bool noLogo = false, string? outputDirectory = null,
            string? runtime = null, string[]? sources = null, Verbosity? verbosity = null, string? versionSuffix = null )
        {
            List<string?> args = new()
            {
                "build",
                projectOrSolution,
                "-c ".Arg( configuration ),
                "-f ".Arg( framework ),
                force ? "--force" : null,
                noDependencies ? "--no-dependencies" : null,
                noIncremental ? "--no-incremental" : null,
                noRestore ? "--no-restore" : null,
                noLogo ? "--nologo" : null,
                "-o ".Arg( outputDirectory ),
                "-r ".Arg( runtime ),
                "-v ".Arg( GetVerbosityString( verbosity ) ),
                "--version-suffix ".Arg( versionSuffix )
            };
            if( sources != null ) args.AddRange( sources.Select( s => "--source ".Arg( s ) ) );
            return CLIRunner.RunAsync( m, "dotnet", args, workingDirectory );
        }

        public static Task<bool> ShutdownBuildServer( IActivityMonitor m, bool msbuild = false, bool razor = false, bool vbcscompiler = false )
            => CLIRunner.RunAsync( m, "dotnet", new List<string?>() {
                "build-server shutdown",
                msbuild ? "--msbuild" : null,
                razor ? "--razor" : null,
                vbcscompiler ? "--vbcscompiler" : null
            } );

        public static Task<bool> Clean( IActivityMonitor m, string workingDirectory = "", string? projectOrSolution = null, string? configuration = null, string? framework = null,
            bool nologo = false, string? outputDirectory = null, string? runtime = null, Verbosity? verbosity = null )
            => CLIRunner.RunAsync( m, "dotnet", new List<string?>()
            {
                "clean",
                projectOrSolution,
                "-c ".Arg( configuration ),
                "-f ".Arg( framework ),
                nologo ? "--nologo" : null,
                "-o ".Arg( outputDirectory ),
                "-r ".Arg( runtime ),
                "-v ".Arg( GetVerbosityString( verbosity ) )
            }, workingDirectory );

        public static Task<bool> Pack( IActivityMonitor m, string workingDirectory = "", string? projectOrSolution = null, string? configuration = null, bool force = false,
            bool includeSource = false, bool includeSymbols = false, bool noBuild = false, bool noDependencies = false, bool noRestore = false, bool noLogo = false,
            string? outputDirectory = null, string? runtime = null, bool serviceable = false, Verbosity? verbosity = null, string? versionSuffix = null )
            => CLIRunner.RunAsync( m, "dotnet", new List<string?>()
            {
                "pack",
                projectOrSolution,
                "-c ".Arg( configuration ),
                force ? "--force" : null,
                noDependencies ? "--no-dependencies" : null,
                includeSource ? "--include-source" : null,
                includeSymbols ? "--include-symbols" : null,
                "-o ".Arg( outputDirectory ),
                "-r ".Arg( runtime ),
                "-v ".Arg( GetVerbosityString( verbosity ) ),
                "--version-suffix ".Arg( versionSuffix ),
                noRestore ? "--no-restore" : null,
                noBuild? "--no-build" : null,
                noLogo ? "--nologo" : null,
                serviceable ? "--serviceable" : null
            }, workingDirectory );

        public static Task<bool> Publish( IActivityMonitor m, string workingDirectory = "", string? projectOrSolution = null, string? configuration = null,
            string? framework = null, bool force = false, string? manifestPath = null, bool noBuild = false, bool noDependencies = false, bool noRestore = false,
            bool noLogo = false, string? outputDirectory = null, string? runtime = null, bool? selfContained = null, Verbosity? verbosity = null, string? versionSuffix = null )
            => CLIRunner.RunAsync( m, "dotnet", new List<string?>()
            {
                "publish",
                projectOrSolution,
                "-c ".Arg( configuration ),
                force ? "--force" : null,
                noDependencies ? "--no-dependencies" : null,
                "-o ".Arg( outputDirectory ),
                "-r ".Arg( runtime ),
                "-v ".Arg( GetVerbosityString( verbosity ) ),
                "--version-suffix ".Arg( versionSuffix ),
                noRestore ? "--no-restore" : null,
                noBuild ? "--no-build" : null,
                noLogo ? "--nologo" : null,
                "-f ".Arg( framework ),
                "--manifest ".Arg( manifestPath ),
                selfContained.HasValue ? ("--self-contained " + selfContained.Value) : null
            }, workingDirectory );

        public static Task<bool> Publish( IActivityMonitor m, string workingDirectory = "", string? root = null, string? configFile = null, bool disableParallel = false,
            bool force = false, bool forceEvaluate = false, bool ignoreFailedSource = false, string? lockFilePath = null, bool lockedMode = false, bool noCache = false,
            bool noDependencies = false, string? packagesDirectory = null, string? runtime = null, IEnumerable<string>? sources = null, bool useLockFile = false, Verbosity? verbosity = null )
        {
            List<string?> args = new()
            {
                "publish",
                root,
                "--configfile ".Arg( configFile ),
                disableParallel ? "--disable-parallel" : null,
                force ? "--force" : null,
                forceEvaluate ? "--force-evaluate" : null,
                ignoreFailedSource ? "--ignore-failed-sources" : null,
                "--lock-file-path ".Arg( lockFilePath ),
                lockedMode ? "--locked-mode" : null,
                noCache ? "--no-cache" : null,
                noDependencies ? "--no-dependencies" : null,
                "--packages ".Arg( packagesDirectory ),
                "-r ".Arg( runtime ),
                useLockFile ? "--use-lock-file" : null,
                "-v ".Arg( GetVerbosityString( verbosity ) )
            };
            if( sources != null ) args.AddRange( sources.Select( s => "-s " + s ) );
            return CLIRunner.RunAsync( m, "dotnet", args, workingDirectory );
        }

        public static Task<bool> Run( IActivityMonitor m, string workingDirectory = "", string? configuration = null, string? framework = null, bool force = false, string? launchProfile = null,
            bool noBuild = false, bool noDependencies = false, bool noLaunchProfile = false, bool noRestore = false, string? projectPath = null, string? runtime = null,
            Verbosity? verbosity = null, string? application = null, string? args = null )
            => CLIRunner.RunAsync( m, "dotnet", new List<string?>()
            {
                "run",
                "-c ".Arg( configuration ),
                "-f ".Arg( framework ),
                force ? "--force" : null,
                "--launch-profile ".Arg( launchProfile ),
                noLaunchProfile ? "--no-launch-profile" : null,
                noRestore ? "--no-restore" : null,
                noBuild ? "--no-build" : null,
                noDependencies ? "--no-dependencies" : null,
                "--project ".Arg( projectPath ),
                "-r ".Arg( runtime ),
                "-v ".Arg( GetVerbosityString( verbosity ) ),
                application,
                args
            }, workingDirectory );

        public static Task<bool> Test( IActivityMonitor m, string workingDirectory = "", string? projectOrSolutionOrDirectoryOrDll = null, string? testAdapterPath = null,
            bool blame = false, bool blameCrash = false, string? blameCrashDumpType = null, bool blameCrashCollectAlways = false, bool blameHang = false, string? blameHangDumpType = null,
            string? blameHangTimeout = null, string? configuration = null, string? collect = null, string? diag = null, string? framework = null, string? filter = null,
            string? logger = null, bool noBuild = false, bool noLogo = false, bool noRestore = false, string? outputDirectory = null, string? resultsDirectory = null,
            string? runtime = null, string? settingsFile = null, bool listTests = false, Verbosity? verbosity = null, string? runsettingsArguments = null )
            => CLIRunner.RunAsync( m, "dotnet", new List<string?>()
            {
                "test",
                projectOrSolutionOrDirectoryOrDll,
                "--test-adapter-path ".Arg( testAdapterPath ),
                blame ? "--blame" : null,
                blameCrash ? "--blame-crash" : null,
                "--blame-crash-dump-type ".Arg( blameCrashDumpType ),
                blameCrashCollectAlways ? "--blame-crash-collect-always" : null,
                blameHang ? "--blame-hang" : null,
                "--blame-hang-dump-type ".Arg( blameHangDumpType ),
                "--blame-hang-timeout ".Arg( blameHangTimeout ),
                "-c ".Arg( configuration ),
                "--collect ".Arg( collect ),
                "--diag ".Arg( diag ),
                "-f ".Arg( framework ),
                "--filter ".Arg( filter ),
                "--logger ".Arg( logger ),
                noBuild ? "--no-build" : null,
                noLogo ? "--nologo" : null,
                noRestore ? "--no-restore" : null,
                "--output ".Arg( outputDirectory ),
                "--result-directory ".Arg( resultsDirectory ),
                "--runtime ".Arg( runtime ),
                "--settings ".Arg( settingsFile ),
                listTests ? "--list-tests" : null,
                "-v ".Arg( GetVerbosityString( verbosity ) ),
                runsettingsArguments
            }, workingDirectory );

        public static Task<bool> Restore( IActivityMonitor m, string workinDirectory = "", string? root = null, string? configFile = null, bool disableParallel = false, bool force = false,
            bool forceEvaluate = false, bool ignoreFailedSources = false, string? lockFilePath = null, bool lockedMode = false, bool noCache = false, bool noDependencies = false,
            string? packages = null, string? runtime = null, IEnumerable<string>? sources = null, bool useLockFile = false, Verbosity? verbosity = null )
        {
            List<string?> args = new()
            {
                "restore",
                root,
                "--configfile ".Arg( configFile ),
                disableParallel ? "--disable-parallel" : null,
                force ? "--force" : null,
                forceEvaluate ? "--force-evaluate" : null,
                ignoreFailedSources ? "--ignore-failed-sources" : null,
                "--lock-file-path ".Arg( lockFilePath ),
                lockedMode ? "--locked-mode" : null,
                noCache ? "--no-cache" : null,
                noDependencies ? "--no-dependencies" : null,
                "--packages ".Arg( packages ),
                "--runtime ".Arg( runtime ),
                useLockFile ? "--use-lock-file" : null,
                GetVerbosityString( verbosity )
            };
            if( sources != null ) args.AddRange( sources.Select( s => "-s " + s ) );
            return CLIRunner.RunAsync( m, "dotnet", args, workinDirectory );
        }

    }
}
