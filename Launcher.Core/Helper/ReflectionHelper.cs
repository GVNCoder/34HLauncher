using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Launcher.Core.Helper
{
    public static class ReflectionHelper
    {
        #region Private helpers

        private static IEnumerable<FieldInfo> _GetFieldsCollection(IReflect type)
            => type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static IEnumerable<PropertyInfo> _GetPropertiesCollection(IReflect type)
            => type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        #endregion

        public static T GetFieldInstance<T>(object instance, string instanceName)
            => _GetFieldsCollection(instance.GetType())
                .Where(m => m.Name == instanceName)
                .Select(p => p.GetValue(instance))
                .OfType<T>()
                .FirstOrDefault();
        
        public static T GetPropertyInstance<T>(object instance, string instanceName)
            => _GetPropertiesCollection(instance.GetType())
                .Where(m => m.Name == instanceName)
                .Select(p => p.GetValue(instance))
                .OfType<T>()
                .FirstOrDefault();
    }
}