using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ATM.Api;
using Newtonsoft.Json;

namespace AtmApp.Helpers
{
    public class JSONSerializer
    {
        public static void Save(object o, bool indented = true)
        {
            var settings = new JsonSerializerSettings() { ContractResolver = new AllFieldsContractResolver() };
            if (indented)
            {
                settings.Formatting = Formatting.Indented;
            }
            string json = JsonConvert.SerializeObject(o, settings);
            File.WriteAllText(@"C:\Windows\Temp\atm.json", json);
        }

        public static ATMachine Load(bool indented = true)
        {
            ATMachine myObj = null;
            try
            {
                var settings = new JsonSerializerSettings() { ContractResolver = new AllFieldsContractResolver() };
                if (indented)
                {
                    settings.Formatting = Formatting.Indented;
                }
                /*using (StreamReader file = File.OpenText(@"C:\Windows\Temp\atm.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    myObj2 = (ATMachine)serializer.Deserialize(file, typeof(ATMachine));
                }*/
                string json = File.ReadAllText(@"C:\Windows\Temp\atm.json");
                myObj = JsonConvert.DeserializeObject<ATMachine>(json);
            }
            catch (Exception)
            {
            }
            return myObj;
        }
    }

    public class AllFieldsContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            var props = type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(p => base.CreateProperty(p, memberSerialization))
                .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(f => base.CreateProperty(f, memberSerialization)))
                .Where(p => !p.PropertyName.Contains("k__BackingField"))
                .ToList();
            props.ForEach(p => { p.Writable = true; p.Readable = true; });
            return props;
        }
    }

    /*public class ForceJSONSerializePrivatesResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
        {
            var props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            List<Newtonsoft.Json.Serialization.JsonProperty> jsonProps = new List<Newtonsoft.Json.Serialization.JsonProperty>();

            foreach (var prop in props)
            {
                jsonProps.Add(base.CreateProperty(prop, memberSerialization));
            }

            foreach (var field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
            {
                jsonProps.Add(base.CreateProperty(field, memberSerialization));
            }

            jsonProps.ForEach(p => { p.Writable = true; p.Readable = true; });
            return jsonProps;
        }
    }*/
}