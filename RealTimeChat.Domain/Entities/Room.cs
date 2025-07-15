using System.ComponentModel.DataAnnotations;

namespace RealTimeChat.Domain.Entities
{
    public class Room
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = null!;

        public ICollection<UserRoom> UserRooms { get; set; } = new List<UserRoom>(); // ✅
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
