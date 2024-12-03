namespace API_Modul295.Models
{
    public static class PriorityLevels
    {
        public const string Hoch = "Hoch";
        public const string Normal = "Normal";
        public const string Niedrig = "Niedrig";

        public static readonly List<string> All = new List<string> { Hoch, Normal, Niedrig };
    }
}