namespace StoneFruit.SpecTests.Support;

public class ValueTypeParsingInstance
{
    public void AsFileInfo(IOutput output, FileInfo file)
    {
        output.WriteLine(file != null);
    }
}
