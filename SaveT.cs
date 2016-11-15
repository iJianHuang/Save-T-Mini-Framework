using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.HD
{
    internal static class SaveT
    {
        private static List<T> GetExtraTs<T>(List<T> firstTs, List<T> secondTs, Func<T, T, bool> isEquals)
        {
            List<T> deltaTs = new List<T>();
            secondTs.ForEach(s =>
            {
                if (!firstTs.Exists(f => isEquals(f, s)))
                {
                    deltaTs.Add(s);
                }
            });
            return deltaTs;
        }

        private static List<Tuple<T, T>> GetUpdatedTs<T>(List<T> firstTs, List<T> secondTs, Func<T, T, bool> isEquals)
        {
            List<Tuple<T, T>> updatedTs = new List<Tuple<T, T>>();
            firstTs.ForEach(f =>
            {
                secondTs.ForEach(s =>
                {
                    if (isEquals(f, s))
                    {
                        updatedTs.Add(new Tuple<T, T>(f, s));
                    }
                });
            });
            return updatedTs;
        }

        private static Tuple<List<T>, List<T>, List<Tuple<T, T>>> GetSetTs<T>(
            List<T> firstTs, List<T> secondTs, Func<T, T, bool> isEquals)
        {
            firstTs = ConvertNullToEmptyListOfT<T>(firstTs);
            secondTs = ConvertNullToEmptyListOfT(secondTs);
            List<T> insertedTs = GetExtraTs(firstTs, secondTs, isEquals);
            List<T> deletedTs = GetExtraTs(secondTs, firstTs, isEquals);
            List<Tuple<T, T>> updatedTs = GetUpdatedTs(firstTs, secondTs, isEquals);
            return new Tuple<List<T>, List<T>, List<Tuple<T, T>>>(insertedTs, deletedTs, updatedTs);
        }

        internal static List<int> SaveTs<T>(List<T> firstTs, List<T> secondTs, Func<T, T, bool> isEquals,
           Func<List<T>, int, List<int>> insertTs, Action<List<T>> deleteTs,
            Action<List<Tuple<T, T>>, int> updateTs, int editedBy)
        {
            List<int> IDs = new List<int>();  
            firstTs = ConvertNullToEmptyListOfT<T>(firstTs);
            secondTs = ConvertNullToEmptyListOfT(secondTs);
            Tuple<List<T>, List<T>, List<Tuple<T, T>>> sets = GetSetTs<T>(firstTs, secondTs, isEquals);
            deleteTs(sets.Item2); // attn: delete first
            IDs = insertTs(sets.Item1, editedBy);
            updateTs(sets.Item3, editedBy);
            return IDs;
        }

        private static List<T> ConvertNullToEmptyListOfT<T>(List<T> listOfTs)
        {
            if (listOfTs == null) listOfTs = new List<T>();
            return listOfTs;
        }

        internal static List<T> WrapIntoListOfT<T>(this T t)
        {
            return t == null ? new List<T>() : new List<T>() { t };
        }
    }
}
