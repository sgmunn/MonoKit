// --------------------------------------------------------------------------------------------------------------------
// <copyright file=".cs" company="sgmunn">
//   (c) sgmunn 2012  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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

            var eventTypes = assembly.GetTypes().Where(t => typeof(IAggregateEvent).IsAssignableFrom(t)).ToList();
            EventTypes.AddRange(eventTypes);
        }
    }

    public class DefaultEventSerializer<T> : IEventSerializer
        where T : class, new()
    {
        private readonly DataContractSerializer serializer;

        static DefaultEventSerializer()
        {
            KnownTypes.RegisterEvents(Assembly.GetExecutingAssembly());
        }

        public DefaultEventSerializer()
        {
            this.serializer = new DataContractSerializer(typeof(T), KnownTypes.EventTypes);
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