using System;
using System.Collections.Generic;
using System.Text;

namespace Exceptions.EquipmentExceptions
{
    public class SerialNumberExistsException : EquipmentUpdateException
    {
        public SerialNumberExistsException(string serialNumber, Exception innerException) : 
            base($"serial number {serialNumber} exists", innerException)
        {
        }
    }
}
