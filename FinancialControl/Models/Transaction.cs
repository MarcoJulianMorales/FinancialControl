using System.ComponentModel.DataAnnotations;

namespace FinancialControl.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Transaction Date")]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }

        [StringLength(maximumLength:1000, ErrorMessage = "Note maximum length is {1} chars")]
        public string Note { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Must select an Account")]
        [Display(Name="Account")]
        public int AccountId { get; set; }
        [Range(1, maximum:int.MaxValue, ErrorMessage = "Must select a Category")]
        [Display(Name = "Category")]
        public int CategoryId{ get; set; }
        [Display(Name = "Operation Type")]
        public OperationType OperationTypeId { get; set; } = OperationType.Income;
    }
}
