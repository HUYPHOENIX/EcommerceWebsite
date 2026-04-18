using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using SharedViewModel.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller
{
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
        
        return CreatedAtAction(nameof(GetProducts), new { id = productDto.Id }, productDto);
    }
}
}