using System;
namespace Models.PublicAPI.Responses.People
{
    public class RefreshTokenView
    {
        public Guid Id { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
