using CK.Core;
using CK.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuinox.TypedCLI.Dotnet
{
    public static class CLIRunner
    {
        public static async Task<bool> RunAsync( IActivityMonitor m, string cliName, IEnumerable<string?> args, string workingDirectory = "" )
        {
            string argStr = args.Where( s => !string.IsNullOrWhiteSpace( s ) ).Concatenate( " " );
            using( m.OpenInfo( $"Running {cliName} {argStr} in {Path.GetFullPath( workingDirectory )}" ) )
            using( Process process = new() )
            {
                TaskCompletionSource<object?> _ctsExit = new();
                process.StartInfo = new ProcessStartInfo( cliName, argStr )
                {
                    WorkingDirectory = workingDirectory,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                process.ErrorDataReceived += ( o, e ) =>
                {
                    if( !string.IsNullOrEmpty( e.Data ) )
                    {
                        lock( process )
                        {
                            m.Warn( e.Data );
                        }
                    };
                };
                process.OutputDataReceived += ( o, e ) =>
                {
                    if( !string.IsNullOrEmpty( e.Data ) )
                    {
                        lock( process )
                        {
                            m.Info( e.Data );
                        }
                    }
                };
                process.Exited += ( _, _ ) => _ctsExit.SetResult( null );
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                await _ctsExit.Task;
                return process.ExitCode == 0;
            }
        }

        public static async Task<(int, IEnumerable<string>)> RunAndGetLinesOutput( IActivityMonitor m, string cliName, IEnumerable<string?> args, string workingDirectory = "" )
        {
            string argStr = args.Where( s => !string.IsNullOrWhiteSpace( s ) ).Concatenate( " " );
            using( m.OpenInfo( $"Running {cliName} {argStr} in {Path.GetFullPath( workingDirectory )}" ) )
            using( Process process = new() )
            {
                StringBuilder sb = new();
                TaskCompletionSource<object?> _ctsExit = new();
                process.StartInfo = new ProcessStartInfo( cliName, argStr )
                {
                    WorkingDirectory = workingDirectory,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                process.ErrorDataReceived += ( o, e ) =>
                {
                    if( !string.IsNullOrEmpty( e.Data ) )
                    {
                        lock( process )
                        {
                            m.Warn( e.Data );
                        }
                    };
                };
                process.OutputDataReceived += ( o, e ) =>
                {
                    if( !string.IsNullOrEmpty( e.Data ) )
                    {
                        sb.Append( e.Data );
                        lock( process )
                        {
                            m.Info( e.Data );
                        }
                    }
                };
                process.Exited += ( _, _ ) => _ctsExit.SetResult( null );
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                await _ctsExit.Task;
                return (process.ExitCode, sb.ToString().Split( '\n' ));
            }
        }

        public static async Task<(int, string)> RunAndGetOutput( IActivityMonitor m, string cliName, IEnumerable<string?> args, string workingDirectory = "" )
        {
            string argStr = args.Where( s => !string.IsNullOrWhiteSpace( s ) ).Concatenate( " " );
            using( m.OpenInfo( $"Running {cliName} {argStr} in {Path.GetFullPath( workingDirectory )}" ) )
            using( Process process = new() )
            {
                StringBuilder sb = new();
                TaskCompletionSource<object?> _ctsExit = new();
                process.StartInfo = new ProcessStartInfo( cliName, argStr )
                {
                    WorkingDirectory = workingDirectory,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                process.ErrorDataReceived += ( o, e ) =>
                {
                    if( !string.IsNullOrEmpty( e.Data ) )
                    {
                        lock( process )
                        {
                            m.Warn( e.Data );
                        }
                    };
                };
                process.OutputDataReceived += ( o, e ) =>
                {
                    if( !string.IsNullOrEmpty( e.Data ) )
                    {
                        sb.Append( e.Data );
                        lock( process )
                        {
                            m.Info( e.Data );
                        }
                    }
                };
                process.Exited += ( _, _ ) => _ctsExit.SetResult( null );
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                await _ctsExit.Task;
                return (process.ExitCode, sb.ToString());
            }
        }
    }
}
