using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using SharedViewModel.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller
{
//We use route api/controller instead of api/products at here 
//because we dont want to hardcode so if we change the class name later it will still know which controller to navigate

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productRepository.GetAllAsync();
        
        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId
        });

        return Ok(productDtos);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductByID(int id)
    {
        var product = await _productRepository.GetProductByID(id);
        if(product == null)
            {
                return NotFound();
            }
        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        };
                return Ok(productDto);
    }
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto productDto)
    {
        var newProduct = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            ImageUrl = productDto.ImageUrl,
            CategoryId = productDto.CategoryId
        };

        var createdProduct = await _productRepository.AddAsync(newProduct);
        productDto.Id = createdProduct.Id;
        //return at 201 Created status special method built into .NET .
        return CreatedAtAction(nameof(GetProducts), new { id = productDto.Id }, productDto);
    }
}
}