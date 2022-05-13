# NJsonSchema.Extensions
Some extension methods for [NJsonSchema](https://github.com/RicoSuter/NJsonSchema).

[![NuGet Badge](https://buildstats.info/nuget/NJsonSchema.Extensions)](https://www.nuget.org/packages/NJsonSchema.Extensions)

## ToJsonSchema

### Convert a JObject to a NSwag JsonSchema

#### JObject
``` c#
var instance = new JObject
{
    {"Uri", new JValue(new Uri("http://localhost:80/abc?a=5"))},
    {"Null", new JValue((object?) null)},
    {"Guid", new JValue(_guid)},
    {"Float", new JValue(10.0f)},
    {"Double", new JValue(Math.PI)},
    {"Check", new JValue(true)},
    {
        "Child", new JObject
        {
            {"ChildInteger", new JValue(4)},
            {"ChildDateTime", new JValue(new DateTime(2018, 2, 17))},
            {"ChildTimeSpan", new JValue(TimeSpan.FromMilliseconds(999))}
        }
    },
    {"Integer", new JValue(9)},
    {"Long", new JValue(long.MaxValue)},
    {"String", new JValue("Test")},
    {"Char", new JValue('c')},
    {"Bytes", new JValue(_bytes)},
    {"Array", new JArray("a1")}
};
```

#### Call `ToJsonSchema`
``` c#
var schema = instance.ToJsonSchema().ToJson(Formatting.Indented).Replace("  ", "    ");
```

#### Generated Swagger Schema
``` json
{
    "$schema": "http://json-schema.org/draft-04/schema#",
    "type": "object",
    "properties": {
        "Uri": {
            "type": "string",
            "format": "uri"
        },
        "Null": {
            "type": "null"
        },
        "Guid": {
            "type": "string",
            "format": "guid"
        },
        "Float": {
            "type": "number",
            "format": "float"
        },
        "Double": {
            "type": "number",
            "format": "double"
        },
        "Check": {
            "type": "boolean"
        },
        "Child": {
            "type": "object",
            "properties": {
                "ChildInteger": {
                    "type": "integer",
                    "format": "int32"
                },
                "ChildDateTime": {
                    "type": "string",
                    "format": "date-time"
                },
                "ChildTimeSpan": {
                    "type": "string",
                    "format": "time-span"
                }
            }
        },
        "Integer": {
            "type": "integer",
            "format": "int32"
        },
        "Long": {
            "type": "integer",
            "format": "int64"
        },
        "String": {
            "type": "string"
        },
        "Char": {
            "type": "string"
        },
        "Bytes": {
            "type": "string",
            "format": "byte"
        },
        "Array": {
            "type": "array",
            "items": {
                "type": "string"
            }
        }
    }
}
```

---

### Convert an Jarray to a NSwag JsonSchema

#### Object
``` c#
var instance = new JArray("a1", "a2");
```

#### Call `ToJsonSchema`
``` c#
var schema = instance.ToJsonSchema().ToJson(Formatting.Indented).Replace("  ", "    ");
```

#### Generated Swagger Schema
``` json
{
    "$schema": "http://json-schema.org/draft-04/schema#",
    "type": "array",
    "items": {
        "type": "string"
    }
}
```

---

### Convert a object to a NSwag JsonSchema

#### object
``` c#
var instance = new
{
    Uri = new Uri("http://localhost:80/abc?a=5"),
    Null = (object?)null,
    Guid = _guid,
    Float = 10.0f,
    Double = Math.E,
    Check = true,
    Child = new
    {
        ChildInteger = 4,
        ChildDateTime = new DateTime(2018, 2, 17),
        ChildTimeSpan = TimeSpan.FromMilliseconds(999)
    },
    Integer = 9,
    Long = long.MaxValue,
    String = "test",
    Char = 'c',
    Bytes = _bytes,
    Array = new[] { "a1" },
    ListT = new List<int> { 1 },
    IList = (IList)new List<int> { 1 },
    IEnumerableT = (IEnumerable<string>)new[] { "s" },
    IEnumerable = (IEnumerable)new[] { "s" }
};
```

#### Call `ToJsonSchema`
``` c#
var schema = instance.ToJsonSchema().ToJson(Formatting.Indented).Replace("  ", "    ");
```

#### Generated Swagger Schema
``` json
{
    "$schema": "http://json-schema.org/draft-04/schema#",
    "type": "object",
    "properties": {
        "Uri": {
            "type": "string",
            "format": "uri"
        },
        "Null": {
            "type": "null"
        },
        "Guid": {
            "type": "string",
            "format": "guid"
        },
        "Float": {
            "type": "number",
            "format": "float"
        },
        "Double": {
            "type": "number",
            "format": "double"
        },
        "Check": {
            "type": "boolean"
        },
        "Child": {
            "type": "object",
            "properties": {
                "ChildInteger": {
                    "type": "integer",
                    "format": "int32"
                },
                "ChildDateTime": {
                    "type": "string",
                    "format": "date-time"
                },
                "ChildTimeSpan": {
                    "type": "string",
                    "format": "time-span"
                }
            }
        },
        "Integer": {
            "type": "integer",
            "format": "int32"
        },
        "Long": {
            "type": "integer",
            "format": "int64"
        },
        "String": {
            "type": "string"
        },
        "Char": {
            "type": "object"
        },
        "Bytes": {
            "type": "array",
            "items": {
                "type": "string",
                "format": "byte"
            }
        },
        "Array": {
            "type": "array",
            "items": {
                "type": "string"
            }
        },
        "ListT": {
            "type": "array",
            "items": [
                {
                    "type": "integer",
                    "format": "int32"
                }
            ]
        },
        "IList": {
            "type": "array",
            "items": [
                {
                    "type": "integer",
                    "format": "int32"
                }
            ]
        },
        "IEnumerableT": {
            "type": "array",
            "items": {
                "type": "string"
            }
        },
        "IEnumerable": {
            "type": "array",
            "items": {
                "type": "string"
            }
        }
    }
}
```
