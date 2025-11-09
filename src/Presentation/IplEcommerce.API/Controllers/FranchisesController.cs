using Microsoft.AspNetCore.Mvc;
using IplEcommerce.Domain.Interfaces;
using IplEcommerce.Domain.Entities;
using IplEcommerce.Application.DTOs;
using IplEcommerce.Application.Common;

namespace IplEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FranchisesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public FranchisesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<IEnumerable<FranchiseDto>>>> GetFranchises()
    {
        try
        {
            var franchises = await _unitOfWork.Repository<Franchise>().GetAllAsync();
            var franchiseDtos = franchises.Where(f => f.IsActive).Select(MapToDto);
            return Ok(BaseResponse<IEnumerable<FranchiseDto>>.Success(franchiseDtos));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<IEnumerable<FranchiseDto>>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BaseResponse<FranchiseDto>>> GetFranchise(int id)
    {
        try
        {
            var franchise = await _unitOfWork.Repository<Franchise>().GetByIdAsync(id);
            if (franchise == null || !franchise.IsActive)
            {
                return NotFound(BaseResponse<FranchiseDto>.Failure("Franchise not found"));
            }

            var franchiseDto = MapToDto(franchise);
            return Ok(BaseResponse<FranchiseDto>.Success(franchiseDto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<FranchiseDto>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    private static FranchiseDto MapToDto(Franchise franchise)
    {
        return new FranchiseDto
        {
            Id = franchise.Id,
            Name = franchise.Name,
            ShortName = franchise.ShortName,
            City = franchise.City,
            Color = franchise.Color,
            LogoUrl = franchise.LogoUrl,
            Description = franchise.Description
        };
    }
}