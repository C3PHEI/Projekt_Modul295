using System.Text.Json.Serialization;

namespace API_Modul295.Models
{
    public class Service
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        
        [JsonIgnore]
        public ICollection<Order> Orders { get; set; }
    }
}