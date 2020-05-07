#if NETSTANDARD1_5
using System;
using System.Reflection;
namespace LitJson
{
    internal static class Netstandard15Polyfill
    {
        internal static Type GetInterface(this Type type, string id)
        {
            return type.GetTypeInfo().GetInterface(id); 
        }

        internal static bool IsClass(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        internal static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }
    }
}
#endif