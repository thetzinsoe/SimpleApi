using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Models;
using SimpleApi.Repositories;

namespace SimpleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public ProductsController(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> Get([FromQuery] PaginationQueryDto query)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(query.PageNumber, query.PageSize);
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(items);

        var result = new PagedResult<ProductDto>
        {
            Data = productDtos,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> Get(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<ProductDto>(product));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Post([FromBody] CreateProductDto dto, [FromServices] IValidator<CreateProductDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var product = _mapper.Map<Product>(dto);
        var createdProduct = await _repository.CreateAsync(product);

        var resultDto = _mapper.Map<ProductDto>(createdProduct);
        return CreatedAtAction(nameof(Get), new { id = resultDto.Id }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateProductDto dto, [FromServices] IValidator<UpdateProductDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _mapper.Map(dto, product);
        await _repository.UpdateAsync(product);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(product);

        return NoContent();
    }
}