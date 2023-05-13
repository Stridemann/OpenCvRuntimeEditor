namespace OpenCvRuntimeEditor.Utils
{
    using System;
    using System.Collections.ObjectModel;

    public static class LinqExtensions
    {
        public static void Foreach<T>(this ObservableCollection<T> collection, Action<T> action)
        {
            foreach (var element in collection)
            {
                action(element);
            }
        }

        public static int IndexOf<T>(this ObservableCollection<T> collection, Func<T, bool> action)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                var element = collection[i];

                if (action(element))
                    return i;
            }

            return -1;
        }

        public static int RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> action)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                var element = collection[i];

                if (action(element))
                {
                    collection.RemoveAt(i);
                    i--;
                }
            }

            return -1;
        }
    }
}
