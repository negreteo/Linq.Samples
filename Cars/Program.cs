using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cars
{
   class Program
   {
      static void Main (string[] args)
      {
         var cars = ProcessCars ("/Users/negreteo/src/Linq.Samples/Cars/fuel.csv");
         var manufactures = ProcessManufactures ("/Users/negreteo/src/Linq.Samples/Cars/manufacturers.csv");

         // Query syntax
         var query1 = from car in cars
         where car.Manufacturer == "BMW" && car.Year == 2016
         orderby car.Combined descending, car.Name ascending
         select new
         {
            car.Manufacturer,
            car.Name,
            car.Combined
         };

         // Method syntax
         var query2 = cars
            .Where (c => c.Manufacturer == "BMW" && c.Year == 2016)
            .OrderByDescending (c => c.Combined)
            .ThenBy (c => c.Name)
            .Select (c => new
            {
               c.Manufacturer,
                  c.Name,
                  c.Combined
            });
         //.First (c => c.Manufacturer =="BMW"); // First can also include filters
         //.Last (c => c.Manufacturer == "BMW"); // Last can also include filters

         // Returns top car filtered by manufacturer and year, orders by efficiency and name
         var topCars = cars
            .OrderByDescending (c => c.Combined)
            .ThenBy (c => c.Name)
            .FirstOrDefault (c => c.Manufacturer == "BMW" && c.Year == 2016); // returns null if not matching results are found         

         if (topCars != null) System.Console.WriteLine ($"Top BMW 2016 in the List: {topCars.Name}");
         System.Console.WriteLine ();

         // Returns any car filtered by manufacturer
         var result = cars.Any (c => c.Manufacturer == "Ford");
         System.Console.WriteLine ();
         System.Console.WriteLine ($"Any Ford Car: { result }");

         System.Console.WriteLine ();
         System.Console.WriteLine ("Most Efficient Cars");
         System.Console.WriteLine ();

         // Returns most efficient cars
         foreach (var car in query2.Take (10))
         {
            System.Console.WriteLine ($"{ car.Manufacturer }: { car.Name }: { car.Combined }");
         }

         // System.Console.WriteLine ();
         // System.Console.WriteLine ("Characters by Car");
         // // Iterates over the items inside each value
         // var result2 = cars.SelectMany (c => c.Name);

         // foreach (var character in result2)
         // {
         //    System.Console.WriteLine (character);
         // }

         // Returns most efficient cars joining manufactures to get the headquarter
         var queryJoin1 =
            from car in cars
         join manufacturer in manufactures
         on car.Manufacturer equals manufacturer.Name
         orderby car.Combined descending, car.Name ascending
         select new
         {
            manufacturer.Headquarters,
            car.Name,
            car.Combined
         };

         var queryJoin2 = cars
            .Join (manufactures,
               c => c.Manufacturer,
               m => m.Name,
               (c, m) => new
               {
                  m.Headquarters,
                     c.Name,
                     c.Combined
               })
            .OrderByDescending (c => c.Combined)
            .ThenBy (c => c.Name);

         System.Console.WriteLine ();
         System.Console.WriteLine ("Most Efficient Cars Headquarters");
         System.Console.WriteLine ();

         foreach (var car in queryJoin2.Take (10))
         {
            System.Console.WriteLine ($"{car.Headquarters} : {car.Name} : {car.Combined}");
         }

         // Returns most efficient cars using a Composite Key Join (Manufacturer, Key)
         var queryJoinCompositeKey =
            from car in cars
         join manufacturer in manufactures
         on new { car.Manufacturer, car.Year }
         equals
         new { Manufacturer = manufacturer.Name, manufacturer.Year }
         orderby car.Combined descending, car.Name ascending
         select new
         {
         manufacturer.Headquarters,
         car.Name,
         car.Combined
         };

         var queryJoinCompositeKey2 = cars
            .Join (manufactures,
               c => new { c.Manufacturer, c.Year },
               m => new { Manufacturer = m.Name, m.Year },
               (c, m) => new
               {
                  m.Headquarters,
                     c.Name,
                     c.Combined
               })
            .OrderByDescending (c => c.Combined)
            .ThenBy (c => c.Name);

         System.Console.WriteLine ();
         System.Console.WriteLine ("Most Efficient Cars Headquarters using Composite Key (Manufacturer, Key)");
         System.Console.WriteLine ();

         foreach (var car in queryJoinCompositeKey.Take (10))
         {
            System.Console.WriteLine ($"{car.Headquarters} : {car.Name} : {car.Combined}");
         }

         // Returns most efficient cars grouped by manufacturer
         var queryGrouping =
            from car in cars
         group car by car.Manufacturer;

         System.Console.WriteLine ();
         System.Console.WriteLine ("Cars grouped by Manufacturer");
         System.Console.WriteLine ();

         foreach (var car in queryGrouping.Take (10))
         {
            System.Console.WriteLine ($"{car.Key} has {car.Count()} cars");
         }

         // Returns cars grouped by manufacturer and efficiency
         var queryGroupingEfficiency =
            from car in cars
         group car by car.Manufacturer.ToUpper () into manufacturer
         orderby manufacturer.Key
         select manufacturer;

         var queryGroupingEfficiency2 =
            cars.GroupBy (c => c.Manufacturer.ToUpper ())
            .OrderBy (g => g.Key);

         System.Console.WriteLine ();
         System.Console.WriteLine ("Cars grouped by Manufacturer and Efficiency");
         System.Console.WriteLine ();

         foreach (var group in queryGroupingEfficiency.Take (10))
         {
            System.Console.WriteLine (group.Key);
            foreach (var car in group.OrderByDescending (c => c.Combined).Take (2))
            {
               System.Console.WriteLine ($"\t{car.Name}:{car.Combined}");
            }
         }

         // Returns cars grouped by manufacturer and country using a group join
         var queryJoinGroup =
            from manufacturer in manufactures
         join car in cars on manufacturer.Name equals car.Manufacturer
         into carGroup
         orderby manufacturer.Name
         select new
         {
            Manufacturer = manufacturer,
            Cars = carGroup
         };

         var queryJoinGroup2 =
            manufactures.GroupJoin (cars, m => m.Name, c => c.Manufacturer, (m, g) =>
               new
               {
                  Manufacturer = m,
                     Cars = g
               }).OrderBy (m => m.Manufacturer.Name);

         System.Console.WriteLine ();
         System.Console.WriteLine ("Cars grouped by Manufacturer, Country and Efficiency");
         System.Console.WriteLine ();

         foreach (var group in queryJoinGroup2.Take (10))
         {
            System.Console.WriteLine ($"{group.Manufacturer.Name} : {group.Manufacturer.Headquarters}");
            foreach (var car in group.Cars.OrderByDescending (c => c.Combined).Take (2))
            {
               System.Console.WriteLine ($"\t{car.Name} : {car.Combined}");
            }
         }

         // Returns top 3 most efficient cars grouped by country
         var queryJoinGroupInto =
            from manufacturer in manufactures
         join car in cars on manufacturer.Name equals car.Manufacturer
         into carGroup
         select new
         {
            Manufacturer = manufacturer,
               Cars = carGroup
         }
         into resultInto
         group resultInto by resultInto.Manufacturer.Headquarters;

         var queryJoinGroupInto2 =
            manufactures.GroupJoin (cars, m => m.Name, c => c.Manufacturer,
               (m, g) =>
               new
               {
                  Manufacturer = m,
                     Cars = g
               })
            .GroupBy (m => m.Manufacturer.Headquarters);

         System.Console.WriteLine ();
         System.Console.WriteLine ("Top 3 Most Efficient Cars grouped by Country");
         System.Console.WriteLine ();

         foreach (var group in queryJoinGroupInto2.Take (10))
         {
            System.Console.WriteLine ($"{group.Key}");
            foreach (var car in group.SelectMany (g => g.Cars)
                  .OrderByDescending (c => c.Combined)
                  .Take (3))
            {
               System.Console.WriteLine ($"\t{car.Name} : {car.Combined}");
            }
         }

         // Returns statistics by manufacturers using Aggregates
         var queryAggregate =
            from car in cars
         group car by car.Manufacturer into carGroup
         select new
         {
            Name = carGroup.Key,
               Max = carGroup.Max (c => c.Combined),
               Min = carGroup.Min (c => c.Combined),
               Avg = carGroup.Average (c => c.Combined)
         }
         into resultAggregate
         orderby resultAggregate.Max descending
         select resultAggregate;

         System.Console.WriteLine ();
         System.Console.WriteLine ("Statistics by Manufacturer");
         System.Console.WriteLine ();

         foreach (var manufacturer in queryAggregate)
         {
            System.Console.WriteLine ($"{manufacturer.Name}");
            System.Console.WriteLine ($"\t Max:{manufacturer.Max}");
            System.Console.WriteLine ($"\t Min:{manufacturer.Min}");
            System.Console.WriteLine ($"\t Avg:{manufacturer.Avg}");
         }

      } // Main

      // Returns a list of cars parsed from a file
      private static List<Car> ProcessCars (string path)
      {
         // Query syntax
         var query =
            from line in File.ReadAllLines (path)
            .Skip (1)
         where line.Length > 1
         select Car.ParseFromCsv (line);

         // return query.ToList ();

         // Method syntax
         return File.ReadAllLines (path)
            .Skip (1) // skip header
            .Where (line => line.Length > 1)
            .Select (line => Car.ParseFromCsv (line))
            .ToList ();
      }

      // Returns a list of manufacturers parsed from a file
      private static List<Manufacturer> ProcessManufactures (string path)
      {
         var query = File.ReadAllLines (path)
            .Where (l => l.Length > 1)
            .Select (l =>
            {
               var columns = l.Split (',');
               return new Manufacturer
               {
                  Name = columns[0],
                     Headquarters = columns[1],
                     Year = int.Parse (columns[2])
               };
            });

         return query.ToList ();
      }

   }
}
