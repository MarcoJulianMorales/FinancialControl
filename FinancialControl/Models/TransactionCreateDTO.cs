using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FinancialControl.Models
{
    public class TransactionCreateDTO: Transaction
    {
        public IEnumerable<SelectListItem> Accounts { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }

    }
}
