using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using CommandLine;
using DLMC.Client;
using DLMC.Server;
using DLMC.Shared.Utils;

namespace DLMC.Launcher
{
    class Program
    {
        static int Main(string[] args)
        {
            Logger.OnLog += (msg) =>
            {
                Console.WriteLine(msg);
            };

            return CommandLine.Parser.Default.ParseArguments<HostOptions, ConnectOptions>(args)
                .MapResult(
                  (HostOptions opts) => RunHostAndReturnExitCode(opts),
                  (ConnectOptions opts) => RunConnectAndReturnExitCode(opts),
                  errs => 1);
        }

        static int RunHostAndReturnExitCode(HostOptions opts)
        {

#if DEBUG

            // Use environment args if pcsx2 and iso paths are not provided
            if (opts.PCSX2Path == null || opts.PCSX2Path == String.Empty)
                opts.PCSX2Path = Environment.GetEnvironmentVariable("PCSX2");

            if (opts.DLPath == null || opts.DLPath == String.Empty)
                opts.DLPath = Environment.GetEnvironmentVariable("DEADLOCKED_ISO");

#endif

            Console.WriteLine($"Server starting on port {opts.Port}...");
            Net.Server server = new Net.Server(opts);
            Console.WriteLine("Press 1 to Stop Server...");

            try
            {
                // Perform text input
                for (; ; )
                {
                    // Update
                    server.Update();

                    // Check exit
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.D1)
                            break;
                    }

                    // Yield
                    Thread.Sleep(5);
                    Thread.Yield();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }

            // Stop the server
            Console.Write("Server stopping...");
            server.Stop();
            Console.WriteLine("Done!");

#if DEBUG
            Console.ReadKey();
#endif

            return 0;
        }

        static int RunConnectAndReturnExitCode(ConnectOptions opts)
        {

#if DEBUG

            // Use environment args if pcsx2 and iso paths are not provided
            if (opts.PCSX2Path == null || opts.PCSX2Path == String.Empty)
                opts.PCSX2Path = Environment.GetEnvironmentVariable("PCSX2");

            if (opts.DLPath == null || opts.DLPath == String.Empty)
                opts.DLPath = Environment.GetEnvironmentVariable("DEADLOCKED_ISO");

#endif

            Console.WriteLine("Client starting...");
            Net.Client client = new Net.Client(opts);
            Console.WriteLine("Press 1 to stop the client...");

            try
            {
                // Perform text input
                for (; ; )
                {
                    // Update
                    client.Update();

                    // Check exit
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.D1)
                            break;
                    }

                    // Yield
                    Thread.Sleep(5);
                    Thread.Yield();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }

            // Stop the server
            Console.Write("Client stopping...");
            client.Stop();
            Console.WriteLine("Done!");

#if DEBUG
            Console.ReadKey();
#endif


            return 1;
        }
    }
}
