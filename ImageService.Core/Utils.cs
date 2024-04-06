using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Core
{
    public static class Utils
    {
        public static async Task<(int From, O Item)> AnyMethodAsync<I, F, O>(this IEnumerable<I> src, Func<I, F> selector, Func<F, Task<O>> operation) 
        {
            int count = 0;

            foreach (var s in src) 
            {
                try
                {
                    var ret = await operation(selector(s));
                    return (count, ret);
                }
                catch (Exception e) { }

                count++;
            }

            throw new InvalidOperationException("Provided methods didn't return any data");
        }

        public static async Task<bool> AllMethodsAsync<I, F>(this IEnumerable<I> src, Func<I, F> selector, Func<F, Task> operation) 
        {
            foreach (var s in src)
            {
                try
                {
                    await operation(selector(s));
                }
                catch(Exception e)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
