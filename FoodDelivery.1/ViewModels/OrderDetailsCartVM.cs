﻿using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery._1.ViewModels
{
    public class OrderDetailsCartVM
    {
        public OrderHeader OrderHeader { get; set; }
   
        public List<ShoppingCart> ListCart { get; set; }
    }
}
