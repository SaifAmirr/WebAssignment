namespace WebAssignment.Models
{
    public class Course
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int CreditHours { get; set; }
    }
}