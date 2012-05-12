namespace MonoKit.Domain.Data
{
    using System;

    public interface ISerializer
    {
        string SerializeToString(object instance);

        object DeserializeFromString(string data);
    }
}

