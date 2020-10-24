using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StoneFruit.Utilities
{
    public class SetVersionsHandler : IHandler
    {
        private readonly IOutput _output;
        private readonly IArguments _args;
        private readonly UtilitiesEnvironment _environment;
        private readonly VersionService _versions;

        public SetVersionsHandler(IOutput output, IArguments args, UtilitiesEnvironment environment, VersionService versions)
        {
            _output = output;
            _args = args;
            _environment = environment;
            _versions = versions;
        }

        public void Execute()
        {
            var version = _args.Shift().Require().MarkConsumed().AsString();
            IReadOnlyList<string> libraries = _args
                .GetAllPositionals()
                .Select(p => p.AsString())
                .ToList();
            if (libraries.Count == 0)
                libraries = _environment.Libraries;
            SetVersions(libraries, version);
        }

        private void SetVersions(IReadOnlyList<string> libraries, string version)
        {
            foreach (var lib in libraries)
            {
                var newVersion = SetVersion(version, lib);
                _output
                    .Color(ConsoleColor.White).Write(lib)
                    .Color(ConsoleColor.Gray).Write(": ")
                    .Color(ConsoleColor.Green).WriteLine(newVersion);
            }
        }

        private string SetVersion(string version, string lib)
        {
            var path = $@".\Src\{lib}\{lib}.csproj";
            if (!File.Exists(path))
            {
                _output.Color(ConsoleColor.Red).WriteLine($"{path} does not exist");
                return "";
            }

            _versions.SetVersion(path, version);
            return version;
        }
    }
}