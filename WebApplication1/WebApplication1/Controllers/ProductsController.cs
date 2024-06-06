using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly int pageSize = 5;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index(int pageIndex, string? search, string? column, string? orderBy)
        {
            IQueryable<Product> query = context.Products;

            // search functionality
            if (search != null)
            {
                query = query.Where(p => p.ProductName.Contains(search) || p.ProductBrand.Contains(search));
            }

            // sort functionality
            string[] validColumns = { "ProductId", "ProductName", "ProductBrand", "ProductCategory", "ProductPrice" };
            string[] validOrderBy = { "desc", "asc" };
            
            if (!validColumns.Contains(column))
            {
                column = "ProductId";
            }

            if (!validOrderBy.Contains(orderBy))
            {
                orderBy = "desc";
            }

            if (column == "ProductName")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.ProductName);
                }
                else
                {
                    query = query.OrderByDescending(p => p.ProductName);
                }
            }
            else if (column == "ProductBrand")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.ProductBrand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.ProductBrand);
                }
            }
            else if (column == "ProductCategory")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.ProductCategory);
                }
                else
                {
                    query = query.OrderByDescending(p => p.ProductCategory);
                }
            }
            else if (column == "ProductPrice")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.ProductPrice);
                }
                else
                {
                    query = query.OrderByDescending(p => p.ProductPrice);
                }
            }
           
           
            else
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.ProductId);
                }
                else
                {
                    query = query.OrderByDescending(p => p.ProductId);
                }
            }


            //query = query.OrderByDescending(p => p.ProductId);

            //pagination functionality
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var products = query.ToList();

            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;

            ViewData["Search"] = search ?? "";

            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderBy;

            return View(products);
        }
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductDto productDto)

        {
            if (productDto.ProductImage == null)
            {
                ModelState.AddModelError("ProductImage", "Product Image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            // save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ProductImage!.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ProductImage.CopyTo(stream);
            }

            // save the new product in the database
            Product product = new Product()
            {
                ProductName = productDto.ProductName,
                ProductBrand = productDto.ProductBrand,
                ProductCategory = productDto.ProductCategory,
                ProductPrice = productDto.ProductPrice,
                ProductDescription = productDto.ProductDescription,
                ProductImageName = newFileName,
                
            };
            context.Products.Add(product);
            context.SaveChanges();
            return RedirectToAction("Index","Products");

         
        }
        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            var productDto = new ProductDto()
            {
                ProductName = product.ProductName,
                ProductBrand = product.ProductBrand,
                ProductCategory = product.ProductCategory,
                ProductPrice = product.ProductPrice,
                ProductDescription = product.ProductDescription,
            };

            ViewData["ProductId"] = product.ProductId;
            ViewData["ProductImageName"] = product.ProductImageName;
           

            return View(productDto);
        }
            [HttpPost]
            public IActionResult Edit(int id, ProductDto productDto)
            {
                var product = context.Products.Find(id);

                if (product == null)
                {
                    return RedirectToAction("Index", "Products");
                }


                if (!ModelState.IsValid)
                {
                ViewData["ProductId"] = product.ProductId;
                ViewData["ProductImageName"] = product.ProductImageName;

                return View(productDto);
                }


                // update the image file if we have a new image file
                string newFileName = product.ProductImageName;
                if (productDto.ProductImage != null)
                {
                    newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    newFileName += Path.GetExtension(productDto.ProductImage.FileName);

                    string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
                    using (var stream = System.IO.File.Create(imageFullPath))
                    {
                        productDto.ProductImage.CopyTo(stream);
                    }

                    // delete the old image
                    string oldImageFullPath = environment.WebRootPath + "/products/" + product.ProductImageName;
                    System.IO.File.Delete(oldImageFullPath);
                }


                // update the product in the database
                product.ProductName = productDto.ProductName;
                product.ProductBrand = productDto.ProductBrand;
                product.ProductCategory = productDto.ProductCategory;
                product.ProductPrice = productDto.ProductPrice;
                product.ProductDescription = productDto.ProductDescription;
                product.ProductImageName = newFileName;


                context.SaveChanges();

                return RedirectToAction("Index", "Products");
            }
        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            string imageFullPath = environment.WebRootPath + "/products/" + product.ProductImageName;
            System.IO.File.Delete(imageFullPath);

            context.Products.Remove(product);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Products");
        }
    }
}
