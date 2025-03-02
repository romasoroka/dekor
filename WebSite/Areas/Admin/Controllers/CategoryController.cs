using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebSite.Data;
using WebSite.DataAccess.Repository.IRepository;
using WebSite.Models;
using WebSite.Utility;


namespace WebSite.Areas.Admin.Controllers
{
    [Area("Admin")]
   // [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork db) { _unitOfWork = db; }
        public IActionResult Index()
        {
            var categories = _unitOfWork.Category .GetAll().ToList();
            return View(categories);
        }
        public IActionResult Upsert(int? id)
        {
            Category category = new Category(); // Створюємо новий об'єкт, щоб уникнути null

            if (id != null && id != 0)
            {
                category = _unitOfWork.Category.Get(c => c.Id == id);
                if (category == null)
                {
                    return NotFound();
                }
            }
            return View(category); // Передаємо не null
        }

        [HttpPost]
        public IActionResult Upsert(Category  obj)
        {
            if (obj.Id == 0)
            {
                _unitOfWork.Category.Add(obj);
            }
            else
            {
                _unitOfWork.Category.Update(obj);
            }
            _unitOfWork.Save();
            TempData["success"] = "Successfully created product";
            return RedirectToAction("Index");
        }

      
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var Category  = _unitOfWork.Category .Get(c => c.Id == id);
            if (Category  == null)
            {
                return NotFound();
            }
            return View(Category );
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {

            if (ModelState.IsValid)
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }
                Category ? Category  = _unitOfWork.Category .Get(c => c.Id == id);
                if (Category  == null)
                {
                    return NotFound();
                }

                _unitOfWork.Category .Remove(Category );
                _unitOfWork.Save();
                TempData["success"] = "Successfully removed Category ";
                return RedirectToAction("Index");
            }
            return View();
        }



        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _unitOfWork.Category.GetAll().ToList();
            return Json(new { data = categories });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var category = _unitOfWork.Category.Get(c => c.Id == id);
            if (category == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Category deleted successfully!" });
        }

    }

}

