using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSite.DataAccess.Repository.IRepository;
using WebSite.Models;

namespace WebSite.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            ViewBag.UserName = user.Name;

            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProp: "Category");
            return View(productList);
        }

        [HttpPost]
        public IActionResult Search(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return RedirectToAction("Index", _unitOfWork.Product.GetAll(includeProp: "Category")); // Якщо рядок пошуку порожній, переадресовуємо на головну сторінку
            }
            searchQuery = searchQuery.ToUpper();
            // Шукаємо продукти, що містять введений запит
            var products = _unitOfWork.Product
                .GetAll().ToList();
            products = products.Where(p => p.Name.ToUpper().Contains(searchQuery)) // Використовуємо Contains для пошуку схожих імен
            .ToList();
            if (products.Count == 1)
            {
                // Якщо знайдений лише один продукт, одразу перенаправляємо на його деталі
                return RedirectToAction("Details", new { productId = products.First().Id });
            }

            return View("Index", _unitOfWork.Product.GetAll(includeProp: "Category"));
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProp: "Category"),
                ProductId = productId,
                Count = 1
            };
            return View(shoppingCart);
        }



        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cart = _unitOfWork.ShoppingCart.Get(u=>u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);

            if (cart == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }

            else
            {
                cart.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cart);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Gallery()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
