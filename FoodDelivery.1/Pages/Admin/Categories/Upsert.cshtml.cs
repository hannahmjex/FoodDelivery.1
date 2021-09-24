using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodDelivery.Pages.Admin.Categories
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitofWork _unitofWork;
        
        [BindProperty]
        public Category CategoryObj { get; set; }

        public UpsertModel(IUnitofWork unitofWork) => _unitofWork = unitofWork;
                  
        //takes optional id
        public IActionResult OnGet(int ? id)
        {
            CategoryObj = new Category();

            //in edit mode
            if (id != 0)
            {
                CategoryObj = _unitofWork.Category.Get(u => u.Id == id);
            }

            //if category doesn't exists
            if(CategoryObj == null)
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
            if (CategoryObj.Id == 0)
            {
                _unitofWork.Category.Add(CategoryObj);
            }
            //exisiting record
            else
            {
                _unitofWork.Category.Update(CategoryObj);
            }
            _unitofWork.Commit();

            return RedirectToPage("./Index");
        }
    }
}
