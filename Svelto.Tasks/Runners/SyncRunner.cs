
using System.Collections;
using Svelto.Tasks.Internal;
using Svelto.Utilities;

namespace Svelto.Tasks
{
    //Be sure you know what you are doing when you are using
    //the Sync runner, it will stall the current thread!
    //Depending by the case, it may be better to
    //use the ManualResetEventEx synchronization instead.

    public class SyncRunner : SyncRunner<IEnumerator>, IRunner
    {}

    public class SyncRunner<T> : IRunner<T> where T:IEnumerator
    {
        public bool paused { set; get; }
        public bool isStopping { private set; get; }

        public void StartCoroutine(IPausableTask<T> task)
        {
            while (task.MoveNext() == true) ThreadUtility.Yield();
        }

        /// <summary>
        /// TaskRunner doesn't stop executing tasks between scenes
        /// it's the final user responsability to stop the tasks if needed
        /// </summary>
        public void StopAllCoroutines()
        {}

        public void Dispose()
        {}

        public int numberOfRunningTasks { get { return -1; } }
    }
}
