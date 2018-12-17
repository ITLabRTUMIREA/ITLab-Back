using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Summary
{
    public class GetSummaryRequest
    {
        public List<Guid> TargetEventTypes { get; set; }
    }
}
