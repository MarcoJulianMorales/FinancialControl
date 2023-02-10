using FinancialControl.Validations;
using System.ComponentModel.DataAnnotations;

namespace FinancialControl.Models
{
    public class Account
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="The field {0} is required")]
        [StringLength(maximumLength:50)]
        [FirstCapLyric]
        public string Name { get; set; }
        [Display(Name = "AccountType")]
        public int AccountTypeId{ get; set; }
        public decimal Balance { get; set; }
        [StringLength(maximumLength: 1000)]
        public string Description { get; set; }
        public string AccountType { get; set; }
    }
}
