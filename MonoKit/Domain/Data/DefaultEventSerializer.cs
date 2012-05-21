namespace MonoKit.Domain.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Reflection;
    using System.Linq;

    public static class KnownTypes
    {
        public static List<Type> EventTypes = new List<Type>();

        public static void RegisterEvents(Assembly assembly)
        {
            Assembly.GetCallingAssembly();

            var eventTypes = assembly.GetTypes().Where(t => typeof(IEvent).IsAssignableFrom(t)).ToList();
            EventTypes.AddRange(eventTypes);
        }
    }
    
    public class DefaultEventSerializer : IEventSerializer
    {
        private readonly DataContractSerializer serializer;

        static DefaultEventSerializer()
        {
            KnownTypes.RegisterEvents(Assembly.GetExecutingAssembly());
        }

        public DefaultEventSerializer()
        {
            this.serializer = new DataContractSerializer(typeof(EventBase), KnownTypes.EventTypes);
        }

        public object DeserializeFromString(string value)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(value);
                    writer.Flush();
                    stream.Position = 0;

                    var result = this.serializer.ReadObject(stream);
                    return result;
                }
            }
        }

        public string SerializeToString(object value)
        {
            using (var stream = new MemoryStream())
            {
                this.serializer.WriteObject(stream, value);
                stream.Flush();
                stream.Position = 0;

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}