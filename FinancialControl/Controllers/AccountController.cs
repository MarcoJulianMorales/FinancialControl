using AutoMapper;
using FinancialControl.Models;
using FinancialControl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace FinancialControl.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountTypeRepository accountTypeRepository;
        private readonly IUsersService usersService;
        private readonly IAccountRepository accountRepository;
        private readonly IMapper mapper;

        public AccountController(
            IAccountTypeRepository accountTypeRepository,
            IUsersService usersService,
            IAccountRepository accountRepository,
            IMapper mapper)
        {
            this.accountTypeRepository = accountTypeRepository;
            this.usersService = usersService;
            this.accountRepository = accountRepository;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var UserId = usersService.GetUserId();
            var AccountsWithAccountType = await accountRepository.Search(UserId);

            var model = AccountsWithAccountType
                .GroupBy(x => x.AccountType)
                .Select(group => new AccountIndex
                {
                    AccountType = group.Key,
                    Accounts = group.AsEnumerable()
                }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var UserId = usersService.GetUserId();
            var model = new AccountCreate();
            model.AccountTypes = await GetAccountTypes(UserId);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(AccountCreate account)
        {
            var UserId = usersService.GetUserId();
            var accountType = await accountTypeRepository.getById(account.AccountTypeId, UserId);

            if(accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (!ModelState.IsValid)
            {
                account.AccountTypes = await GetAccountTypes(UserId);
                return View(account);
            }
            await accountRepository.Create(account);
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Edit(int Id)
        {
            var UserId = usersService.GetUserId();
            var account = await accountRepository.GetById(Id, UserId);
            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var model = mapper.Map<AccountCreate>(account);

            model.AccountTypes = await GetAccountTypes(UserId);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(AccountCreate accountEdit)
        {
            var UserId = usersService.GetUserId();
            var account = await accountRepository.GetById(accountEdit.Id, UserId);

            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var accountType = await accountTypeRepository.getById(accountEdit.AccountTypeId, UserId);

            if(accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await accountRepository.Update(accountEdit);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            var UserId = usersService.GetUserId();
            var account = await accountRepository.GetById(Id, UserId);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int Id)
        {
            var UserId = usersService.GetUserId();
            var account = await accountRepository.GetById(Id, UserId);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            await accountRepository.Delete(Id);
            return RedirectToAction("Index");
        }
        private async Task<IEnumerable<SelectListItem>> GetAccountTypes(int UserId)
        {
            var accountTypes = await accountTypeRepository.Get(UserId);
            return accountTypes.Select(x => new SelectListItem(x.Name, x.Id.ToString()));

        }
        
    }
}
