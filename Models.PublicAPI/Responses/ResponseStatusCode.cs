using System;

namespace Models.PublicAPI.Responses
{
    public enum ResponseStatusCode
    {
        Unknown,
        OK,
        FieldExist,
        NotFound,
        IncorrectRequestData,
        NotImplenment,
        EquipmentTypeNotFound,
        EventTypeNotFound
    }
}
