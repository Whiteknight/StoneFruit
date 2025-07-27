namespace StoneFruit.SpecTests.Support;

#pragma warning disable CA1822 // Mark members as static

public class ValueTypeParsingInstance
{
    public void AsFileInfo(IOutput output, FileInfo file)

    {
        output.WriteLine(file != null);
    }

    public void AsGuid(IOutput output, Guid guid)
    {
        output.WriteLine(guid != Guid.Empty);
    }
}
