using CK.Core;
using CK.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kuinox.TypedCLI.Dotnet
{
    public static class CLIRunner
    {
        public static async Task<bool> RunAsync( IActivityMonitor m, string cliName, IEnumerable<string?> args, string workingDirectory = "" )
        {
            string argStr = args.Where( s => !string.IsNullOrWhiteSpace( s ) ).Concatenate( " " );
            ProcessStartInfo startInfo = new( cliName, argStr )
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using( m.OpenInfo( $"Running {cliName} {argStr} in {Path.GetFullPath( workingDirectory )}" ) )
            using( Process process = Process.Start( startInfo )! )
            {
                TaskCompletionSource<object?> _ctsExit = new();
                process.Exited += ( _, _ ) => _ctsExit.SetResult( null );
                if( process.HasExited ) _ctsExit.SetResult( null );
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
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                await _ctsExit.Task;
                return process.ExitCode == 0;
            }
        }

        public static async Task<(int, IEnumerable<string>)> RunAndGetLinesOutput( IActivityMonitor m, string cliName, IEnumerable<string?> args, string workingDirectory = "" )
        {
            string argStr = args.Where( s => !string.IsNullOrWhiteSpace( s ) ).Concatenate( " " );
            ProcessStartInfo startInfo = new( cliName, argStr )
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            TaskCompletionSource<object?> _ctsExit = new();
            using( m.OpenInfo( $"Running {cliName} {argStr} in {Path.GetFullPath( workingDirectory == "" ? "." : workingDirectory )}" ) )
            using( Process process = Process.Start( startInfo )! )
            using( CancellationTokenSource cts = new() )
            using( cts.Token.Register( () => _ctsExit.SetResult( null ) ) )
            {
                process.EnableRaisingEvents = true;
                StringBuilder sb = new();

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
                process.Exited += ( _, _ ) => cts.Cancel();
                if( process.HasExited ) cts.Cancel();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                await _ctsExit.Task;
                return (process.ExitCode, sb.ToString().Split( '\n' ));
            }
        }

        public static async Task<(int, string)> RunAndGetOutput( IActivityMonitor m, string cliName, IEnumerable<string?> args, string workingDirectory = "" )
        {
            string argStr = args.Where( s => !string.IsNullOrWhiteSpace( s ) ).Concatenate( " " );
            ProcessStartInfo startInfo = new( cliName, argStr )
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using( m.OpenInfo( $"Running {cliName} {argStr} in {Path.GetFullPath( workingDirectory )}" ) )
            using( Process process = Process.Start( startInfo )! )
            {
                StringBuilder sb = new();
                TaskCompletionSource<object?> _ctsExit = new();
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
                if( process.HasExited ) _ctsExit.SetResult( null );
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                await _ctsExit.Task;
                return (process.ExitCode, sb.ToString());
            }
        }
    }
}
