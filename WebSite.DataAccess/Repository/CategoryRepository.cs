﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebSite.Data;
using WebSite.DataAccess.Repository.IRepository;
using WebSite.Models;

namespace WebSite.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category >, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(Category  Category )
        {
            _context.Categories.Update(Category );
        }
    }
}
