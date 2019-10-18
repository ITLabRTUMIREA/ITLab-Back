using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.People.Roles
{
    public static class RolesHelper
    {
        private static readonly byte[] Powers = Enumerable
            .Range(0, 8)
            .Select(v => Math.Pow(2, v))
            .Select(v => (byte)v)
            .ToArray();

        public static string RoleString(IEnumerable<RoleNames> roleNames)
        {
            var rolesCount = Enum.GetNames(typeof(RoleNames)).Length;
            var bufferLength = rolesCount / 8
                               + (rolesCount % 8 == 0 ? 0 : 1);
            var buffer = new byte[bufferLength];

            foreach (var role in roleNames)
            {
                var roleNum = (int)role;
                buffer[roleNum / 8] |= Powers[roleNum % 8];
            }
            return Convert.ToBase64String(buffer);
        }
    }
}
