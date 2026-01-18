namespace StoneFruit.Execution.IO;

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
