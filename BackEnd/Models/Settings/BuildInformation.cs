using System;
namespace BackEnd.Models.Settings
{
    /// <summary>
    /// Information about the build
    /// </summary>
    public class BuildInformation
    {
        /// <summary>
        /// Build unique id
        /// </summary>
        public string BuildId { get; set; }
        /// <summary>
        /// Build Data
        /// </summary>
        public string BuildDateString { get; set; }
    }
}
