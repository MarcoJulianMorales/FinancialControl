using FinancialControl.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinancialControl.Models
{
    public class AccountType
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="The field Name is required")]
        [FirstCapLyric]
        [Remote(action: "VerifyExistAccountType", controller: "AccountType")]
        public string Name { get; set; }
        public int UserId{ get; set; }
        public int Orden { get; set; }

        /*Testing default validations*/

    }
}
