using System;
using System.Reflection;

namespace Argilla.Server
{
    public class InstanceHelper
    {
        public static T Create<T>(string typeName)
        {
            Type type = Type.GetType(typeName);

            if (type == null)
            {
                return default;
            }

            try
            {
                return (T)Activator.CreateInstance(type);
            }
            catch
            {
                return default;
            }
        }
    }
}
