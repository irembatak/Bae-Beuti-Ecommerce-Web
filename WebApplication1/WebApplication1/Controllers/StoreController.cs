using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 8;
        public StoreController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index(int pageIndex, string? search, string? brand, string? category, string? sort)
        {
            IQueryable<Product> query = context.Products;

            query = query.OrderByDescending(p => p.ProductId);

            // search functionality
            if (search != null && search.Length > 0)
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            // filter functionality
            if (brand != null && brand.Length > 0)
            {
                query = query.Where(p => p.ProductBrand.Contains(brand));
            }

            if (category != null && category.Length > 0)
            {
                query = query.Where(p => p.ProductCategory.Contains(category));
            }

            // sort functionality
            if (sort == "price_asc")
            {
                query = query.OrderBy(p => p.ProductPrice);
            }
            else if (sort == "price_desc")
            {
                query = query.OrderByDescending(p => p.ProductPrice);
            }
            else
            {
                // newest products first
                query = query.OrderByDescending(p => p.ProductId);
            }

            // pagination functionality
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var products = query.ToList();

            ViewBag.Products = products;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            var storeSearchModel = new StoreSearchModel()
            {
                Search = search,
                ProductBrand = brand,
                ProductCategory = category,
                Sort = sort
            };

            // Get distinct product brands
            var brands = context.Products.Select(p => p.ProductBrand).Distinct().ToList();
            ViewBag.Brands = brands;

            return View(storeSearchModel);
        }

        public IActionResult Details(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Store");
            }

            return View(product);
        }

    }



}

