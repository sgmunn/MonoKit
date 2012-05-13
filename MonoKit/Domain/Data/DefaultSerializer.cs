namespace MonoKit.Domain.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    
    // todo: register event / command and state types 
    public  class DefaultSerializer<T> : ISerializer
    {
        private  readonly DataContractSerializer singletonInstance;

        public DefaultSerializer()
        {
            singletonInstance = new DataContractSerializer(
                typeof(T),
                new List<Type> {  typeof(EventBase), typeof(CreatedEvent), typeof(TestEvent2), typeof(NextEvent)}
                );
        }

        public  object DeserializeFromString(string value)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(value);
                writer.Flush();
                stream.Position = 0;

                return (T)singletonInstance.ReadObject(stream);
            }
        }

        public  string SerializeToString(object value)
        {
            using (var stream = new MemoryStream())
            {
                singletonInstance.WriteObject(stream, value);
                stream.Flush();
                stream.Position = 0;
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }
    }
}