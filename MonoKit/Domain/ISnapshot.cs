namespace MonoKit.Domain
{
    using System;

    public interface ISnapshot
    {
        void LoadFromSnapshot(object snapshot, int snapshotVersion);
        
        object GetSnapshot();
    }
}

