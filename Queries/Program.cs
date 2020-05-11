using System;
using System.Linq;
using System.Collections.Generic;
using static System.Console;

namespace Queries
{
    class Program
    {
        static void Main(string[] args)
        {
            var numbers = MyLinq.Random().Where(n => n > 0.5).Take(10); //exp of Deffered Streaming operators
            foreach (var number in numbers)
            {
                WriteLine(number);
            }

            WriteLine();
            var movies = new List<Movie>
            {
                new Movie { Title = "The Dark Knight", Rating = 8.9f, Year = 2008},
                new Movie { Title = "The King's Speech", Rating = 8.0f, Year = 2010},
                new Movie { Title = "Casablanca", Rating = 8.5f, Year = 1942},
                new Movie { Title = "Star Wars V", Rating = 8.7f, Year = 1980 }
            };

            var query = movies.Where(m => m.Year > 2000) //"where" - Streaming Operator-looks at each item and if finds 
                                                        //a match Returns/Yeilds that match to main code right away, then goes back to eterate futher
                                .OrderByDescending(m => m.Rating);// - NonStreaming Operator-iterate through ALL items and then goes back to the main code

            //foreach (var movie in query)
            //{
            //    WriteLine(movie.Title);
            //}

            //Console.WriteLine(query.Count());
            var enumerator = query.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current.Title);
            }
        

            ReadKey();
        }
    }
}
