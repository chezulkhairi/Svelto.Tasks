#if UNITY_5 || UNITY_5_3_OR_NEWER

using System.Collections;
using Svelto.DataStructures;
using Svelto.Tasks.Unity.Internal;
using UnityEngine;

#if TASKS_PROFILER_ENABLED
using Svelto.Tasks.Profiler;
#endif

namespace Svelto.Tasks.Unity
{
    
    /// while you can instantiate a MonoRunner, you should use the standard one
    /// whenever possible. Instantiating multiple runners will defeat the
    /// initial purpose to get away from the Unity monobehaviours
    /// internal updates. MonoRunners are disposable though, so at
    /// least be sure to dispose of them once done
    /// </summary>
    
    public abstract class MonoRunner<T> : IRunner<T> where T:IEnumerator
    {
        public bool paused { set; get; }
        public bool isStopping { get { return _flushingOperation.stopped; } }
        public int  numberOfRunningTasks { get { return _coroutines.Count; } }
        
        public GameObject _go;

        ~MonoRunner()
        {
            StopAllCoroutines();
        }
        
        /// <summary>
        /// TaskRunner doesn't stop executing tasks between scenes
        /// it's the final user responsibility to stop the tasks if needed
        /// </summary>
        public virtual void StopAllCoroutines()
        {
            paused = false;

            UnityCoroutineRunner<T>.StopRoutines(_flushingOperation);

            _newTaskRoutines.Clear();
        }

        public virtual void StartCoroutine(PausableTask<T> task)
        {
            paused = false;

            _newTaskRoutines.Enqueue(task); //careful this could run on another thread!
        }

        public virtual void Dispose()
        {
            StopAllCoroutines();
            
            GameObject.DestroyImmediate(_go);
        }
        
        protected readonly ThreadSafeQueue<PausableTask<T>> _newTaskRoutines = new ThreadSafeQueue<PausableTask<T>>();
        protected readonly FasterList<PausableTask<T>> _coroutines =
            new FasterList<PausableTask<T>>(NUMBER_OF_INITIAL_COROUTINE);
        
        protected UnityCoroutineRunner<T>.FlushingOperation _flushingOperation =
            new UnityCoroutineRunner<T>.FlushingOperation();
        
        const int NUMBER_OF_INITIAL_COROUTINE = 3;
    }
}
#endif
