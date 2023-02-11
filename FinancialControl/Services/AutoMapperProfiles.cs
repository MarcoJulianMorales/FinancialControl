using AutoMapper;
using FinancialControl.Models;

namespace FinancialControl.Services
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() {
            CreateMap<Account, AccountCreate>();
            CreateMap<TransactionUpdateDTO, Transaction>().ReverseMap();
        }
    }
}
