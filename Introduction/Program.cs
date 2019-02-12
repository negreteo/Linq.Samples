using System;
using System.IO;
using System.Linq;

namespace Introduction
{
    class Program
    {
        static void Main (string[] args)
        {
            string path = Environment.GetFolderPath (Environment.SpecialFolder.Personal) + "/Documents";
            ShowLargeFilesInDirectory (path);
        }

        private static void ShowLargeFilesInDirectory (string path)
        {
            // LINQ sample
            // var query = from file in new DirectoryInfo (path).GetFiles ()
            // orderby file.Length descending
            // select file;

            // foreach (var file in query.Take (5)){}
            // {
            //     System.Console.WriteLine ($"{file.Name,-20} : {file.Length,10:N0}");
            // }

            // LINQ using method calls
            var query = new DirectoryInfo (path).GetFiles ()
                .OrderByDescending (f => f.Length)
                .Take (5);

            foreach (var file in query)
            {
                System.Console.WriteLine ($"{file.Name,-20} : {file.Length,10:N0}");
            }
        }
    }
}
