using System;
using System.Collections.Generic;
using static System.Console;
//using Features.Linq;
using System.Linq;

namespace Features
{
    class Program
    {
        static void Main(string[] args)
        {
            //this method returns type of the last parameter
            Func<int, int> square = x => x * x;
            Func<int, int, int> add = (x, y) => x + y;

            //this method returns void
            Action<int> write = x => Console.WriteLine(x);

            WriteLine(square(add(3,5)));
            write(5);
            WriteLine();

            var developers = new Employee[]
            {
                new Employee { Id = 1, Name = "Scott" },
                new Employee { Id = 2, Name = "Chris" }
            };

            var Sales = new List<Employee>
            {
                new Employee { Id = 3, Name = "Alex"}
            };

            //method sintax of writing linq
            var query = developers.Where(e => e.Name.Length == 5).OrderByDescending(e => e.Name);

            //query sintax of writing linq
            var query2 = from employee in developers
                         where employee.Name.Length == 5
                         orderby employee.Name descending
                         select employee;

            foreach (var empl in query2)
            {
                WriteLine(empl.Name);
            }

            WriteLine("\nPress any key to exit...");
            ReadKey();
        }

        private static bool NameStartsWithS(Employee employee)
        {
            return employee.Name.StartsWith("S");
        }
    }
}
