using System.ComponentModel;

namespace ApexZenith.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsDeleted { get; set; }



    }


}



