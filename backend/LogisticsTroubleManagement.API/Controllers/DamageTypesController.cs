using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Validators;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsTroubleManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DamageTypesController : ControllerBase
    {
        private readonly IDamageTypeRepository _damageTypeRepository;
        private readonly CreateDamageTypeDtoValidator _createValidator;
        private readonly UpdateDamageTypeDtoValidator _updateValidator;

        public DamageTypesController(
            IDamageTypeRepository damageTypeRepository,
            CreateDamageTypeDtoValidator createValidator,
            UpdateDamageTypeDtoValidator updateValidator)
        {
            _damageTypeRepository = damageTypeRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DamageTypeDto>>> GetAll()
        {
            var damageTypes = await _damageTypeRepository.GetBySortOrderAsync();
            var dtos = damageTypes.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<DamageTypeDto>>> GetActive()
        {
            var damageTypes = await _damageTypeRepository.GetActiveAsync();
            var dtos = damageTypes.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<DamageTypeDto>>> GetByCategory(string category)
        {
            var damageTypes = await _damageTypeRepository.GetByCategoryAsync(category);
            var dtos = damageTypes.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DamageTypeDto>> GetById(int id)
        {
            var damageType = await _damageTypeRepository.GetByIdAsync(id);
            if (damageType == null)
                return NotFound();

            return Ok(MapToDto(damageType));
        }

        [HttpPost]
        public async Task<ActionResult<DamageTypeDto>> Create(CreateDamageTypeDto createDto)
        {
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            if (await _damageTypeRepository.ExistsByNameAsync(createDto.Name))
                return BadRequest("指定された名称は既に存在します");

            var damageType = new DamageType(
                createDto.Name,
                createDto.Description,
                createDto.Category,
                createDto.SortOrder
            );

            await _damageTypeRepository.AddAsync(damageType);
            await _damageTypeRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = damageType.Id }, MapToDto(damageType));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDamageTypeDto updateDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var damageType = await _damageTypeRepository.GetByIdAsync(id);
            if (damageType == null)
                return NotFound();

            if (await _damageTypeRepository.ExistsByNameAsync(updateDto.Name, id))
                return BadRequest("指定された名称は既に存在します");

            damageType.Update(updateDto.Name, updateDto.Description, updateDto.Category, updateDto.SortOrder);
            damageType.SetActive(updateDto.IsActive);

            await _damageTypeRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var damageType = await _damageTypeRepository.GetByIdAsync(id);
            if (damageType == null)
                return NotFound();

            // 論理削除（無効化）
            damageType.SetActive(false);
            await _damageTypeRepository.SaveChangesAsync();

            return NoContent();
        }

        private static DamageTypeDto MapToDto(DamageType damageType)
        {
            return new DamageTypeDto
            {
                Id = damageType.Id,
                Name = damageType.Name,
                Description = damageType.Description,
                Category = damageType.Category,
                SortOrder = damageType.SortOrder,
                IsActive = damageType.IsActive
            };
        }
    }
}
