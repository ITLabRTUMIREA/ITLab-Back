namespace Models.PublicAPI.Requests
{
    public class DeletableRequest : IdRequest
    {
        public bool Delete { get; set; }
    }
}