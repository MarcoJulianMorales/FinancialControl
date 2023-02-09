using System.ComponentModel.DataAnnotations;

namespace FinancialControl.Validations
{
    public class FirstCapLyricAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null || string.IsNullOrEmpty(value.ToString())) {
                return ValidationResult.Success;
            }
            
            var firstChar = value.ToString()[0].ToString();
            
            if(firstChar != firstChar.ToUpper() ) {
                return new ValidationResult("The first Lyric must be capitalized");
            }

            return ValidationResult.Success;
        }
    }
}
