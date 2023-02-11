namespace FinancialControl.Models
{
    public class TransactionUpdateDTO: TransactionCreateDTO
    {
        public int PrevAccountId { get; set; }
        public decimal PrevAmount { get; set;}
    }
}
