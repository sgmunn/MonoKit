namespace MonoKit.Data
{
    using System;
    using MonoKit.Tasks;

    public static class SyncScheduler
    {
        public static SyncTaskScheduler TaskScheduler = new SyncTaskScheduler();
    }
}

