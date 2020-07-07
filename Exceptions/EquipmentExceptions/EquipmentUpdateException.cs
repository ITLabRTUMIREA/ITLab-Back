using System;
using System.Collections.Generic;
using System.Text;

namespace Exceptions.EquipmentExceptions
{
    public class EquipmentUpdateException : BusinessException
    {
        public EquipmentUpdateException(string message) : base(message)
        {
        }

        public EquipmentUpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
