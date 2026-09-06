using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.data;
using WebApplication1.Dto;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class DashController(AppDbContext context) : Controller
    {
        public IActionResult Index()
        {
            var list = context.Products.Select(x=> new ProductDto {
                Id = x.Id,
                ProductName =x.ProductName,Description=x.Description,Price=x.Price,
            Color=x.Color}).ToList();
            return View(list);
        }

        public IActionResult ProductForm()
        {
            return View();
        }

        public async Task<IActionResult> CreateProduct(ProductDto dto)
        {
            if (dto==null)
            {
                ViewBag.ErrorMessage = "Please all The field";
                return View("ProductForm");
            }

            var product = new Product
            {
                
                ProductName = dto.ProductName,
                Description = dto.Description,
                Price = dto.Price,
                Color = dto.Color
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteProduct(int ProductId)
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == ProductId);
            if (product==null)
            {
                return NotFound();
            }
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateProductForm(int id)
        {
            var data = await context.Products.Select(x=> new ProductDto
            {
                Id = id,
                Color = x.Color,
                ProductName = x.ProductName,
                Description= x.Description,
                Price= x.Price,
            }).FirstOrDefaultAsync(x=> x.Id==id);
            return View(data);
        }

        public async Task<IActionResult> UpdateProduct(ProductDto dto)
        {
            if (dto==null)
            {
                ViewBag.ErrorMessage = "all fields are required";
                return View("UpdateProductForm");
            }
            var data = await context.Products.FirstOrDefaultAsync(x=>x.Id==dto.Id);
            if (data == null)
            {
                return NotFound();
            }

            data.ProductName = dto.ProductName;
            data.Description = dto.Description;
            data.Price = dto.Price;
            data.Color = dto.Color;
            context.Products.Update(data);
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult CancleButton()
        {
            return RedirectToAction("Index");
        }

    }
}
