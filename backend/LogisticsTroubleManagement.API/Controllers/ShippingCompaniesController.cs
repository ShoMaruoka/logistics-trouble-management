using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Validators;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsTroubleManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShippingCompaniesController : ControllerBase
    {
        private readonly IShippingCompanyRepository _shippingCompanyRepository;
        private readonly CreateShippingCompanyDtoValidator _createValidator;
        private readonly UpdateShippingCompanyDtoValidator _updateValidator;
        private readonly IUnitOfWork _unitOfWork;

        public ShippingCompaniesController(
            IShippingCompanyRepository shippingCompanyRepository,
            CreateShippingCompanyDtoValidator createValidator,
            UpdateShippingCompanyDtoValidator updateValidator,
            IUnitOfWork unitOfWork)
        {
            _shippingCompanyRepository = shippingCompanyRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingCompanyDto>>> GetAll()
        {
            var shippingCompanies = await _shippingCompanyRepository.GetBySortOrderAsync();
            var dtos = shippingCompanies.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ShippingCompanyDto>>> GetActive()
        {
            var shippingCompanies = await _shippingCompanyRepository.GetActiveAsync();
            var dtos = shippingCompanies.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("company-type/{companyType}")]
        public async Task<ActionResult<IEnumerable<ShippingCompanyDto>>> GetByCompanyType(string companyType)
        {
            var shippingCompanies = await _shippingCompanyRepository.GetByCompanyTypeAsync(companyType);
            var dtos = shippingCompanies.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingCompanyDto>> GetById(int id)
        {
            var shippingCompany = await _shippingCompanyRepository.GetByIdAsync(id);
            if (shippingCompany == null)
                return NotFound();

            return Ok(MapToDto(shippingCompany));
        }

        [HttpPost]
        public async Task<ActionResult<ShippingCompanyDto>> Create(CreateShippingCompanyDto createDto)
        {
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            if (await _shippingCompanyRepository.ExistsByNameAsync(createDto.Name))
                return BadRequest("指定された名称は既に存在します");

            var shippingCompany = new ShippingCompany(
                createDto.Name,
                createDto.Description,
                createDto.CompanyType,
                createDto.ContactInfo,
                createDto.SortOrder
            );

            await _shippingCompanyRepository.AddAsync(shippingCompany);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = shippingCompany.Id }, MapToDto(shippingCompany));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateShippingCompanyDto updateDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var shippingCompany = await _shippingCompanyRepository.GetByIdAsync(id);
            if (shippingCompany == null)
                return NotFound();

            if (await _shippingCompanyRepository.ExistsByNameAsync(updateDto.Name, id))
                return BadRequest("指定された名称は既に存在します");

            shippingCompany.Update(updateDto.Name, updateDto.Description, updateDto.CompanyType, 
                updateDto.ContactInfo, updateDto.SortOrder);
            shippingCompany.SetActive(updateDto.IsActive);

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var shippingCompany = await _shippingCompanyRepository.GetByIdAsync(id);
            if (shippingCompany == null)
                return NotFound();

            // 論理削除（無効化）
            shippingCompany.SetActive(false);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        private static ShippingCompanyDto MapToDto(ShippingCompany shippingCompany)
        {
            return new ShippingCompanyDto
            {
                Id = shippingCompany.Id,
                Name = shippingCompany.Name,
                Description = shippingCompany.Description,
                CompanyType = shippingCompany.CompanyType,
                ContactInfo = shippingCompany.ContactInfo,
                SortOrder = shippingCompany.SortOrder,
                IsActive = shippingCompany.IsActive
            };
        }
    }
}
