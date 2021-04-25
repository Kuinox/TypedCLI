using CK.Core;
using Kuinox.TypedCLI.Dotnet;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kuinox.TypedCLI.NPM
{
    public static partial class Npm
    {
        public static class DistTag
        {
            public static Task<bool> Add( IActivityMonitor m, string packageId, IEnumerable<string> tag, string workingDirectory = "" )
            {
                List<string?> args = new()
                {
                    "dist-tag add",
                    packageId
                };
                args.AddRange( tag );
                return CLIRunner.RunAsync( m, "npm", args, workingDirectory );
            }

            public static Task<bool> Add( IActivityMonitor m, string packageId, string tag, string workingDirectory = "" )
                => Add( m, packageId, new string[] { tag }, workingDirectory );

            public static Task<bool> Rm( IActivityMonitor m, string packageid, string tag, string workingDirectory = "" )
                => CLIRunner.RunAsync( m, "npm", new string[] { "dist-tag rm", packageid, tag }, workingDirectory );
        }
    }
}
