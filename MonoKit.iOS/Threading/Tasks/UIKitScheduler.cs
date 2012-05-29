//  --------------------------------------------------------------------------------------------------------------------
//  https://gist.github.com/1431457
//  --------------------------------------------------------------------------------------------------------------------
 
namespace MonoKit.Threading.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MonoTouch.Foundation;

    public class UIKitScheduler : NSRunLoopScheduler
    {
        protected override void QueueAction (NSAction action)
        {
            _runLoop.BeginInvokeOnMainThread (action);
        }
    }

    public class NSRunLoopScheduler : TaskScheduler
    {
        class ScheduledTask
        {
            public Task TheTask;
            public bool ShouldRun = true;
            public bool IsRunning = false;
        }

        object _taskListLock = new object ();
        List<ScheduledTask> _taskList = new List<ScheduledTask> ();

        protected NSRunLoop _runLoop;

        public NSRunLoopScheduler ()
        {
            _runLoop = NSRunLoop.Current;
            if (_runLoop == null)
                throw new InvalidOperationException ("Cannot create scheduler on thread without an NSRunLoop");
        }

        public NSRunLoopScheduler (NSRunLoop runLoop)
        {
            if (runLoop == null)
                throw new ArgumentNullException ("runLoop");
            _runLoop = runLoop;
        }

        public override int MaximumConcurrencyLevel {
            get {
                return int.MaxValue;
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks ()
        {
            lock (_taskListLock) {
                return _taskList.Select (x => x.TheTask).ToList ();
            }
        }

        protected virtual void QueueAction (NSAction action)
        {
            throw new NotImplementedException ("NSRunLoopScheduler doesn't work yet");
        }

        protected override void QueueTask (Task task)
        {
            if (task == null)
                throw new ArgumentNullException ("task");

            var t = new ScheduledTask () { TheTask = task };

            lock (_taskListLock) {
                //
                // Cleanout the task list before adding this new task
                //
                _taskList = _taskList.Where (x => x.ShouldRun && !x.IsRunning).ToList ();
                _taskList.Add (t);
            }

            QueueAction (delegate {
                if (t.ShouldRun) {
                    t.IsRunning = true;
                    base.TryExecuteTask (t.TheTask);
                }
            });
        }

        protected override bool TryDequeue (Task task)
        {
            var t = default (ScheduledTask);

            lock (_taskListLock) {
                t = _taskList.FirstOrDefault (x => x.TheTask == task);
            }

            if (t != null && !t.IsRunning) {
                t.ShouldRun = false;
                return !t.IsRunning;
            } else {
                return false;
            }
        }

        protected override bool TryExecuteTaskInline (Task task, bool taskWasPreviouslyQueued)
        {
            if (task == null)
                throw new ArgumentNullException ("task");

            //
            // Are we in the right NSRunLoop?
            //
            var curRunLoop = NSRunLoop.Current;

            if ((curRunLoop != null) && (curRunLoop.Handle == _runLoop.Handle)) {

                //
                // Our dequeue is really simple, so just say no if this
                // task was queued before
                //
                if (taskWasPreviouslyQueued)
                    return false;

                //
                // Run it on this thread
                //
                return base.TryExecuteTask (task);

            } else {
                return false;
            }
        }
    }
}
