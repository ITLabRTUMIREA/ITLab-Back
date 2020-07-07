using System;
using System.Collections.Generic;
using System.Text;

namespace Exceptions.EquipmentExceptions
{
    public class InvalidChildrenIdsException : EquipmentUpdateException
    {
        public InvalidChildrenIdsException() : base($"one or more children id was not found")
        {
        }
    }
}
