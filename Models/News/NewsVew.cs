using System;
using System.Collections.Generic;
using System.Text;

namespace Models.News
{
    /// <summary>
    /// News model for landing page
    /// </summary>
    public class NewsView
    {
        /// <summary>
        /// Unique identofier of news
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Title of news
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// News description in markdown (.md)
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Title image uri
        /// </summary>
        public string ImageUri { get; set; }
        /// <summary>
        /// Publishing date
        /// </summary>
        public DateTime Date { get; set; }
    }
}
