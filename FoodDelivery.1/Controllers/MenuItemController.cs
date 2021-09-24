using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MenuItemController : Controller
	{
		private readonly IUnitofWork _unitofWork;
		private readonly IWebHostEnvironment _hostEnv;

		public MenuItemController(IUnitofWork unitofWork, IWebHostEnvironment hostEnv)
		{
			_unitofWork = unitofWork;
			_hostEnv = hostEnv;
		}

		[HttpGet]
		public IActionResult Get()
		{
			return Json(new { data = _unitofWork.MenuItem.List(null, null, "Category,FoodType") });
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var objFromDb = _unitofWork.MenuItem.Get(c => c.Id == id);
			if (objFromDb == null)
			{
				return Json(new { success = false, message = "Error while deleting." });
			}

			if (objFromDb.Image != null)
            {
				var imgPath = Path.Combine(_hostEnv.WebRootPath,
					objFromDb.Image.TrimStart('\\'));
				if (System.IO.File.Exists(imgPath))
                {
					System.IO.File.Delete(imgPath);
                }
            }
			_unitofWork.MenuItem.Delete(objFromDb);
			_unitofWork.Commit();
			return Json(new { success = true, message = "Delete successful." });
		}
	}
}
