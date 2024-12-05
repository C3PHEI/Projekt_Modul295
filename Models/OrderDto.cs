namespace API_Modul295.Models
{
    public class OrderDto
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }    // Kundenname
        public string Email { get; set; }           // E-Mail
        public string Phone { get; set; }           // Telefon
        public string Priority { get; set; }        // Priorität
        public int ServiceID { get; set; }          // Dienstleistungs-ID
        public string ServiceName { get; set; }     // Name der Dienstleistung
        public string Status { get; set; }          // Status
        public DateTime DateCreated { get; set; }   // Erstellungsdatum
        public DateTime? DateModified { get; set; } // Änderungsdatum
        public DateTime? DateModifie { get; set; }
    }
}