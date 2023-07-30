using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CSV_withMVC.Models
{
    public class User
    {
        [Index(1)]
        public string Username { get; set; }
        [Index(2)]
        [Key]
        public int UserIdentifier { get; set; }
        [Index(3)]
        public int Age { get; set; }
        [Index(4)]
        public string City { get; set; }
        [Index(5)]
        public string PhoneNumber { get; set; }
        [Index(6)]
        public string Email { get; set; }
    }
}
