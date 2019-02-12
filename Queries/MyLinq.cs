using System;
using System.Collections.Generic;

namespace Queries
{
   public static class MyLinq
   {
      public static IEnumerable<T> Filter<T> (this IEnumerable<T> source, Func<T, bool> predicate)
      {
         // Not needed when using Yield     
         //var result = new List<T> ();

         foreach (var item in source)
         {
            if (predicate (item)) yield return item; // or result.Add (item); when not using Yield
         }

         // Not needed when using Yield
         //return result;
      }

   }
}
