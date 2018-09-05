using System.Collections.Generic;

namespace AppAsToy.Coroutine
{
    internal class Coroutine : IRunnable, ICoroutine
    {
        RoutineRunner routineRunner = new RoutineRunner();

        List<IYieldable> yieldableStack = new List<IYieldable>();
        public int Count { get { return yieldableStack.Count; } }
        public bool IsFinished { get { return routineRunner.IsFinished; } }
        
        internal void Initialize(IEnumerator<IYieldable> routineEnumerator, string filePath, int lineNumber)
        {
            routineRunner.Initialize(routineEnumerator, filePath, lineNumber);
        }

        public void Push(IYieldable yieldable)
        {
            yieldableStack.Add(yieldable);
        }

        public IYieldable Peek()
        {
            return yieldableStack[yieldableStack.Count - 1];
        }

        public void Pop()
        {
            yieldableStack.RemoveAt(yieldableStack.Count - 1);
        }

        public IYieldable RunNext()
        {
            return routineRunner.RunNext();
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            DisposeYieldableStack();
            routineRunner.Dispose();
        }

        void DisposeYieldableStack()
        {
            if (yieldableStack.Count == 0) { return; }

            for (int i = yieldableStack.Count - 1; i >= 0; i--)
            {
                var yieldable = yieldableStack[i];
                if (yieldable is Routine routine)
                {
                    routine.Dispose();
                }
            }
            yieldableStack.Clear();
        }
    }

    internal class Coroutine<TResult> : Coroutine, ICoroutine<TResult>
    {
        public TResult Result { get; set; }
    }
}
