namespace MonoKit.Domain
{
    using System;

    public interface ISnapshotSupport
    {
        void LoadFromSnapshot(ISnapshot snapshot);
        
        ISnapshot GetSnapshot();
    }
}

