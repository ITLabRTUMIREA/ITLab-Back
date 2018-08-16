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
        EventTypeNotFound, 
        InvalidToken,
        UserNotFound,
        WrongPassword,
        IncorrectEquipmentIds,
        Unauthorized,
        DeleteRoleError,
        IncorrectUserIds,
        EquipmentReserved,
        IncorrectAccessToken,
        NoShifts,
        LastShift,
        YouAreInRole
    }
}
