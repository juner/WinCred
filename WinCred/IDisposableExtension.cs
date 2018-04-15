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
    }
}
