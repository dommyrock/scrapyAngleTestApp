using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SiteSpecificScrapers.Helpers
{
    public static class ReflectionHelper
    {
        public static class ReflectiveEnumerator
        {
            static ReflectiveEnumerator()
            {
            }

            /// <summary>
            /// Returns IEnumerable of all classes that implement base class "BaseScraperClass"
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="constructorArgs"></param>
            /// <returns></returns>
            public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class, IComparable<T>
            {
                List<T> objects = new List<T>();
                foreach (Type type in
                    Assembly.GetAssembly(typeof(T)).GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
                {
                    objects.Add((T)Activator.CreateInstance(type, constructorArgs));
                }
                objects.Sort();
                return objects;
            }
        }

        //        A few notes:

        //Don't worry about the "cost" of this operation - you're only going to be doing it once(hopefully) and even then it's not as slow as you'd think.
        //You need to use Assembly.GetAssembly(typeof(T)) because your base class might be in a different assembly.
        //You need to use the criteria type.IsClass and !type.IsAbstract because it'll throw an exception if you try to instantiate an interface or abstract class.
        //I like forcing the enumerated classes to implement IComparable so that they can be sorted.
        //Your child classes must have identical constructor signatures, otherwise it'll throw an exception. This typically isn't a problem for me.
    }
}