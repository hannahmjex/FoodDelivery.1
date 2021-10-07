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
using Stripe;

namespace FoodDelivery._1.Pages.Customer.Cart
{
    public class SummaryModel : PageModel
    {
        private readonly IUnitofWork _unitofWork;
        public SummaryModel(IUnitofWork unitofWork) => _unitofWork = unitofWork;

        [BindProperty]
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
                OrderDetailsCart.OrderHeader.OrderTotal += OrderDetailsCart.OrderHeader.OrderTotal * SD.SalesTaxPercent;
                ApplicationUser applicationUser = _unitofWork.ApplicationUser.Get(c => c.Id == claim.Value);
                OrderDetailsCart.OrderHeader.DeliveryName = applicationUser.FullName;
                OrderDetailsCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
                OrderDetailsCart.OrderHeader.DevlieryTime = DateTime.Now;
                OrderDetailsCart.OrderHeader.DeliveryDate = DateTime.Now;

            }
        }

        public IActionResult OnPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsCart.ListCart = _unitofWork.ShoppingCart.List(c => c.ApplicationUserId == claim.Value).ToList();
            OrderDetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            OrderDetailsCart.OrderHeader.OrderDate = DateTime.Now;
            OrderDetailsCart.OrderHeader.UserId = claim.Value;
            OrderDetailsCart.OrderHeader.Status = SD.StatusSubmitted;
            OrderDetailsCart.OrderHeader.DevlieryTime = Convert.ToDateTime(OrderDetailsCart.OrderHeader.DeliveryDate.ToShortDateString() + " " + OrderDetailsCart.OrderHeader.DevlieryTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            
            _unitofWork.OrderHeader.Add(OrderDetailsCart.OrderHeader);
            _unitofWork.Commit();

            foreach (var item in OrderDetailsCart.ListCart)
            {
                item.MenuItem = _unitofWork.MenuItem.Get(m => m.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = OrderDetailsCart.OrderHeader.Id,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    count = item.Count
                };

                OrderDetailsCart.OrderHeader.OrderTotal += (orderDetails.count * orderDetails.Price) * (1 + SD.SalesTaxPercent);
                _unitofWork.OrderDetails.Add(orderDetails);
            }

            OrderDetailsCart.OrderHeader.OrderTotal = Convert.ToDouble(string.Format("{0:.##}", OrderDetailsCart.OrderHeader.OrderTotal));

            HttpContext.Session.SetInt32(SD.ShoppingCart, 0);

            _unitofWork.Commit();
            
            //calling stripe pop up api through JS
            if (stripeToken != null)
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(OrderDetailsCart.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order Id: " + OrderDetailsCart.OrderHeader.Id,
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);
                OrderDetailsCart.OrderHeader.TransactionId = charge.Id;

                if (charge.Status.ToLower() == "succeeded")
                {
                    OrderDetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                }
                else
                {
                    OrderDetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
            }
            _unitofWork.Commit();
            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { id = OrderDetailsCart.OrderHeader.Id });

        }
    }
}
