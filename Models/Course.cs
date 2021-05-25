namespace FitBot.Models
{
    public class Course
    {
        public string Title { get; set; }
        public string Instructor { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string Date { get; set; }

        public override string ToString()
        {
            return Title + "\n"
            + "Instructor: " + Instructor + "\n"
            + "Date: " + Date + "\n"
            + "Time: " + TimeStart + "-" + TimeEnd + "\n";
        }
    }
    public enum CourseCondition
    {
        Available,
        NotAvailable
    }
}