namespace MonoKit.Tasks
{
    using System;

    public class SyncTaskScheduler : LimitedConcurrencyLevelTaskScheduler 
    {
        public SyncTaskScheduler() : base(1)
        {
        }
    }
}

