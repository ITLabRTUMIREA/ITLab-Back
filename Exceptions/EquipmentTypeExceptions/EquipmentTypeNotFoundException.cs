using System;
using System.Collections.Generic;
using System.Text;

namespace Exceptions.EquipmentTypeExceptions
{
    public class EquipmentTypeNotFoundException : EquipmentTypeFindException
    {
        public EquipmentTypeNotFoundException(Guid equipmentTypeId, Exception innerException) : base($"equipment type {equipmentTypeId} not found", innerException)
        {
        }
    }
}
