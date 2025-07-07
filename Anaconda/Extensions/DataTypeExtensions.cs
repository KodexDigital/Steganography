using System.Reflection;

namespace Anaconda.Extensions
{
    public static class DataTypeExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> AsEnumerableKeyValuePair(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)!.ToString()
            )!;

        }
    }
}
