﻿namespace API_Modul295.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }  
        public int FailedLoginAttempts { get; set; }
        public bool IsLocked { get; set; }
        public bool IsAdmin { get; set; }  
        public string Role { get; set; }  
    }
}