namespace API_Modul295.Data
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public int FailedLoginAttempts { get; set; }
        public bool IsLocked { get; set; }
        public bool IsAdmin { get; set; }
    }
}