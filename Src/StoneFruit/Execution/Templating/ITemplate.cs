using System.Collections.Generic;

namespace StoneFruit.Execution.Templating;

public interface ITemplate
{
    IEnumerable<OutputMessage> Render(object? value);
}
