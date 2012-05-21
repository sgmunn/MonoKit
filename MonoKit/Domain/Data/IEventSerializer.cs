namespace MonoKit.Domain.Data
{
    using System;

    public interface IEventSerializer
    {
        string SerializeToString(object instance);

        object DeserializeFromString(string data);
    }
}

