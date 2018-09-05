using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAsToy.Coroutine
{
    internal interface IRunnable : IYieldable
    {
        IYieldable RunNext();
    }
}
