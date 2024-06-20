using ChocoLuxAPI.Models;
using Chronos.Specification;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ChocoLuxAPI.Repository
{
    public class ProductRepository : RepositoryBase<Product, AppDbContext>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public override Expression<Func<AppDbContext, DbSet<Product>>> DataSet() => o => o.tblProducts;
        public override Expression<Func<Product, object>> Key() => a => a.ProductId;

    }
}
