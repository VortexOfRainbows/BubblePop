using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public static class JsonUtils
{
    public static object ToNativeType(this JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                var dict = new Dictionary<string, object>();
                foreach (var prop in ((JObject)token).Properties())
                {
                    dict[prop.Name] = prop.Value.ToNativeType();
                }
                return dict;

            case JTokenType.Array:
                var list = new List<object>();
                foreach (var item in (JArray)token)
                {
                    list.Add(item.ToNativeType());
                }
                return list;

            case JTokenType.Null:
            case JTokenType.None:
                return null;

            default:
                // Returns standard primitive types (string, long, double, boolean, etc.)
                return ((JValue)token).Value;
        }
    }
}
