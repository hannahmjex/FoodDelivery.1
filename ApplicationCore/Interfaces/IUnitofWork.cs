﻿using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IUnitofWork
    {
        public IGenericRepository<Category> Category { get; }
        public IGenericRepository<FoodType> FoodType { get; }
        public IGenericRepository<MenuItem> MenuItem { get; }
        public IGenericRepository<ApplicationUser> ApplicationUser { get; }
        public IGenericRepository<ShoppingCart> ShoppingCart { get; }
        public IGenericRepository<OrderHeader> OrderHeader { get; }
        public IGenericRepository<OrderDetails> OrderDetails { get; }

        int Commit();

        Task<int> CommitAsync();
    }
}
