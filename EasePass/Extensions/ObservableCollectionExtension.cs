/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasePass.Extensions
{
    internal static class ObservableCollectionExtension
    {


        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            int length = sortableList.Count;
            for (int i = 0; i < length; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }

        public static ObservableCollection<T> ReverseSelf<T>(this ObservableCollection<T> collection)
        {
            return new ObservableCollection<T>(collection.Reverse());
        }
    }
}
