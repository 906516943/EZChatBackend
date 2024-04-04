﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Core
{
    public static class Utils
    {
        public static async Task<(int From, O? Item)> AnyMethodAsync<I, F, O>(this IEnumerable<I> src, Func<I, F> selector, Func<F, Task<O>> operation) 
        {
            int count = 0;
            O? ret = default(O);


            foreach (var s in src) 
            {
                try
                {
                    ret = await operation(selector(s));
                    return (count, ret);
                }
                catch (InvalidOperationException e) { }

                count++;
            }

            return (-1, default(O));
        }
    }
}