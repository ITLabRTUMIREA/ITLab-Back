using System;
using System.Collections.Generic;
using System.Text;

namespace Exceptions.EquipmentTypeExceptions
{
    public class EquipmentTypeFindException : BusinessException
    {
        public EquipmentTypeFindException(string message) : base(message)
        {
        }

        public EquipmentTypeFindException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
