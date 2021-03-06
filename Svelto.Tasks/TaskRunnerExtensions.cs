using System;
using Svelto.Tasks;
using System.Collections;
using Svelto.Utilities;

public static class TaskRunnerExtensions
{
    /// <summary>
    /// the first instructions until the first yield are executed immediately
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="task"></param>
    /// <returns></returns>
    public static ContinuationWrapper RunOnScheduler(this IEnumerator enumerator, IRunner runner)
    {
        return TaskRunner.Instance.RunOnScheduler(runner, enumerator);
    }

    public static ContinuationWrapper Run(this IEnumerator enumerator)
    {
        return TaskRunner.Instance.Run(enumerator);
    }
    
    public static void Complete(this IEnumerator enumerator, int _timeout = -1)
    {
        var quickIterations = 0;

        if (_timeout > 0)
        {
            DateTime then  = DateTime.Now.AddMilliseconds(_timeout);
            bool     valid = true;

            while (enumerator.MoveNext() && (valid = (DateTime.Now < then)))
                ThreadUtility.Wait(ref quickIterations);

            if (valid == false)
                throw new Exception("synchronous task timed out, increase time out or check if it got stuck");
        }
        else
        {
            while (enumerator.MoveNext())
                ThreadUtility.Wait(ref quickIterations);
        }
    }
    
    public static ParallelTaskCollection Combine(this IEnumerator enumerator, IEnumerator enumerator2)
    {
        var parallel = enumerator as ParallelTaskCollection;

        if (parallel == null)
        {
            parallel = new ParallelTaskCollection();
            parallel.Add(enumerator);
        }
        
        parallel.Add(enumerator2);

        return parallel;
    }
}

