using CK.Core;
using Kuinox.TypedCLI.Dotnet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kuinox.TypedCLI.NPM
{
    public static class Npm
    {
        public static Task<bool> Ci( IActivityMonitor m, string workingDirectory = "" )
            => CLIRunner.RunAsync( m, "npm", new string[] { "ci" }, workingDirectory );

        public static Task<bool> NpmInstall( IActivityMonitor m, string? thingToInstall = null, string workingDirectory = "" )
        {

        }
    }
}
