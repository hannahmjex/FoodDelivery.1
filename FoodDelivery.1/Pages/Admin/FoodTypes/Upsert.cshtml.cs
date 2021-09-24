using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodDelivery.Pages.Admin.FoodTypes
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitofWork _unitofWork;

        [BindProperty]
        public FoodType FoodTypeObj { get; set; }

        public UpsertModel(IUnitofWork unitofWork) => _unitofWork = unitofWork;

        //takes optional id
        public IActionResult OnGet(int? id)
        {
            FoodTypeObj = new FoodType();

            //in edit mode
            if (id != 0)
            {
                FoodTypeObj = _unitofWork.FoodType.Get(u => u.Id == id);
            }

            //if food doesn't exists
            if (FoodTypeObj == null)
            {
                return NotFound();
            }

            //assume insert mode
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //if new
            if (FoodTypeObj.Id == 0)
            {
                _unitofWork.FoodType.Add(FoodTypeObj);
            }
            //exisiting record
            else
            {
                _unitofWork.FoodType.Update(FoodTypeObj);
            }
            _unitofWork.Commit();

            return RedirectToPage("./Index");
        }
    }
}
