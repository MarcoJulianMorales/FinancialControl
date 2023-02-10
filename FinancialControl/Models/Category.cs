using System.ComponentModel.DataAnnotations;

namespace FinancialControl.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="The field {0} is required")]
        [StringLength(maximumLength:50, ErrorMessage = "Maximum {1} chars")]
        public string Name { get; set; }
        [Display(Name= "Operation Type")]
        public OperationType OperationTypeId { get; set; }
        public int UserId { get; set; }
    }
}
