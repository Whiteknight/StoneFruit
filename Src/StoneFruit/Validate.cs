namespace StoneFruit;

public static class Validate
{
    public static Maybe<string> IsNotNullOrEmpty(string value)
        => string.IsNullOrEmpty(value)
            ? default
            : value;
}
