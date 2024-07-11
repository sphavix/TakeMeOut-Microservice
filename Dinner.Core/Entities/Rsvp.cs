using System.Text.Json.Serialization;

namespace Dinner.Core.Entities
{
    public class Rsvp 
    {
        public Guid Id { get; set; }
        public Guid DinnerId { get; set; }
        public string UserName { get; set; } = string.Empty;

        [JsonIgnore]
        public Lunch Lunch { get; set; }
    }
}