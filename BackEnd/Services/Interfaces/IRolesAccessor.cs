using System;
namespace BackEnd.Services.Interfaces
{
    public interface IRolesAccessor
    {
        Guid ParticipantRoleId { get; }
        Guid OrginizerRoleId { get; }
    }
}
