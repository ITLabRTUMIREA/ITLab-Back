namespace Models.PublicAPI.Requests.Events.Event.Edit
{
    public class PlaceEditRequest : DeletableRequest
    {
        public int TargetParticipantsCount { get; set; }
    }
}