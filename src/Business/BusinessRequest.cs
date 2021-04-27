using System;

namespace Business
{
    public class BusinessRequest
    {
        public int RequestingUserId { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}