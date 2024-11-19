using System.ComponentModel.DataAnnotations;

namespace ACID_Banking.API.Models
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }
        [Required]
        [StringLength(100)]
        public string AccountHolder { get; set; }
        [Required]
        public decimal Balance {  get; set; }
    }
}
