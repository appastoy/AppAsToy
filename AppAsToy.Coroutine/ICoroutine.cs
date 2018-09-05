
namespace AppAsToy.Coroutine
{
    public interface ICoroutine : IYieldable
    {
        void Stop();
    }

    public interface ICoroutine<TResult> : ICoroutine, IYieldable<TResult>
    {

    }
}
