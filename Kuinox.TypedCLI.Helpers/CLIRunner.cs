using CK.Core;
using CK.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
            using( m.OpenInfo( $"Running {cliName} {argStr} in {Path.GetFullPath( workingDirectory == "" ? "." : workingDirectory )}" ) )
            using( Process process = Process.Start( startInfo )! )
            {
                process.EnableRaisingEvents = true;
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
                ReflectionHack( process );
                process.WaitForExit();
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
            using( m.OpenInfo( $"Running {cliName} {argStr} in {Path.GetFullPath( workingDirectory == "" ? "." : workingDirectory )}" ) )
            using( Process process = Process.Start( startInfo )! )
            {
                process.EnableRaisingEvents = true;
                List<string> lines = new();

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
                            lines.Add( e.Data );
                            m.Info( e.Data );
                        }
                    }
                };
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                process.WaitForExit( int.MaxValue ); //First we need for the program to exit.
                // Here the program exited, but background may still pump messages
                ReflectionHack( process ); // This will cancel the reads in the pipes background loop.
                process.WaitForExit(); // This allow to wait for the 2 pipes async to finish looping and flushing the last messages.
                // Here you shouldn't receive any message.

                if( process.ExitCode != 0 ) throw new IOException( $"Subprocess returned with exit code '{process.ExitCode}'." );
                return (process.ExitCode, lines);
            }
        }

        /// <summary>
        /// This method shut down things in <see cref="Process"/>. Call it when you know that the process has exited.
        /// Sometimes the <see cref="Process"/> class deadlock itself.
        /// See this for more info: https://github.com/dotnet/runtime/issues/51277
        /// </summary>
        /// <param name="process"></param>
        static void ReflectionHack( Process process )
        {
            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo? outputField = typeof( Process ).GetField( "_output", bindingFlags );
            FieldInfo? errorField = typeof( Process ).GetField( "_error", bindingFlags );
            ((IDisposable)outputField!.GetValue( process )!).Dispose();
            ((IDisposable)errorField!.GetValue( process )!).Dispose();
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
            using( m.OpenInfo( $"Running {cliName} {argStr} in {Path.GetFullPath( workingDirectory == "" ? "." : workingDirectory )}" ) )
            using( Process process = Process.Start( startInfo )! )
            {
                process.EnableRaisingEvents = true;
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
                ReflectionHack( process );
                process.WaitForExit();
                return (process.ExitCode, sb.ToString());
            }
        }
    }
}
