using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLMC.Launcher
{
    [Verb("host", HelpText = "Host a session.")]
    public class HostOptions
    {
        [Option('p', "port", Required = true, HelpText = "Public port to bind.", Default = 19001)]
        public int Port { get; set; }
        
        [Option("pcsx2", Required = false, HelpText = "Path to pcsx2.")]
        public string PCSX2Path { get; set; }

        [Option("dl", Required = false, HelpText = "Path to deadlocked iso.")]
        public string DLPath { get; set; }
    }

    [Verb("connect", HelpText = "Connect to session.")]
    public class ConnectOptions
    {
        [Option('h', "hostname", Required = true, HelpText = "Server hostname or ip address.")]
        public string Hostname { get; set; }

        [Option('p', "port", Required = true, HelpText = "Server port.", Default = 19001)]
        public int Port { get; set; }

        [Option("pcsx2", Required = false, HelpText = "Path to pcsx2.")]
        public string PCSX2Path { get; set; }

        [Option("dl", Required = false, HelpText = "Path to deadlocked iso.")]
        public string DLPath { get; set; }
    }
}
