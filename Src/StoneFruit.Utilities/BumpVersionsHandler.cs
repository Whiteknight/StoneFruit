using System;
using System.Collections.Generic;
using System.IO;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Utilities
{
    public class BumpVersionsHandler : IHandler
    {
        private readonly IOutput _output;
        private readonly IArguments _args;
        private readonly UtilitiesEnvironment _environment;
        private readonly VersionService _versions;

        public BumpVersionsHandler(IOutput output, IArguments args, UtilitiesEnvironment environment, VersionService versions)
        {
            _output = output;
            _args = args;
            _environment = environment;
            _versions = versions;
        }

        public void Execute()
        {
            var libraries = _environment.Libraries;
            var libArg = _args.Shift();
            if (libArg.Exists())
                libraries = new[] { libArg.Value };
            var major = _args.HasFlag("major");
            var minor = _args.HasFlag("minor");
            BumpVersions(libraries, major, minor);
        }

        private void BumpVersions(IReadOnlyList<string> libraries, bool major, bool minor)
        {
            foreach (var lib in libraries)
            {
                var version = BumpVersion(major, minor, lib);
                _output
                    .Color(ConsoleColor.White).Write(lib)
                    .Color(ConsoleColor.Gray).Write(": ")
                    .Color(ConsoleColor.Green).WriteLine(version);
            }
        }

        private string BumpVersion(bool major, bool minor, string lib)
        {
            var path = $@".\Src\{lib}\{lib}.csproj";
            if (!File.Exists(path))
            {
                _output.Color(ConsoleColor.Red).WriteLine($"{path} does not exist");
                return "";
            }

            var version = _versions.GetVersion(path);
            version = _versions.GetNextVersion(version, major, minor);
            _versions.SetVersion(path, version);
            return version;
        }
    }
}