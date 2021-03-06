using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
#if NETSTANDARD1_3
#endif

namespace NJsonSchema.Extensions;

public static class NJsonSchemaExtensions
{
    #region JsonSchemaProperties
    private static JsonSchemaProperty Boolean => new() { Type = JsonObjectType.Boolean };
    private static JsonSchemaProperty Byte => new() { Type = JsonObjectType.String, Format = JsonFormatStrings.Byte };
    private static JsonSchemaProperty Date => new() { Type = JsonObjectType.String, Format = JsonFormatStrings.DateTime };
    private static JsonSchemaProperty Float => new() { Type = JsonObjectType.Number, Format = JsonFormatStrings.Float };
    private static JsonSchemaProperty Double => new() { Type = JsonObjectType.Number, Format = JsonFormatStrings.Double };
    private static JsonSchemaProperty Email => new() { Type = JsonObjectType.String, Format = JsonFormatStrings.Email };
    private static JsonSchemaProperty Guid => new() { Type = JsonObjectType.String, Format = JsonFormatStrings.Guid };
    private static JsonSchemaProperty Integer => new() { Type = JsonObjectType.Integer, Format = JsonFormatStrings.Integer };
    private static JsonSchemaProperty IpV4 => new() { Type = JsonObjectType.String, Format = JsonFormatStrings.IpV4 };
    private static JsonSchemaProperty IpV6 => new() { Type = JsonObjectType.String, Format = JsonFormatStrings.IpV6 };
    private static JsonSchemaProperty Long => new() { Type = JsonObjectType.Integer, Format = JsonFormatStrings.Long };
    private static JsonSchemaProperty Null => new() { Type = JsonObjectType.Null };
    private static JsonSchemaProperty Object => new() { Type = JsonObjectType.Object };
    private static JsonSchemaProperty String => new() { Type = JsonObjectType.String };
    private static JsonSchemaProperty TimeSpan => new() { Type = JsonObjectType.String, Format = JsonFormatStrings.TimeSpan };
    private static JsonSchemaProperty Uri => new() { Type = JsonObjectType.String, Format = JsonFormatStrings.Uri };
    #endregion

    /// <summary>
    /// Convert a <see cref="JObject"/> to a <see cref="JsonSchema"/>.
    /// </summary>
    /// <param name="instance">The <see cref="JObject"/> instance.</param>
    /// <param name="settings">The <see cref="NJsonSchemaSettings"/> to use when converting. [Optional]</param>
    /// <returns>The converted <see cref="JsonSchema"/>.</returns>
    public static JsonSchema ToJsonSchema(this JObject instance, NJsonSchemaSettings? settings = null)
    {
        settings ??= new NJsonSchemaSettings();
        return ConvertJToken(instance ?? throw new ArgumentNullException(nameof(instance)), settings);
    }

    /// <summary>
    /// Convert a <see cref="JArray"/> to a <see cref="JsonSchema"/>.
    /// </summary>
    /// <param name="instance">The <see cref="JArray"/> instance.</param>
    /// <param name="settings">The <see cref="NJsonSchemaSettings"/> to use when converting. [Optional]</param>
    /// <returns>The converted <see cref="JsonSchema"/>.</returns>
    public static JsonSchema ToJsonSchema(this JArray instance, NJsonSchemaSettings? settings = null)
    {
        settings ??= new NJsonSchemaSettings();
        return ConvertJToken(instance ?? throw new ArgumentNullException(nameof(instance)), settings);
    }

    /// <summary>
    /// Convert a <see cref="object"/> to a <see cref="JsonSchema"/>.
    /// </summary>
    /// <param name="instance">The <see cref="object"/> instance.</param>
    /// <param name="settings">The <see cref="NJsonSchemaSettings"/> to use when converting. [Optional]</param>
    /// <returns>The converted <see cref="JsonSchema"/>.</returns>
    public static JsonSchema ToJsonSchema(this object instance, NJsonSchemaSettings? settings = null)
    {
        settings ??= new NJsonSchemaSettings();
        return instance switch
        {
            JArray bodyAsJArray => bodyAsJArray.ToJsonSchema(settings),
            JObject bodyAsJObject => bodyAsJObject.ToJsonSchema(settings),
            null => throw new ArgumentNullException(nameof(instance)),
            _ => ConvertValue(instance, settings)
        };
    }

