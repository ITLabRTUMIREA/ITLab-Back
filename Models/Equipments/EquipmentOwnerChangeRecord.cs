using Models.People;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Equipments
{
    /// <summary>
    /// Record about change owner event
    /// </summary>
    public class EquipmentOwnerChangeRecord
    {
        /// <summary>
        ///  Equipment whose owner is changing id
        /// </summary>
        public Guid EquipmentId { get; set; }
        /// <summary>
        /// Equipment whose owner is changing
        /// </summary>
        public Equipment Equipment { get; set; }
        /// <summary>
        /// When the owner changed
        /// </summary>
        public DateTime ChangeOwnerTime { get; set; }
        /// <summary>
        /// New owner id
        /// </summary>
        public Guid? NewOwnerId { get; set; }
        /// <summary>
        /// New owner
        /// </summary>
        public User NewOwner { get; set; }
        /// <summary>
        /// Id of person who grant access to equipment
        /// </summary>
        public Guid GranterId { get; set; }
        /// <summary>
        /// Person who grant access to equipment
        /// </summary>
        public User Granter { get; set; }
    }
}
