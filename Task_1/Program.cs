using System;
using System.Collections;
using System.Collections.Generic;
using static System.Linq.Enumerable;
namespace Task_1
{
    public static class GenerationInCollection
        {
            public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> collection, IEqualityComparer<T> comparer, int count) 
            {
                if (collection == null)
                    throw new NullReferenceException();
                if (collection.Count() < count)
                    throw new ArgumentException("Incorrect condition for the combination!");
                if (collection.Distinct(comparer).Count() != collection.Count())
                    throw new ArgumentException("There is non-uniq elements!");
                if (count == 1) return collection.Select(t => new T[] { t });
                return Combinations(collection, comparer, count - 1)
                    .SelectMany(t => collection, (t1, t2) => t1.Concat(new T[] { t2 }));
            }
            
            public static IEnumerable<IEnumerable<T>> Subsets<T>(this IEnumerable<T> collection, IEqualityComparer<T> comparer)
            {
                if (collection == null)
                    throw new NullReferenceException("Collection is empty!");
                if (collection.Distinct(comparer).Count() != collection.Count())
                    throw new ArgumentException("There is non-uniq elements!");
                List<T> list = collection.ToList();
                int length = list.Count;
                int max = (int)Math.Pow(2, list.Count);

                for (int count = 0; count < max; count++)
                {
                    List<T> subset = new List<T>();
                    uint forIndex = 0;
                    while (forIndex < length)
                    {
                        if ((count & (1u << (int)forIndex)) > 0)
                        {
                            subset.Add(list[(int)forIndex]);
                        }
                        forIndex++;
                    }
                    yield return subset;
                }
            }
            
            public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> collection, IEqualityComparer<T> comparer, int lengthOfCollection)
            {
                if (collection == null)
                    throw new NullReferenceException("Collection is empty!");
                if (collection.Distinct(comparer).Count() != collection.Count())
                    throw new ArgumentException("There is non-uniq elements!");
                if (lengthOfCollection == 1) return collection.Select(t => new T[] { t });
                return Permutations(collection, comparer, lengthOfCollection - 1)
                    .SelectMany(t => collection.Where(o => !t.Contains(o)),
                        (t1, t2) => t1.Concat(new T[] { t2 }));
            }
            
        }
        
        
    class Program
    {
        public static void PrintIEnumerable<T>(IEnumerable<IEnumerable<T>> collection)
                {
                    Console.Write("[");
                    foreach (var item in collection)
                    {
                        Console.Write("[");
                        foreach (var item1 in item)
                        {
                            Console.Write(" ");
                            Console.Write(item1);
                            Console.Write(" ");
                        }
                        Console.Write("]");
                    }
                    Console.Write("]");
                    Console.WriteLine();
                }

        static void Main(string[] args)
        {
            List<int> list = new List<int>() {1, 2, 3};
            try
            {
                Console.WriteLine("COMBINATIONS: ");
                PrintIEnumerable(GenerationInCollection.Combinations(new List<int>() {1, 2, 3}, EqualityComparer<int>.Default, 2));
                Console.WriteLine("PERMUTATIONS: ");
                PrintIEnumerable(GenerationInCollection.Permutations(list, EqualityComparer<int>.Default, list.Count));
                Console.WriteLine("SUBSETS: ");
                PrintIEnumerable(GenerationInCollection.Subsets(new List<char>() {'a', 'b', 'c'},
                    EqualityComparer<char>.Default));
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Collection is empty!");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}