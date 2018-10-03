using System;
using System.Reflection;
using System.ComponentModel;
namespace Extensions
{
    public static class EnumExtensions
    {
        public static string Discription(this object obj)
            =>
            obj?.GetType().GetCustomAttribute<DescriptionAttribute>()?.Description
                ?? throw new ArgumentException("object must ");
        
    }
}
