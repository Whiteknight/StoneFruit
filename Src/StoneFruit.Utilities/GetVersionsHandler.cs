using System;
using System.Collections.Generic;
using System.IO;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Utilities
{
    public class GetVersionsHandler : IHandler
    {
        private readonly IOutput _output;
        private readonly IArguments _args;
        private readonly UtilitiesEnvironment _environment;
        private readonly VersionService _versions;

        public GetVersionsHandler(IOutput output, IArguments args, UtilitiesEnvironment environment, VersionService versions)
        {
            _output = output;
            _args = args;
            _environment = environment;
            _versions = versions;
        }

        public void Execute()
        {
            IReadOnlyList<string> libraries;
            var libArg = _args.Shift();
            if (libArg.Exists())
                libraries = new string[] { libArg.Value };
            else
                libraries = _environment.Libraries;
            GetVersions(libraries);
        }

        private void GetVersions(IReadOnlyList<string> libraries)
        {
            foreach (var lib in libraries)
            {
                var path = $@".\Src\{lib}\{lib}.csproj";
                if (!File.Exists(path))
                {
                    _output.Color(ConsoleColor.Red).WriteLine($"{path} does not exist");
                    continue;
                }

                var version = _versions.GetVersion(path);

                if (string.IsNullOrEmpty(version))
                {
                    _output.Color(ConsoleColor.Red).WriteLine($"{path} does not contain Version");
                    continue;
                }

                _output
                    .Color(ConsoleColor.White).Write(lib)
                    .Color(ConsoleColor.Gray).Write(": ")
                    .Color(ConsoleColor.Green).WriteLine(version);
            }
        }
    }
}