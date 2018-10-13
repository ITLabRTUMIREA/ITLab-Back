
namespace Models.PublicAPI.NotifyRequests
{
    public class NotifyRequest<T>
    {
        public string Type { get; set; }
        public T Data { get; set; }
    }
}
