using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodTypeController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        public FoodTypeController(IUnitofWork unitofWork) => _unitofWork = unitofWork;

        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = _unitofWork.FoodType.List() });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitofWork.FoodType.Get(c => c.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }
            _unitofWork.FoodType.Delete(objFromDb);
            _unitofWork.Commit();
            return Json(new { success = true, message = "Delete successful." });
        }
    }
}
