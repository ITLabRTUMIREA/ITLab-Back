using System;
namespace _build.shared
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
        public DateTimeOffset BuildDateString { get; set; }
    }
}
