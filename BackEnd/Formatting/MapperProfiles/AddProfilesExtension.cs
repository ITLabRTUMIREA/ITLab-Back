using AutoMapper;
using Requests = BackEnd.Formatting.MapperProfiles.RequestProfiles;
using Responses = BackEnd.Formatting.MapperProfiles.ResponseProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Formatting.MapperProfiles
{
    /// <summary>
    /// Extensions class for adding profiles
    /// </summary>
    public static class AddProfilesExtension
    {
        /// <summary>
        /// Add all profiles from that project
        /// </summary>
        /// <param name="config"></param>
        public static void AddBackendProfiles(this IMapperConfigurationExpression config)
        {
            config.AddProfile<Requests.EquipmentProfile>();
            config.AddProfile<Requests.EventProfile>();
            config.AddProfile<Requests.UserProfile>();

            config.AddProfile<Responses.EquipmentProfile>();
            config.AddProfile<Responses.EventProfile>();
            config.AddProfile<Responses.UserProfile>();

            // TODO separate that file
            config.AddProfile<MapperProfile>();
        }
    }
}
