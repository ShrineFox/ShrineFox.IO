using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ShrineFox.IO
{
    public class Yml
    {
        public static List<KeyValuePair<object, object>> Deserialize(string path)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
            List<KeyValuePair<object, object>> data = deserializer.Deserialize<IDictionary<object, object>>(File.ReadAllText(path)).ToList();
            return data;
        }

        public static void Serialize(List<KeyValuePair<object, object>> ymlObj, string path)
        {
            var obj = ymlObj.ToDictionary(t => t.Key, t => t.Value);
            var serializer = new SerializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
            var yamlTxt = serializer.Serialize(obj);
            File.WriteAllText(path, yamlTxt);
        }
    }
}
