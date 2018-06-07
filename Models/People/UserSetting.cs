using System;
namespace Models.People
{
    public class UserSetting
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
    }
}
