using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using PropertyDescriptor = System.ComponentModel.PropertyDescriptor;

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

        public static object Deserialize2(string path)
        {
            using (var r = new StringReader(File.ReadAllText(path)))
                return new Deserializer().Deserialize(r);
        }

        public static void Serialize(List<KeyValuePair<object, object>> ymlObj, string path)
        {
            /*
            var obj = ymlObj.ToDictionary(t => t.Key, t => t.Value);
            var serializer = new SerializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
            var yamlTxt = serializer.Serialize(obj);
            File.WriteAllText(path, yamlTxt);
            */
            var obj = ymlObj.ToDictionary(t => t.Key, t => t.Value);
            var serializer = new SerializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
            var yamlTxt = serializer.Serialize(obj);
            File.WriteAllText(path, yamlTxt);
        }

        public static string Lookup(dynamic ymlObj, string inputKey)
        {
            string result = "";

            var query = new YmlQuery(ymlObj);
            var data = query.On("Name");
            // If object is a list of key/value pairs of objects...
            if (ymlObj.GetType() == typeof(List<KeyValuePair<object, object>>))
            {
                var list = (List<KeyValuePair<object, object>>)ymlObj;
                // For each item in list...
                for (int i = 0; i < list.Count; i++)
                {
                    var dic = list[i].ToDictionary();
                    // If dictionary contains key...
                    if (dic.ContainsKey(inputKey))
                    {
                        dic.TryGetValue(inputKey, out var value);
                        if (value != null)
                        {
                            if (value.GetType() == typeof(string))
                                result = value.ToString();
                            else if (value.GetType() == typeof(List<KeyValuePair<object, object>>))
                            {
                                result = Lookup(value, inputKey);
                            }
                        }
                    }
                    else
                    {
                        // See if any of the values has the key...
                        foreach (var value in dic.Values)
                        {
                            if (value.GetType() == typeof(string)) { } // Section Name
                            else if (value.GetType() == typeof(List<KeyValuePair<object, object>>))
                            {
                                result = Lookup(value, inputKey);
                            }
                            else if (value.GetType() == typeof(List<object>))
                            {
                                List<KeyValuePair<object, object>> newList = new List<KeyValuePair<object, object>>();
                                foreach (List<object> obj in (List<object>)value)
                                {
                                    var kvp = new KeyValuePair<object, object>(obj[0], obj[1]);
                                    newList.Add(kvp);
                                }
                                result = Lookup(newList, inputKey);
                            }
                            else
                                throw new Exception($"{value.GetType().ToString()}");
                        }
                    }
                }
            }
            else
                throw new Exception($"{ymlObj.GetType().ToString()}");

            if (result == "")
                throw new Exception($"{ymlObj.GetType().ToString()}");
            return result;
        }

        static Dictionary<string, object> ToDictionary(object obj)
        {
            int i = 0;
            var props = obj.GetType().GetProperties();
            return props.ToDictionary(k => props[i].Name, v => props[i++].GetValue(obj));
        }

        public static Dictionary<string, object> ConvertFromObjectToDictionary(object arg)
        {
            return arg.GetType().GetProperties().ToDictionary(property => property.Name, property => property.GetValue(arg));
        }
    }

    public static class DynamicKeyValueHelper
    {
        public static KeyValuePair<object, object> Cast<K, V>(this KeyValuePair<K, V> kvp)
        {
            return new KeyValuePair<object, object>(kvp.Key, kvp.Value);
        }

        public static KeyValuePair<T, V> CastFrom<T, V>(Object obj)
        {
            return (KeyValuePair<T, V>)obj;
        }

        public static KeyValuePair<object, object> CastFrom(Object obj)
        {
            var type = obj.GetType();
            if (type.IsGenericType)
            {
                if (type == typeof(KeyValuePair<,>))
                {
                    var key = type.GetProperty("Key");
                    var value = type.GetProperty("Value");
                    var keyObj = key.GetValue(obj, null);
                    var valueObj = value.GetValue(obj, null);
                    return new KeyValuePair<object, object>(keyObj, valueObj);
                }
            }
            throw new ArgumentException(" ### -> public static KeyValuePair<object , object > CastFrom(Object obj) : Error : obj argument must be KeyValuePair<,>");
        }
    }

    public static class ObjectToDictionaryHelper
    {
        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }

        public static IDictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null)
                ThrowExceptionWhenSourceArgumentIsNull();

            var dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToDictionary<T>(property, source, dictionary);
            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value))
                dictionary.Add(property.Name, (T)value);
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
        }
    }

    public class YmlQuery
    {
        private object yamlDic;
        private string key;
        private object current;

        public YmlQuery(object yamlDic)
        {
            this.yamlDic = yamlDic;
        }

        public YmlQuery On(string key)
        {
            this.key = key;
            this.current = query<object>(this.current ?? this.yamlDic, this.key, null);
            return this;
        }
        public YmlQuery Get(string prop)
        {
            if (this.current == null)
                throw new InvalidOperationException();

            this.current = query<object>(this.current, null, prop, this.key);
            return this;
        }

        public List<T> ToList<T>()
        {
            if (this.current == null)
                throw new InvalidOperationException();

            return (this.current as List<object>).Cast<T>().ToList();
        }

        private IEnumerable<T> query<T>(object _dic, string key, string prop, string fromKey = null)
        {
            var result = new List<T>();
            if (_dic == null)
                return result;
            if (typeof(IDictionary<object, object>).IsAssignableFrom(_dic.GetType()))
            {
                var dic = (IDictionary<object, object>)_dic;
                var d = dic.Cast<KeyValuePair<object, object>>();

                foreach (var dd in d)
                {
                    if (dd.Key as string == key)
                    {
                        if (prop == null)
                        {
                            result.Add((T)dd.Value);
                        }
                        else
                        {
                            result.AddRange(query<T>(dd.Value, key, prop, dd.Key as string));
                        }
                    }
                    else if (fromKey == key && dd.Key as string == prop)
                    {
                        result.Add((T)dd.Value);
                    }
                    else
                    {
                        result.AddRange(query<T>(dd.Value, key, prop, dd.Key as string));
                    }
                }
            }
            else if (typeof(IEnumerable<object>).IsAssignableFrom(_dic.GetType()))
            {
                var t = (IEnumerable<object>)_dic;
                foreach (var tt in t)
                {
                    result.AddRange(query<T>(tt, key, prop, key));
                }

            }
            return result;
        }
    }
}
