using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethods
{
    class Person
    {
        internal int PersonID;
        internal string car;
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };

            #region FirstOrDefault
            var quary = numbers.First();
            Console.WriteLine("First or Default is {0}", quary);

            #endregion

            #region Where
            var evenNumbers = from number in numbers
                              where number % 2 == 0
                              select number;

            Console.Write("Even Numbers in our List ");

            foreach (var item in evenNumbers)
                Console.Write(" {0}", item);


            #endregion

            List<Person> persons = new List<Person>();

            persons[0] = new Person { PersonID = 1, car = "Ferrari" };
            persons[1] = new Person { PersonID = 1, car = "BMW" };
            persons[2] = new Person { PersonID = 2, car = "Audi" };

            var results = persons.GroupBy(p => p.PersonID);

            foreach (var item in results)
            {
                Console.WriteLine(item);

            }
            Console.ReadKey();
        }
    }

    public static class Extensions
    {

        public static T FirstOrDefault<T>(this IEnumerable<T> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (obj.Any())
            {
                return (obj as IList<T>)[0];
            }
            else
            {
                using (IEnumerator<T> e = obj.GetEnumerator())
                {
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                }
            }

            return default(T);
        }

        public static T First<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (!source.Any())
            {
                throw new NullReferenceException("No elements");
            }
            using (IEnumerator<T> e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    return e.Current;
                }
            }
            throw new OverflowException();
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, int, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            int index = 0;
            foreach (T item in source)
            {
                if (predicate(item, index))
                {
                    yield return item;
                }
                index++;
            }
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (T t in source)
            {
                if (predicate(t))
                {
                    yield return t;
                }
            }

        }

        public static IEnumerable<U> Select<T, U>(this IEnumerable<T> source, Func<T, U> func)
        {
            foreach (T item in source)
            {
                yield return func(item);
            }
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (selector == null)
                throw new ArgumentNullException("selector");

            int index = -1;
            foreach (TSource element in source)
            {
                index++;
                yield return selector(element, index);
            }
        }

        internal class Grouped<TKey, TElement> : List<TElement>,IGrouping<TKey, TElement>
        {
            public TKey Key { get; private set; }

            public Grouped(TKey key, IEnumerable<TElement> elements) :
                base(elements)
            {
                this.Key = key;
            }
        }
        public static IEnumerable<IGrouping<TKey, TElement>>GroupBy<TKey, TElement>(this IEnumerable<TElement> source,Func<TElement, TKey> keySelector)
        {
            using (IEnumerator<TElement> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    yield break;
                }

                TKey previousKey = keySelector(enumerator.Current);
                var batch = new List<TElement>();

                do
                {
                    TKey currentKey = keySelector(enumerator.Current);
                    if (currentKey.Equals(previousKey))
                    {
                        batch.Add(enumerator.Current);
                    }
                    else
                    {
                        yield return new Grouped<TKey, TElement>(previousKey, batch);
                        previousKey = currentKey;
                        batch = new List<TElement>();
                        batch.Add(enumerator.Current);
                    }
                }
                while (enumerator.MoveNext());

                yield return new Grouped<TKey, TElement>(previousKey, batch);
            }
        }

    }
}
