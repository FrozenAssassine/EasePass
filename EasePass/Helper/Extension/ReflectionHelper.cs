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

using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasePass.Helper.Extension
{
    internal class ReflectionHelper
    {
        public static IExtensionInterface[] GetAllExternalInstances(string path)
        {
            try
            {
                List<IExtensionInterface> res = new List<IExtensionInterface>();
                Assembly plugin = Assembly.LoadFrom(path);
                IEnumerable<Type> types = GetLoadableTypes(plugin);
                foreach (Type t in types)
                {
                    if (t.GetInterfaces().Contains(typeof(IExtensionInterface)))
                    {
                        IExtensionInterface obj = (IExtensionInterface)GetInstanceOf(t);
                        if(obj is IInitializer initializer)
                        {
                            try
                            {
                                initializer.Init();
                            }
                            catch { }
                        }
                        if (obj != null) res.Add(obj);
                    }
                }
                return res.ToArray();
            }
            catch
            {
                return [];
            }
        }

        private static object GetInstanceOf(Type type)
        {
            try
            {
                ConstructorInfo[] cis = type.GetConstructors();
                foreach (ConstructorInfo ci in cis)
                {
                    if (ci.GetParameters().Length == 0)
                    {
                        return type.Assembly.CreateInstance(type.FullName);
                    }
                }
            }
            catch { }
            return null;
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                List<Type> types = new List<Type>();
                foreach (TypeInfo ti in assembly.DefinedTypes)
                {
                    types.Add(ti.AsType());
                }
                return types;
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}
