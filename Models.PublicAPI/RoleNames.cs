using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Models.PublicAPI
{
    public class RoleNames
    {
        private static List<string> list;
        public static List<string> List = list ??
            (list = typeof(RoleNames)
                .GetFields(BindingFlags.Public | BindingFlags.Static |
                            BindingFlags.FlattenHierarchy)
               .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
               .Select(f => f.GetRawConstantValue().ToString())
               .ToList());
        public const string ParticipantRoleName = "Participant";
        public const string OrginizerRoleName = "Organizer";
    }
}
