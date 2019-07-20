using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.News;

namespace IdentityServer.Services.News
{
    public class DebugNewsSource : INewsSource
    {
        private static readonly List<NewsView> news = new List<NewsView>
        {
            new NewsView
            {
                Id = Guid.Parse("8cc26187-fcac-46b5-a7e7-33ea14d7b885"),
                Title = "Junction TOKYO",
                ImageUri = "https://live.staticflickr.com/787/41003506121_9141ece9ae_o.jpg",
                Date = new DateTime(2018, 3, 5),
                Description = @"
# Titile

## lilte title

> Paragraph

* list
* items
                               "
            },
            new NewsView
            {
                Id = Guid.Parse("8cc26187-fcac-46b5-a7e7-33ea14d7b884"),
                Title = "Fun Hack",
                ImageUri = "https://it-events.com/system/ckeditor/pictures/16776/content_zagruzhennoe_1.jpg",
                Date = new DateTime(2018, 3, 5),
                Description = @"
# Titile

## lilte title
                                
some text, many text

> Paragraph
                               "
            }
        };
        static DebugNewsSource()
        {
            news.Sort((a, b) => a.Date.CompareTo(b.Date));
        }
        public ValueTask<List<NewsView>> GetLastNews(int count = 5)
        {
            return new ValueTask<List<NewsView>>(news.Take(count).ToList());
        }
    }
}
