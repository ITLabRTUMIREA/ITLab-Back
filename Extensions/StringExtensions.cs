using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Extensions
{
    public static class StringExtensions
    {
        public static object ParseToJson(this string jsonString)
        {
            return JToken.Parse(jsonString);
        }
    }
}
