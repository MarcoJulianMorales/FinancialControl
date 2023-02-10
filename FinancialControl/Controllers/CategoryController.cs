using FinancialControl.Models;
using FinancialControl.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialControl.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IUsersService usersService;

        public CategoryController(ICategoryRepository categoryRepository, IUsersService usersService)
        {
            this.categoryRepository = categoryRepository;
            this.usersService = usersService;
        }

        public IActionResult Create()
        {
            return View();
        }
        
        public async Task<IActionResult> Index()
        {
            var UserId = usersService.GetUserId();
            var categories = await categoryRepository.Get(UserId);
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if(!ModelState.IsValid)
            {
                return View(category);
            }

            var UserId = usersService.GetUserId();
            category.UserId = UserId;
            await categoryRepository.Create(category);
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Edit(int Id)
        {
            var UserId = usersService.GetUserId();
            var category = await categoryRepository.GetById(Id, UserId);

            if(category == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category categoryEdit)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryEdit);
            }

            var userId = usersService.GetUserId();
            var category = await categoryRepository.GetById(categoryEdit.Id, userId);

            if(category == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            categoryEdit.UserId = userId;
            await categoryRepository.Update(categoryEdit);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            var UserId = usersService.GetUserId();
            var category = await categoryRepository.GetById(Id, UserId);

            if (category == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int Id)
        {
            var userId = usersService.GetUserId();
            var category = await categoryRepository.GetById(Id, userId);

            if (category == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await categoryRepository.Delete(Id);
            return RedirectToAction("Index");
        }
    }
}
