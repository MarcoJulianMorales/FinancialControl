using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancialControl.Models
{
    public class AccountCreate: Account
    {
        public IEnumerable<SelectListItem> AccountTypes { get; set; }
    }
}
