namespace NJsonSchema.Extensions;

public class NJsonSchemaSettings
{
    /// <summary>
    /// Automatically resolve the Format based on the content of the string value.
    /// Default value true.
    /// </summary>
    public bool ResolveFormatForString { get; set; } = true;
}