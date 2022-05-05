using Newtonsoft.Json.Linq;
using NJsonSchema.Validation.FormatValidators;

namespace NJsonSchema.Extensions;

internal static class StringMapper
{
    private static readonly IFormatValidator[] FormatValidators =
    {
        new DateFormatValidator(),
        new DateTimeFormatValidator(),
        new TimeSpanFormatValidator(),
        new TimeFormatValidator(),
        new GuidFormatValidator(),
        new UuidFormatValidator(),
        new EmailFormatValidator(),
        new IpV4FormatValidator(),
        new IpV6FormatValidator(),
        new UriFormatValidator()
    };

    public static JsonSchemaProperty Map(string? value)
    {
        var stringSchemaProperty = new JsonSchemaProperty { Type = JsonObjectType.String };
        if (string.IsNullOrEmpty(value))
        {
            return stringSchemaProperty;
        }

        foreach (var validator in FormatValidators)
        {
            if (validator.IsValid(value, JTokenType.String))
            {
                stringSchemaProperty.Format = validator.Format;
                break;
            }
        }

        return stringSchemaProperty;
    }
}