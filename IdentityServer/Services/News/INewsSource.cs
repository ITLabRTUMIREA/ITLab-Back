using Models.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Services.News
{
    /// <summary>
    /// Source of news
    /// </summary>
    public interface INewsSource
    {
        /// <summary>
        /// Get concret count of last news
        /// </summary>
        /// <param name="count">Count of needed news</param>
        /// <returns>Finded news</returns>
        ValueTask<List<NewsView>> GetLastNews(int count = 5);
    }
}
