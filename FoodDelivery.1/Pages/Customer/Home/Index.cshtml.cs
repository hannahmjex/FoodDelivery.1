using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodDelivery._1.Pages.Customer.Home
{
	public class IndexModel : PageModel
	{
		private readonly IUnitofWork _unitofWork;
		public IndexModel(IUnitofWork unitofWork) => _unitofWork = unitofWork;
		
		public IEnumerable<MenuItem> MenuItemList { get; set; }
		public IEnumerable<Category> CategoryList { get; set; }

		public void OnGet()
		{
			MenuItemList = _unitofWork.MenuItem.List(null, null, "Category,FoodType");
			CategoryList = _unitofWork.Category.List(null, q => q.DisplayOrder, null);
		}
	}
}
