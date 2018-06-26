using System;
using System.Reflection;
using System.Linq;

namespace DotnetTestAbstractions.DependencyInjection
{
    public static class StartupConventions
    {
        public const string CONTAINER_FIELD_NAME = "Container";
    }

    public static class StartupConventionLoader
    {
        public static TField GetStaticField<TStartup, TField>(string fieldName) where TField : class
        {
            // TODO: cache field lookup
            var startupClass = typeof(TStartup);

            PropertyInfo property = startupClass.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Static);
            if (property == null)
                throw new InvalidOperationException($"Type '{startupClass}' does not contain a member named '{fieldName}'");

            var value = property.GetValue(null);
            if (value == null)
                throw new InvalidOperationException($"Type '{startupClass}' does not contain a static member named '{fieldName} or the member's value is null");

            var convertedValue = value as TField;
            if (convertedValue == null)
                throw new InvalidOperationException($"Type '{startupClass}' does not contain a static member named '{fieldName} of type '{typeof(TField)}', type is {value.GetType()}");

            return convertedValue;
        }
    }
}