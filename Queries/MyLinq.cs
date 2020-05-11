using System;
using System.Collections.Generic;
using System.Text;

namespace Queries
{
    public static class MyLinq
    {
        public static IEnumerable<double> Random()
        {
            var random = new Random();
            while (true)
            {
                yield return random.NextDouble();
            }
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> source,
                                                Func<T,bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item; //"yield" is doing Defer Excusion
                    //once foreach loop finds item that satisfy the condition in the Main program
                    //it Yields/Passes that item to Foreach loop in the Main program, that loop 
                    //writes item to console and then this Foreach loop continue
                }
            }            
        }
    }
}
