using System;
using System.Text.Json.Serialization;

namespace RealTimeChat.Domain.Entities
{
    public class UserRoom
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [JsonIgnore]//Entity’lerin içinde birbirini döngüsel olarak referanslaması çözüm olarak  attribute kullanıldı.
        public Guid RoomId { get; set; }

        [JsonIgnore]
        public Room Room { get; set; } = null!;
    }
}
