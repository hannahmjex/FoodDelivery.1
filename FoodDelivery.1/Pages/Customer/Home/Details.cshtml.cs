using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodDelivery._1.Pages.Customer.Home
{
	public class DetailsModel : PageModel
	{
		private readonly IUnitofWork _unitofWork;
		public DetailsModel(IUnitofWork unitofWork) => _unitofWork = unitofWork;

		[BindProperty]
		public ShoppingCart ShoppingCartObj { get; set; }

		public async Task OnGet(int id)
		{
			ShoppingCartObj = new ShoppingCart()
			{
				MenuItem = await _unitofWork.MenuItem.GetAsync(m => m.Id == id, false, "Category,FoodType")
			};
			ShoppingCartObj.MenuItemId = id;
		}

		public IActionResult OnPost()
		{
			if (ModelState.IsValid)
			{
				//get the applicationuser id from aspnetusers table
				var claimsIdentity = (ClaimsIdentity)this.User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
				ShoppingCartObj.ApplicationUserId = claim.Value;

				//if there is already a cart for this user, retrieve it
				ShoppingCart cartFromDb = _unitofWork.ShoppingCart.Get(c => c.ApplicationUserId == ShoppingCartObj.ApplicationUserId && c.MenuItemId == ShoppingCartObj.MenuItemId);

				if(cartFromDb == null)
				{
					_unitofWork.ShoppingCart.Add(ShoppingCartObj);
				}
				else
				{
					cartFromDb.Count += ShoppingCartObj.Count;
					_unitofWork.ShoppingCart.Update(cartFromDb);
				}
				_unitofWork.Commit();

				//this is for our Icon on the shared layout menu (3)
				var count = _unitofWork.ShoppingCart.List(c => c.ApplicationUserId == ShoppingCartObj.ApplicationUserId).Count();
				HttpContext.Session.SetInt32(SD.ShoppingCart, count);
				return RedirectToPage("Index");
			}
			else
			{
				ShoppingCartObj.MenuItem = _unitofWork.MenuItem.Get(m => m.Id == ShoppingCartObj.MenuItemId, false, "Category,FoodType");
			}
			return Page();
		}
	}
}


