using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace StoneFruit.Utilities
{
    public class VersionService
    {
        public string GetVersion(string csprojPath)
        {
            var doc = new XmlDocument();
            doc.Load(csprojPath);
            var root = doc.DocumentElement;

            var versionNode = root.ChildNodes.Cast<XmlNode>()
                .Where(n => n.Name == "PropertyGroup")
                .Select(n => n["Version"])
                .FirstOrDefault();

            return versionNode?.InnerText;
        }

        public void SetVersion(string csprojPath, string version)
        {
            var lines = File.ReadAllLines(csprojPath);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.Contains("<Version>"))
                {
                    var newLine = Regex.Replace(line, "<Version>[^<]+</Version>", $"<Version>{version}</Version>");
                    lines[i] = newLine;
                    break;
                }
            }

            File.WriteAllLines(csprojPath, lines);
        }

        public string GetNextVersion(string currentVersion, bool bumpMajor, bool bumpMinor)
        {
            var match = Regex.Match(currentVersion, @"(\d+)\.(\d+)\.(\d+)");
            var major = int.Parse(match.Groups[1].Value);
            var minor = int.Parse(match.Groups[2].Value);
            var patch = int.Parse(match.Groups[3].Value);

            if (bumpMajor)
                return $"{major + 1}.0.0";
            if (bumpMinor)
                return $"{major}.{minor + 1}.0";
            return $"{major}.{minor}.{patch + 1}";
        }
    }
}