using System;

namespace AppAsToy.Coroutine
{
    public interface IYieldable : IDisposable
    {
        bool IsFinished { get; }
    }

    public interface IYieldable<TResult> : IYieldable
    {
        TResult Result { get; }
    }
}
