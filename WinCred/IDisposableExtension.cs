using System;
using System.Collections.Generic;

namespace Advapi32.WinCred
{
    static class IDisposableExtension
    {
        public static T2 Using<T1,T2>(this T1 source, Func<T1,T2> process) where T1 : IDisposable
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (process == null)
                throw new ArgumentNullException(nameof(process));
            using (source)
                return process(source);
        }
        public static void Using<T>(this T source, Action<T> process) where T : IDisposable
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (process == null)
                throw new ArgumentNullException(nameof(process));
            using (source)
                process(source);
        }
        public static IEnumerable<T> Using<T>(this T source) where T : IDisposable
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            using (source)
                yield return source;
        }
        public static IEnumerable<T2> Using<T1, T2>(this T1 source, Func<T1, IEnumerable<T2>> process) where T1 : IDisposable
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (process == null)
                throw new ArgumentNullException(nameof(process));
            using (source)
                foreach (var value in process(source))
                    yield return value;
        }
        public static IEnumerable<T> Using<T>(this IEnumerable<T> source) where T : IDisposable
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            foreach (var s in source)
                using (s)
                    yield return s;
        }
        public static IEnumerable<T2> Using<T1, T2>(this IEnumerable<T1> source,Func<T1, IEnumerable<T2>> process) where T1 : IDisposable
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (process == null)
                throw new ArgumentNullException(nameof(process));
            foreach(var s in source)
                using (s)
                    foreach (var value in process(s))
                        yield return value;
        }
    }
}
