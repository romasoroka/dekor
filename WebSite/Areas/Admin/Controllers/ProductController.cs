using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebSite.Data;
using WebSite.DataAccess.Repository.IRepository;
using WebSite.Models;
using WebSite.Models.ViewModels;
using WebSite.Utility;


namespace WebSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment) { _unitOfWork = db; _webHostEnvironment = webHostEnvironment; }
        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll(includeProp: "Category").ToList();
            return View(products);
        }
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u =>
                new SelectListItem { Text = u.Name, Value = u.Id.ToString() }),
                Product = new Product()
            };
            if (id != null && id != 0) 
            {
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
            }
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string path = Path.Combine(wwwRootPath, @"images\product");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            
            if (!string.IsNullOrEmpty(obj.Product.ImageURL) && obj.Product.ImageURL.StartsWith("data:image"))
            {
                try
                {
                    var base64Data = obj.Product.ImageURL.Substring(obj.Product.ImageURL.IndexOf(',') + 1);
                    byte[] imageBytes = Convert.FromBase64String(base64Data);

                    string fileName = Guid.NewGuid().ToString() + ".png"; 
                    string filePath = Path.Combine(path, fileName);
                    System.IO.File.WriteAllBytes(filePath, imageBytes);

                    obj.Product.ImageURL = @"\images\product\" + fileName;
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"Помилка при обробці Base64 зображення: {ex.Message}";
                    return View();
                }
            }

                if (ModelState.IsValid)
            {
                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    var productToChange = _unitOfWork.Product.Get(u => u.Id == obj.Product.Id);
                    if (productToChange.ImageURL != null)
                    {
                        var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, productToChange.ImageURL.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    _unitOfWork.Product.Update(obj.Product);
                }
                _unitOfWork.Save();
                TempData["success"] = "Successfully created product";
                return RedirectToAction("Index");
            }

            return View();
        }



        #region api calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.Product.GetAll(includeProp: "Category").ToList();
            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToDelete = _unitOfWork.Product.Get(u  => u.Id == id);
            if (productToDelete == null)
            {
                return Json(new {success = false, message = "Deleting error!"});
            }
            if (productToDelete.ImageURL != null)
            {
                var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, productToDelete.ImageURL.TrimStart('\\'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            _unitOfWork.Product.Remove(productToDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted successfully!" });
        }

        #endregion
    }
}
