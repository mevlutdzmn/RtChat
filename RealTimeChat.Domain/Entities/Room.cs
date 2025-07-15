using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealTimeChat.Domain.Entities
{
    public class Room
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = null!;

        [JsonIgnore]
        public ICollection<UserRoom> UserRooms { get; set; } = new List<UserRoom>(); // ✅

        [JsonIgnore]
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
