using System;
namespace Models.PublicAPI.Responses.People.Properties
{
    /// <summary>
    /// view of user property type
    /// </summary>
    public class UserPropertyTypeView
    {
        /// <summary>
        /// Unique identifyer
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Title of user property
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Description of user property
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Count od instances of that type
        /// </summary>
        public int InstancesCount { get; set; }
        /// <summary>
        /// Is hard locket field, can't be deleted
        /// </summary>
        public bool IsLocked { get; set; }
    }
}
