using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.CacheManager
{

    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<T> valueFactory) :
            base(() => Task.Factory.StartNew(valueFactory))
        { }

        public AsyncLazy(string key, Func<string, Task<T>> taskFactory) :
            base(() => Task.Factory.StartNew(() => taskFactory(key)).Unwrap())
        { }

        public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
    }
}
