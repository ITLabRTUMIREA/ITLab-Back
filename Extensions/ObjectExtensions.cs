using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions
{
    public static class ObjectExtensions
    {
        public static T To<T>(this T o, out T x) => x = o;
        public static T With<T>(this T value, params object[] array) => value;
        public static T With<T, A>(this T value, A a) => value;
        public static T With<T, A, B>(this T value, A a, B b) => value;
        public static T With<T, A, B, C>(this T value, A a, B b, C c) => value;
        public static T With<T, A, B, C, D>(this T value, A a, B b, C c, D d) => value;
        public static T With<T, A, B, C, D, E>(this T value, A a, B b, C c, D d, E e) => value;
    }
}
