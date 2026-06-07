using System.ComponentModel;

namespace ApexZenith.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string IsHeadOffice { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string GoogleMapUrl { get; set; } = string.Empty;

      

    }


}



