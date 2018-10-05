using System;
namespace Models.People
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime CreateTime { get; set; }
        public string UserAgent { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}