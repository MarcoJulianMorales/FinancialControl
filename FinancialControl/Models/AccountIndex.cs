namespace FinancialControl.Models
{
    public class AccountIndex
    {
        public string AccountType { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        
        public decimal Balance => Accounts.Sum(a => a.Balance);
    }
}
