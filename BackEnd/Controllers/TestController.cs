using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using BackEnd.Authorize;
using Models.People.Roles;
using Microsoft.AspNetCore.Authorization;
using Extensions;

namespace BackEnd.Controllers
{
    [AllowAnonymous]
    [Route("api/mytest")]
    public class TestController : Controller
    {
        private readonly IJwtFactory jwtFactory;

        public TestController(IJwtFactory jwtFactory)
        {
            this.jwtFactory = jwtFactory;
        }
        [HttpGet]
        public string Index()
        => RoleString(RoleNames.CanEditEquipment, RoleNames.CanDeleteEventRole);

        private static byte[] powers = Enumerable
                .Range(0, 8)
                .Select(v => Math.Pow(2, v))
                .Select(v => (byte)v)
                .ToArray();

        private static string RoleString(params RoleNames[] roleNames)
        {
            var rolesCount = Enum.GetNames(typeof(RoleNames)).Length;
            var bufferLength = rolesCount / 8
                + (rolesCount % 8 == 0 ? 0 : 1);
            var buffer = new byte[bufferLength];

            foreach (var role in roleNames)
            {
                var roleNum = (int)role;
                buffer[roleNum / 8] |= powers[roleNum % 8];
            }
            Console.WriteLine($"buffer: {string.Join(',', buffer)}");
            return Convert.ToBase64String(buffer);
        }

        private static IEnumerable<RoleNames> GetNames(string encoded)
        {
            var buffer = Convert.FromBase64String(encoded);
            Console.WriteLine("length: " + buffer.Length);
            Console.WriteLine($"buffer 2: {string.Join(',', buffer)}");
            for (var i = 0; i < buffer.Length; i++)
            for (var j = 0; j < powers.Length; j++)
                if ((buffer[i] & powers[j]) != 0)
                    yield return (RoleNames) (i * 8 + j);
        }
    }
}
