using AutoMapper;
using FinancialControl.Models;
using FinancialControl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinancialControl.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IAccountRepository accountRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ITransactionsRepository transactionsRepository;
        private readonly IMapper mapper;

        public TransactionController(
            IUsersService usersService, 
            IAccountRepository accountRepository,
            ICategoryRepository categoryRepository,
            ITransactionsRepository transactionsRepository,
            IMapper mapper) {
            this.usersService = usersService;
            this.accountRepository = accountRepository;
            this.categoryRepository = categoryRepository;
            this.transactionsRepository = transactionsRepository;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var UserId = usersService.GetUserId();
            var model = new TransactionCreateDTO();
            model.Accounts = await GetAccounts(UserId);
            model.Categories = await GetCategories(UserId, model.OperationTypeId);
            return View (model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionCreateDTO model)
        {
            var UserId = usersService.GetUserId();

            if (!ModelState.IsValid)
            {
                model.Accounts = await GetAccounts(UserId);
                model.Categories = await GetCategories(UserId, model.OperationTypeId);
                return View(model);
            }

            var account = await accountRepository.GetById(model.AccountId, UserId);
            
            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var category = await accountRepository.GetById(model.CategoryId, UserId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            model.UserId = UserId;

            if(model.OperationTypeId == OperationType.Bill)
            {
                model.Amount *= -1;
            }

            await transactionsRepository.Create(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var UserId = usersService.GetUserId();
            var transaction = await transactionsRepository.GetById(Id, UserId);

            if(transaction is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var model = mapper.Map<TransactionUpdateDTO>(transaction);

            model.PrevAmount = model.Amount;

            if(model.OperationTypeId == OperationType.Bill)
            {
                model.PrevAmount = model.Amount * -1;
            }

            model.PrevAccountId = transaction.AccountId;
            model.Categories = await GetCategories(UserId, transaction.OperationTypeId);
            model.Accounts = await GetAccounts(UserId);
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Edit(TransactionUpdateDTO model)
        {
            var UserId = usersService.GetUserId();

            if (!ModelState.IsValid)
            {
                model.Accounts = await GetAccounts(UserId);
                model.Categories = await GetCategories(UserId, model.OperationTypeId);
                return View(model);
            }

            var account = await accountRepository.GetById(model.AccountId, UserId);

            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var category = await categoryRepository.GetById(model.CategoryId, UserId);

            if(category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var transaction = mapper.Map<Transaction>(model);

            if(model.OperationTypeId == OperationType.Bill)
            {
                transaction.Amount *= -1;
            }

            await transactionsRepository.Update(transaction, model.PrevAmount, model.PrevAccountId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            var UserId = usersService.GetUserId();

            var transaction = await transactionsRepository.GetById(Id, UserId);

            if(transaction is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await transactionsRepository.Delete(Id);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> GetAccounts(int UserId)
        {
            var accounts = await accountRepository.Search(UserId);
            return accounts.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> GetCategories(int UserId, OperationType operationType)
        {
            var categories = await categoryRepository.Get(UserId, operationType);
            return categories.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> GetCategories([FromBody] OperationType operationType)
        {
            var UserId = usersService.GetUserId();
            var categories = await GetCategories(UserId, operationType);
            return Ok(categories);
        }
        
    }
}
