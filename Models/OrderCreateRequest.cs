// Models/OrderCreateRequest.cs
namespace API_Modul295.Models
{
    public class OrderCreateRequest
    {
        public string CustomerName { get; set; }  // Kundenname
        public string Email { get; set; }         // E-Mail
        public string Phone { get; set; }         // Telefon
        public string Priority { get; set; }      // Priorität
        public int ServiceID { get; set; }        // Dienstleistungs-ID
    }
}