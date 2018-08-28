using System;
namespace Models.PublicAPI.Requests.Account
{
    public class AccountEditRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
