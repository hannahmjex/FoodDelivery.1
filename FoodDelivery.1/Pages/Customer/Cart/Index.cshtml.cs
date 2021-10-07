using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using FoodDelivery._1.ViewModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodDelivery._1.Pages.Customer.Cart
{
    public class IndexModel : PageModel
    {
        private readonly IUnitofWork _unitofWork;
        public IndexModel(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        public OrderDetailsCartVM OrderDetailsCart { get; set; }

        public void OnGet()
        {
            OrderDetailsCart = new OrderDetailsCartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCart = new List<ShoppingCart>()
            };

            OrderDetailsCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                IEnumerable<ShoppingCart> cart = _unitofWork.ShoppingCart.List(c => c.ApplicationUserId == claim.Value);
                if (cart != null)
                {
                    OrderDetailsCart.ListCart = cart.ToList();
                }

                foreach (var cartList in OrderDetailsCart.ListCart)
                {
                    cartList.MenuItem = _unitofWork.MenuItem.Get(n => n.Id == cartList.MenuItemId);
                    OrderDetailsCart.OrderHeader.OrderTotal += (cartList.MenuItem.Price * cartList.Count);
                }
            }
        }

        public IActionResult OnPostMinus(int CartId)
        {
            var cart = _unitofWork.ShoppingCart.Get(c => c.Id == CartId);
            if (cart.Count == 1)
            {
                _unitofWork.ShoppingCart.Delete(cart);
            }
            else
            {
                cart.Count -= 1;
                _unitofWork.ShoppingCart.Update(cart);
            }
            _unitofWork.Commit();

            var count = _unitofWork.ShoppingCart.List(u => u.ApplicationUserId == cart.ApplicationUserId).Count();
            HttpContext.Session.SetInt32(SD.ShoppingCart, count);
            return RedirectToPage("/Customer/Cart/Index");
        }

        public IActionResult OnPostPlus(int CartId)
        {
            var cart = _unitofWork.ShoppingCart.Get(c => c.Id == CartId);

            cart.Count += 1;
            _unitofWork.ShoppingCart.Update(cart);

            _unitofWork.Commit();

            return RedirectToPage("/Customer/Cart/Index");
        }

        public IActionResult OnPostRemove(int CartId)
        {
            var cart = _unitofWork.ShoppingCart.Get(c => c.Id == CartId);
            _unitofWork.ShoppingCart.Delete(cart);
            _unitofWork.Commit();
            var count = _unitofWork.ShoppingCart.List(u => u.ApplicationUserId == cart.ApplicationUserId).Count();
            HttpContext.Session.SetInt32(SD.ShoppingCart, count);
            return RedirectToPage("/Customer/Cart/Index");
        }
    }
}
