using Dapper;
using FinancialControl.Models;
using FinancialControl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FinancialControl.Controllers
{
    public class AccountTypeController: Controller
    {
        private readonly IAccountTypeRepository accountTypeRepository;
        private readonly IUsersService usersService;

        public AccountTypeController(IAccountTypeRepository accountTypeRepository,
            IUsersService usersService)
        {
            this.accountTypeRepository = accountTypeRepository;
            this.usersService = usersService;
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var UserId = 1;
            var accountTypes = await accountTypeRepository.Get(UserId);
            return View(accountTypes);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountType accountType)
        {
            if(!ModelState.IsValid)
            {
                return View(accountType);
            }
            accountType.UserId = usersService.GetUserId();

            var AccountTypeExists = await accountTypeRepository.Exists(accountType.Name, accountType.UserId);
            if(AccountTypeExists)
            {
                ModelState.AddModelError(nameof(accountType.Name),
                    $"The name {accountType.Name} already exists");
                return View(accountType);
            }

            await accountTypeRepository.Create(accountType);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int Id)
        {
            var UserId = usersService.GetUserId();
            var accountType = await accountTypeRepository.getById(Id, UserId);

            if(accountType == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(accountType);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(AccountType accountType)
        {
            var UserId = usersService.GetUserId();
            var AccountTypeExists = await accountTypeRepository.getById(accountType.Id, UserId);
            
            if(AccountTypeExists == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            await accountTypeRepository.Update(accountType);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var UserId = usersService.GetUserId();
            var accountType = await accountTypeRepository.getById(Id, UserId);

            if(accountType == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccountType(int Id)
        {
            var UserId = usersService.GetUserId();
            var accountType = await accountTypeRepository.getById(Id, UserId);

            if (accountType == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            await accountTypeRepository.Delete(Id);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> VerifyExistAccountType(string name)
        {
            var UserId = usersService.GetUserId(); 
            var AccountTypeAlreadyExists = await accountTypeRepository.Exists(name, UserId);
            if(AccountTypeAlreadyExists)
            {
                return Json($"The name {name} already exists");
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> Order([FromBody] int[] ids)
        {
            var UserId = usersService.GetUserId();
            var accountTypes = await accountTypeRepository.Get(UserId);
            var accountTypesIds = accountTypes.Select(x => x.Id);

            var accountTypesIdsNotBelongsToUser = ids.Except(accountTypesIds).ToList();

            if(accountTypesIdsNotBelongsToUser.Count > 0)
            {
                return Forbid();
            }

            var accountTypesSorted = ids.Select((value, index) => 
                new AccountType() { Id = value, Orden = index + 1 }).AsEnumerable();
            
            await accountTypeRepository.Order(accountTypesSorted);
            return Ok();
        }
    }
}
