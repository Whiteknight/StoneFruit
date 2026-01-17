namespace StoneFruit.Execution.IO;

public interface IObjectOutputWriter
{
    void WriteObject<T>(T obj);

    void MaybeWriteObject<T>(T? obj)
        where T : class
    {
        if (obj != null)
            WriteObject(obj);
    }
}

public sealed class NullObjectOutputWriter : IObjectOutputWriter
{
    public void WriteObject<T>(T obj)
    {
        // Do nothing
    }
}

public sealed class JsonObjectOutputWriter : IObjectOutputWriter
{
    private static readonly System.Text.Json.JsonSerializerOptions _settings = new System.Text.Json.JsonSerializerOptions
    {
        WriteIndented = true
    };

    private readonly IOutput _output;

    public JsonObjectOutputWriter(IOutput output)
    {
        _output = output;
    }

    public void WriteObject<T>(T obj)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(obj, _settings);
        _output.WriteLine(json);
    }
}

public static class ObjectWriterExtensions
{
    public static IIoSetup UseJsonObjectWriter(this IIoSetup setup)
        => setup.UseObjectWriter<JsonObjectOutputWriter>();
}