    private static JsonSchemaProperty ConvertJToken(JToken value, NJsonSchemaSettings settings)
    {
        var type = value.Type;
        switch (type)
        {
            case JTokenType.Array:
                var arrayItem = value.HasValues ? ConvertJToken(value.First!, settings) : Object;
                return new JsonSchemaProperty
                {
                    Type = JsonObjectType.Array,
                    Item = arrayItem
                };

            case JTokenType.Boolean:
                return Boolean;

            case JTokenType.Bytes:
                return Byte;

            case JTokenType.Date:
                return Date;

            case JTokenType.Guid:
                return Guid;

            case JTokenType.Float:
                return value is JValue { Value: double } ? Double : Float;

            case JTokenType.Integer:
                var valueAsLong = value.Value<long>();
                return valueAsLong is > int.MaxValue or < int.MinValue ? Long : Integer;

            case JTokenType.Null:
                return Null;

            case JTokenType.Object:
                var jsonSchemaPropertyForObject = new JsonSchemaProperty { Type = JsonObjectType.Object };
                foreach (var jProperty in ((JObject)value).Properties())
                {
                    jsonSchemaPropertyForObject.Properties.Add(jProperty.Name, ConvertJToken(jProperty.Value, settings));
                }

                return jsonSchemaPropertyForObject;

            case JTokenType.String:
                return settings.ResolveFormatForString ? StringMapper.Map(value.Value<string>()) : String;

            case JTokenType.TimeSpan:
                return TimeSpan;

            case JTokenType.Uri:
                return Uri;

            default:
                return Object;
        }
    }

    private static JsonSchemaProperty ConvertValue(object value, NJsonSchemaSettings settings)
    {
        switch (value)
        {
            case Array array:
                return new JsonSchemaProperty
                {
                    Type = JsonObjectType.Array,
                    Item = ConvertType(array.GetType().GetElementType()!)
                };

            case IList list:
                var genericArguments = list.GetType().GetGenericArguments();

                JsonSchemaProperty listType;
                if (genericArguments.Length > 0)
                {
                    listType = ConvertType(genericArguments[0]);
                }
                else
                {
                    listType = list.Count > 0 ? ConvertValue(list[0]!, settings) : Object;
                }

                return new JsonSchemaProperty
                {
                    Type = JsonObjectType.Array,
                    Items = { listType }
                };

            case IEnumerable<byte>:
                return Byte;

            case bool:
                return Boolean;

            case DateTime:
                return Date;

            case double:
                return Double;

            case System.Guid:
                return Guid;

            case float:
                return Float;

            case short:
            case int:
                return Integer;

            case long:
                return Long;

            case string stringValue:
                return settings.ResolveFormatForString ? StringMapper.Map(stringValue) : String;

            case System.TimeSpan:
                return TimeSpan;

            case System.Uri:
                return Uri;

            case not null: // object
                var jsonSchemaPropertyForObject = Object;
                foreach (var propertyInfo in value.GetType().GetProperties())
                {
                    var propertyValue = propertyInfo.GetValue(value, null);
                    var jsonSchemaProperty = value != null ? ConvertValue(propertyValue, settings) : ConvertType(propertyInfo.PropertyType);
                    jsonSchemaPropertyForObject.Properties.Add(propertyInfo.Name, jsonSchemaProperty);
                }

                return jsonSchemaPropertyForObject;

            case null:
                return Null;
        }
    }

    private static JsonSchemaProperty ConvertType(Type type)
    {
        if (type == typeof(bool) || type == typeof(bool?))
        {
            return Boolean;
        }

        if (type == typeof(byte) || type == typeof(byte?))
        {
            return Byte;
        }

        if (type == typeof(DateTime) || type == typeof(DateTime?))
        {
            return Date;
        }

        if (type == typeof(float) || type == typeof(float?) || type == typeof(decimal) || type == typeof(decimal?))
        {
            return Float;
        }

        if (type == typeof(double) || type == typeof(double?))
        {
            return Double;
        }

        if (type == typeof(Guid) || type == typeof(Guid?))
        {
            return Guid;
        }

        if (type == typeof(int) || type == typeof(short) || type == typeof(int?) || type == typeof(short?))
        {
            return Integer;
        }

        if (type == typeof(long) || type == typeof(long?))
        {
            return Long;
        }

        if (type == typeof(string))
        {
            return String;
        }

        if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
        {
            return TimeSpan;
        }

        if (type == typeof(Uri))
        {
            return Uri;
        }

        return Object;
    }
}