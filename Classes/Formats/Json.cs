using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.IO;

namespace ShrineFox.IO
{
    public class Json
    {
        public static JObject Deserialize(string jsonPath)
        {
            return JObject.Parse(File.ReadAllText(jsonPath));
        }

        public static void Serialize(JObject jsonData, string jsonPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(jsonPath));
            using (FileSys.WaitForFile(jsonPath)) { };
            File.WriteAllText(jsonPath, jsonData.ToString());
        }
    }

    public static class JTokenOverride
    {
        public static T GetValue<T>(this JToken jToken, string key, T defaultValue = default(T))
        {
            dynamic ret = jToken[key];
            if (ret == null) return defaultValue;
            if (ret is JObject) return JsonConvert.DeserializeObject<T>(ret.ToString());
            return (T)ret;
        }
    }
}