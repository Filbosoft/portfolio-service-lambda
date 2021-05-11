using System;

namespace Business
{
    public class BusinessRequest
    {
        public long RequestingUserId { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}