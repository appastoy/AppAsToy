using System.Collections.Generic;

namespace AppAsToy.Coroutine
{
    internal class Routine : IRunnable
    {
        RoutineRunner routineRunner = new RoutineRunner();

        public bool IsFinished { get { return routineRunner.IsFinished; } }

        internal void Initialize(IEnumerator<IYieldable> routineEnumerator, string filePath, int lineNumber)
        {
            routineRunner.Initialize(routineEnumerator, filePath, lineNumber);
        }

        public IYieldable RunNext()
        {
            return routineRunner.RunNext();
        }

        public void Dispose()
        {
            routineRunner.Dispose();
        }
    }

    internal class Routine<TResult> : Routine, IYieldable<TResult>
    {
        public TResult Result { get; set; }
    }
}
