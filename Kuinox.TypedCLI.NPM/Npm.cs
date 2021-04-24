using CK.Core;
using Kuinox.TypedCLI.Dotnet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kuinox.TypedCLI.NPM
{
    public static partial class Npm
    {
        static string? Arg( this string @this, string? argVal ) => argVal is null ? null : @this + argVal;

        public static Task<bool> Ci( IActivityMonitor m, string workingDirectory = "" )
            => CLIRunner.RunAsync( m, "npm", new string[] { "ci" }, workingDirectory );

        public static Task<bool> Install( IActivityMonitor m, string? thingToInstall = null, string workingDirectory = "", bool saveProd = false, bool saveDev = false,
            bool saveOptional = false, bool noSave = false, bool saveExact = false, bool saveBundle = false, bool dryRun = false, bool packageLockOnly = false, bool force = false,
            string? tag = null, bool global = false, bool globalStyle = false, bool legacyBundling = false, bool legacyPeerDeps = false, bool strictPeerDeps = false,
            bool noPackageLock = false, bool ignoreScripts = false, bool noAudit = false, bool noBinLinks = false, bool noFund = false, bool savePeer = false )
            => CLIRunner.RunAsync( m, "npm", new string?[] {
                "install",
                thingToInstall,
                saveProd ? "--save-prod" : null,
                saveDev ? "--save-dev" : null,
                saveOptional ? "--save-optional" : null,
                savePeer ? "--save-peer" : null,
                noSave ? "--no-save" : null,
                saveExact ? "--save-exact" : null,
                saveBundle ? "--save-bundle" : null,
                dryRun ? "--dry-run" : null,
                packageLockOnly ? "--package-lock-only" : null,
                force ? "--force" : null,
                "--tag ".Arg(tag),
                global ? "--global" : null,
                globalStyle ? "--global-style" : null,
                legacyBundling ? "--legacy-bundling" : null,
                legacyPeerDeps ? "--legacy-peer-deps" : null,
                strictPeerDeps ? "--strict-peer-deps" : null,
                noPackageLock ? "--no-package-lock" : null,
                ignoreScripts ? "--ignore-scripts" : null,
                noAudit ? "--no-audit" : null,
                noBinLinks ? "--no-bin-links" : null,
                noFund ? "--no-fund" : null
            }, workingDirectory );

        public static Task<bool> Ping( IActivityMonitor m, string workingDirectory = "", string? registryToPing = null )
            => CLIRunner.RunAsync( m, "npm", new string?[]
            {
                "ping",
                "--registry ".Arg(registryToPing)
            }, workingDirectory );

        public static Task<bool> RunScript( IActivityMonitor m, string command, string? args = null, string workingDirectory = "", bool silent = false )
            => CLIRunner.RunAsync( m, "npm", new string?[]
            {
                "run-script",
                silent ? "--silent" : null,
                command,
                "-- ".Arg(args)
            }, workingDirectory );

        public static async Task<IEnumerable<string>?> Pack( IActivityMonitor m, IEnumerable<string>? thingsToPack = null, bool dryRun = false, string workingDirectory = "" )
        {
            List<string?> args = new()
            {
                "pack",
                dryRun ? "--dry-run" : null
            };
            if( thingsToPack != null ) args.AddRange( thingsToPack );
            (int code, IEnumerable<string> lines) = await CLIRunner.RunAndGetOutput( m, "npm", args, workingDirectory );
            if( code != 0 ) return null;
            return lines;
        }

        public enum AccessLevel
        {
            Public,
            Restricted
        }

        public static Task<bool> Publish( IActivityMonitor m, string workingDirectory = "", IEnumerable<string>? thingsToPublish = null, string? tag = null, bool dryRun = false,
            AccessLevel? accessLevel = null )
        {
            List<string?> args = new()
            {
                "publish",
                "--tag ".Arg( tag ),
                dryRun ? "--dry-run" : null
            };
            if( thingsToPublish != null ) args.AddRange( thingsToPublish );
            if( accessLevel.HasValue )
            {
                args.Add( "--access " + accessLevel switch
                {
                    AccessLevel.Public => "public",
                    AccessLevel.Restricted => "restricted",
                    _ => throw new ArgumentException( nameof( accessLevel ) )
                } );
            }
            return CLIRunner.RunAsync( m, "npm", args, workingDirectory );
        }
    }
}
