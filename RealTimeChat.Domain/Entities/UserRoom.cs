using System;

namespace RealTimeChat.Domain.Entities
{
    public class UserRoom
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;
    }
}
