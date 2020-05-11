using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using static System.Console;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            //if see that table is different from our Car class, wipes db and create new one !not do it on Production
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>()); 


            InsertData();
            QueryData();
            
            //XML
            //CreateXML();
            //QueryXML();

            #region working with LINQ queries
            //----working with LINQ queries
            //var manufacturers = ProcessManufacturers("manufacturers.csv");


            //var query =
            //            from car in cars
            //            group car by car.Manufacturer into carGroup
            //            select new
            //            {
            //                Name = carGroup.Key,
            //                //looping 3 TIMES through all cars' list - see below to use Aggregate instead
            //                Max = carGroup.Max(c => c.Combined),
            //                Min = carGroup.Min(c => c.Combined),
            //                Average = carGroup.Average(c => c.Combined)
            //            } into result
            //            orderby result.Max descending
            //            select result;


            //var query2 =
            //    cars.GroupBy(c => c.Manufacturer)
            //        .Select(g =>
            //                    {
            //                        //using Aggregate we loop only ONCE through all cars' list
            //                        var results = g.Aggregate(new CarStatictics(),
            //                                            (acc, c) => acc.Accumulate(c),
            //                                            acc => acc.Compute());
            //                        return new
            //                        {
            //                            Name = g.Key,
            //                            Avg = results.Average,
            //                            Max = results.Max,
            //                            Min = results.Min
            //                        };
            //                    })
            //                    .OrderBy(r => r.Max);


            //foreach (var result in query)
            //{
            //    Console.WriteLine($"{result.Name}");
            //    Console.WriteLine($"\tMax : {result.Max}");
            //    Console.WriteLine($"\tMin : {result.Min}");
            //    Console.WriteLine($"\tAverage : {result.Average}");
            //}

            //JOINS
            //var query = from car in cars
            //            join manufacturer in manufacturers on car.Manufacturer equals manufacturer.Name
            //            orderby car.Combined descending, car.Name ascending
            //            select new
            //            {
            //                manufacturer.Headquarters,
            //                car.Name,
            //                car.Combined
            //            };



            //var query2= cars.Join(manufacturers,
            //                     c => c.Manufacturer,
            //                     m => m.Name,
            //                     (c,m) => new //putting results into new IEnumerable collection
            //                     {
            //                         m.Headquarters,
            //                         c.Name,
            //                         c.Combined
            //                     })
            //                .OrderByDescending(c => c.Combined)
            //                .ThenBy(c => c.Name);

            //var result = cars.SelectMany(c => c.Name)
            //                 .OrderBy(c => c);

            //foreach(var character in result)
            //{
            //    WriteLine(character);
            //}

            //SelectMany iterates through sequence of each item in original sequence
            //foreach (var name in result)
            //{
            //  foreach (var character in name)
            //    {
            //        WriteLine(character);
            //    }
            //}


            //foreach (var car in query.Take(10))
            //{
            //    WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}");
            //}
            #endregion


            WriteLine("\nPress any key to exit...");
            ReadKey();
        }

        private static void QueryData()
        {
            var db = new CarDb();

            //turning logging on to sql statements that are issued to sql server
            db.Database.Log = Console.WriteLine;
            
            #region simple queries
            //if IQueryable is used - EF translate our linq statements into sql queries 
            //and then compiler uses those sql to query data
            //(not all statements can be translated: ex: string.split(' '))

            //if IEnumerable is used - then compiler translates our linq into sql queries
            // and that HAPPENS IN MEMORY ex: .toList()

            //to see what interface is used - hover over linq operator: .Where ect

            var query = db.Cars.Where(c => c.Manufacturer == "BMW")
                                .OrderByDescending(c => c.Combined)
                                .ThenBy(c => c.Name)
                                 .Take(10);



            foreach (var car in query)
            {
                Console.WriteLine($"{car.Name}: {car.Combined}");
            }

            #endregion

            var query2 =
                    from car in db.Cars
                    group car by car.Manufacturer into manufacturer
                    select new
                    {
                        Name = manufacturer.Key,
                        Cars = manufacturer.OrderByDescending(c => c.Combined).Take(2)
                    };

            foreach (var group in query2)
            {
                Console.WriteLine(group.Name);
                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t{car.Name}: {car.Combined}");
                }
            }
        }

        private static void InsertData()
        {
            var cars = ProcessCars("fuel.csv");
            var db = new CarDb();

            if (!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }
                db.SaveChanges();
            }
        }

        private static void QueryXML()
        {
            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex";
            var document = XDocument.Load("fuel.xml");

            var query = document.Element(ns + "Cars")?.Elements(ex + "Car").Where(c => c.Attribute("Manufacturer")?.Value == "BMW").Select(n => n.Attribute("Name").Value);

            foreach (var name in query)
            {
                WriteLine(name);
            }
        }

        private static void CreateXML()
        {
            var records = ProcessCars("fuel.csv");

            var ns = (XNamespace)"http://pluralsight.com/cars/2016";
            var ex = (XNamespace)"http://pluralsight.com/cars/2016/ex"; // ex stands for extended namespace
            var document = new XDocument();
            var cars = new XElement(ns + "Cars",

                        from record in records
                        select new XElement(ex + "Car",
                                            new XAttribute("Name", record.Name),
                                            new XAttribute("Combined", record.Combined),
                                            new XAttribute("Manufacturer", record.Manufacturer))
                                            );

            //adding alias reduces the file size
            cars.Add(new XAttribute(XNamespace.Xmlns + "ex", ex));

            document.Add(cars);
            document.Save("fuel.xml");
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query = File.ReadAllLines(path)
                            .Where(l => l.Length > 1)
                            .Select(l =>
                            {
                                var columns = l.Split(',');
                                return new Manufacturer
                                {
                                    Name = columns[0],
                                    Headquarters = columns[1],
                                    Year = int.Parse(columns[2])
                                };
                            });

            return query.ToList();
        }

        public class CarStatictics
        {
            public CarStatictics()
            {
                Max = Int32.MinValue;
                Min = Int32.MaxValue;
            }

            public CarStatictics Accumulate(Car car)
            {
                Count += 1;
                Total += car.Combined;
                Max = Math.Max(Max, car.Combined);
                Min = Math.Min(Min, car.Combined);

                return this;
            }

            public CarStatictics Compute()
            {
                Average = Total / Count;
                return this;
            }

            public int Max { get; set; }
            public int Min { get; set; }
            public int Count { get; set; }
            public int Total { get; set; }
            public double Average { get; set; }
        }
        private static List<Car> ProcessCars(string path)
        {
            return
                File.ReadAllLines(path)
                    .Skip(1)
                    .Where(line => line.Length > 1) //cutting off last empty line
                    .Select(Car.ParseFromCsv)
                    .ToList();                    
        }
    }
}
