using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace API.Controllers;

public class ProductsController : BaseApiController
{
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IGenericRepository<ProductBrand> _productBrandRepo;
    private readonly IGenericRepository<ProductType> _productTypeRepo;
    private readonly IMapper _mapper;

    public ProductsController(IGenericRepository<Product> productRepo,
        IGenericRepository<ProductBrand> productBrandRepo,
        IGenericRepository<ProductType> productTypeRepo, IMapper mapper)
    {
        _productRepo = productRepo;
        _productBrandRepo = productBrandRepo;
        _productTypeRepo = productTypeRepo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts()
    {
        var spec = new ProductsWithTypesAndBrandsSpecification();
        
        var products = await _productRepo.ListAsync(spec);
        
            // Manually mapping
        // return products.Select(product => new ProductToReturnDto
        // {
        //     Id = product.Id,
        //     Name = product.Name,
        //     Description = product.Description,
        //     Price = product.Price,
        //     PictureUrl = product.PictureUrl,
        //     ProductType = product.ProductType.Name,   
        //     ProductBrand = product.ProductBrand.Name
        // }).ToList();

            // mapping using AutoMapper
        return Ok(_mapper
            .Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
    }

    [HttpGet("{id}")]
    // TO DO: to fix video 55
    //[ProducesErrorResponseType(StatusCodes.Status200OK)]
    // [ProducesErrorResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
    {
        var spec = new ProductsWithTypesAndBrandsSpecification(id);

        var product = await _productRepo.GetEntityWithSpec(spec);

        if (product == null) return NotFound(new ApiResponse(404));
        
           // Manually mapping
        // return new ProductToReturnDto
        // {
        //     Id = product.Id,
        //     Name = product.Name,
        //     Description = product.Description,
        //     Price = product.Price,
        //     PictureUrl = product.PictureUrl,
        //     ProductType = product.ProductType.Name,   
        //     ProductBrand = product.ProductBrand.Name
        // };

            // mapping using AutoMapper
        return _mapper.Map<Product, ProductToReturnDto>(product);
        
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrandsAsync()
    {
        return Ok(await _productBrandRepo.ListAllAsync());
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypesAsync()
    {
        return Ok(await _productTypeRepo.ListAllAsync());
    }
}