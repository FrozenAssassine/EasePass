using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal class ReflectionHelper
    {
        public static IExtensionInterface[] GetAllExternalInstances(string[] paths)
        {
            List<IExtensionInterface> res = new List<IExtensionInterface>();
            foreach (string file in paths)
            {
                Assembly plugin = Assembly.LoadFrom(file);
                IEnumerable<System.Type> types = GetLoadableTypes(plugin);
                foreach (System.Type t in types)
                {
                    if (t.GetInterfaces().Contains(typeof(IExtensionInterface)))
                    {
                        IExtensionInterface obj = (IExtensionInterface)GetInstanceOf(t);
                        if (obj != null) res.Add(obj);
                    }
                }
            }
            return res.ToArray();
        }

        public static IExtensionInterface[] GetAllExternalInstances(string path)
        {
            List<IExtensionInterface> res = new List<IExtensionInterface>();
            Assembly plugin = Assembly.LoadFrom(path);
            IEnumerable<System.Type> types = GetLoadableTypes(plugin);
            foreach (System.Type t in types)
            {
                if (t.GetInterfaces().Contains(typeof(IExtensionInterface)))
                {
                    IExtensionInterface obj = (IExtensionInterface)GetInstanceOf(t);
                    if (obj != null) res.Add(obj);
                }
            }
            return res.ToArray();
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
