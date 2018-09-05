using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAsToy.Coroutine
{
    internal class RoutineRunner
    {
        IEnumerator<IYieldable> routineEnumerator;

        public string FilePath { get; private set; } = string.Empty;
        public int LineNumber { get; private set; }
        public bool IsFinished { get; private set; }

        internal void Initialize(IEnumerator<IYieldable> routineEnumerator, string filePath, int lineNumber)
        {
            this.routineEnumerator = routineEnumerator;
            FilePath = filePath;
            LineNumber = lineNumber;
        }

        public IYieldable RunNext()
        {
            if (IsFinished) { return null; }

            if (!routineEnumerator.MoveNext())
            {
                Dispose();
                return null;
            }

            return routineEnumerator.Current;
        }

        public void Dispose()
        {
            if (routineEnumerator != null)
            {
                routineEnumerator.Dispose();
                routineEnumerator = null;
            }
            IsFinished = true;
        }
    }
}
