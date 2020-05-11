using System;
using System.Collections.Generic;
using System.Text;

namespace Features.Linq
{
    public static class MyLinq
    {
        public static int Count<T> (this IEnumerable<T> sequence) 
        //"this" makes this method to be used as extension method on class IEnumerable
        {
            int count = 0;
            foreach (var item in sequence)
            {
                count += 1;
            }
            return count;
        }
    }
}
