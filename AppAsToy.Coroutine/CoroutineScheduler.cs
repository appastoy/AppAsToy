using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AppAsToy.Coroutine
{
    public sealed class CoroutineScheduler : IDisposable
    {
        internal struct CoroutineContextSection : IDisposable
        {
            readonly CoroutineScheduler scheduler;
            readonly Coroutine previousContext;

            internal CoroutineContextSection(CoroutineScheduler scheduler, Coroutine context)
            {
                this.scheduler = scheduler;
                previousContext = scheduler.currentCoroutine;
                scheduler.currentCoroutine = context;
            }

            public void Dispose()
            {
                scheduler.currentCoroutine = previousContext;
            }
        }

        internal struct RunnableContextSection : IDisposable
        {
            readonly CoroutineScheduler scheduler;
            readonly Routine previousContext;

            internal RunnableContextSection(CoroutineScheduler scheduler, IRunnable context)
            {
                this.scheduler = scheduler;
                previousContext = scheduler.currentRoutine;
                scheduler.currentRoutine = context;
            }

            public void Dispose()
            {
                scheduler.currentRoutine = previousContext;
            }
        }

        List<Coroutine> coroutines = new List<Coroutine>();
        Coroutine currentCoroutine;
        Routine currentRoutine;

        public ICoroutine StartCoroutine(IEnumerator<IYieldable> routineEnumerator, [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            var coroutine = new Coroutine();
            coroutine.Initialize(routineEnumerator, filePath, lineNumber);
            RunAndAdd(coroutine);

            return coroutine;
        }

        public ICoroutine<TResult> StartCoroutine<TResult>(IEnumerator<IYieldable> routineEnumerator, [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            var coroutine = new Coroutine<TResult>();
            coroutine.Initialize(routineEnumerator, filePath, lineNumber);
            RunAndAdd(coroutine);

            return coroutine;
        }

        void RunAndAdd(Coroutine coroutine)
        {
            RunCoroutine(coroutine);
            if (!coroutine.IsFinished)
            {
                coroutines.Add(coroutine);
            }
        }

        void RunCoroutine(Coroutine coroutine)
        {
            using (new CoroutineContextSection(this, coroutine))
            {
                if (!TryRunYieldable(currentCoroutine))
                {
                    RunRunnable(currentCoroutine);
                }
            }
        }

        bool TryRunYieldable(Coroutine coroutine)
        {
            bool isYielded = false;
            while (currentCoroutine.Count > 0)
            {
                var yieldable = currentCoroutine.Peek();
                if (yieldable.IsFinished)
                {
                    currentCoroutine.Pop();
                    continue;
                }

                ProcessYield(yieldable);
                isYielded = true;
                break;
            }

            return isYielded;
        }

        void RunRunnable(IRunnable runnable)
        {
            var yieldable = runnable.RunNext();
            if (!runnable.IsFinished)
            {
                ProcessYield(yieldable);
            }
        }

        void ProcessYield(IYieldable yieldable)
        {
            if (yieldable == null && yieldable.IsFinished) { return; }

            currentCoroutine.Push(yieldable);
            if (yieldable is IRunnable runnable)
            {
                using (new RunnableContextSection(this, runnable))
                {
                    RunRunnable(runnable);
                }
            }
            
            // TODO: Yieldable 타입에 따른 처리
            // WaitForAnimationEnd
            // WaitForFixedUpdateEnd
        }

        

        // TODO: Update 짜기
        // 직접 코루틴 업데이트 돌리기


        public void SetResult<TResult>(TResult result)
        {
            var currentCoroutineT = currentCoroutine as IYieldable<TResult>;
            if (currentCoroutineT == null)
            {
                throw new Exception("현재 코루틴은 결과를 설정할 수 없습니다.");
            }

            currentCoroutineT.Result = result;
        }

        public ISubRoutine WaitSubRoutine(IEnumerator<IYieldable> routineEnumerator, [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            var currentCoroutine = currentRoutine as Coroutine;
            if (currentCoroutine != null)
            {
                throw new Exception("코루틴이 시작되지 않은 상태에서 입니다.");
            }

            var subRoutine = new SubRoutine();
            subRoutine.Initialize(routineEnumerator, filePath, lineNumber);

            return subRoutine;
        }

        public void Dispose()
        {
            coroutines.ForEach(coroutine => coroutine.Dispose());
            coroutines.Clear();
        }
    }
}
