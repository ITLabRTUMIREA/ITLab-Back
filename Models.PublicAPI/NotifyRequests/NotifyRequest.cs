
namespace Models.PublicAPI.NotifyRequests
{
    public class NotifyRequest<T>
    {
        public NotifyType Type { get; set; }
        public T Data { get; set; }
        public string Secret { get; set; }
    }
}
