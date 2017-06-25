﻿using System.Data.Entity.Validation;
using System.Linq;
using refactor_me.Core;
using refactor_me.Core.Repositories;
using refactor_me.Persistence.Repositories;

namespace refactor_me.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProductApplicationContext _context;
        public IProductRepository Products { get; }
        public IProductOptionRepository ProductOptions { get; }

        public UnitOfWork(ProductApplicationContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
            ProductOptions = new ProductOptionRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public int Complete()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
           
        }
    }
}