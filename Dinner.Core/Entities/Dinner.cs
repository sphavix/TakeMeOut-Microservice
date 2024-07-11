namespace Dinner.Core.Entities
{
    public class Dinner
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        
        public DateTime EventDate { get; set; }

        public string Description { get; set; }

        public string UserName { get; set; }

        public string ContactPhone { get; set; }
        public string Country { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public virtual ICollection<Rsvp> Rsvps { get; set; }

        public bool IsUserHost(string userName)
        {
            return (string.Equals(UserName, userName, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsUserRegistered(string userName)
        {
            return Rsvps == null ? false : Rsvps.Any(r => string.Equals(r.UserName, userName, StringComparison.OrdinalIgnoreCase));
        }
    }
}