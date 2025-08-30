using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Validators;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsTroubleManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TroubleTypesController : ControllerBase
    {
        private readonly ITroubleTypeRepository _troubleTypeRepository;
        private readonly CreateTroubleTypeDtoValidator _createValidator;
        private readonly UpdateTroubleTypeDtoValidator _updateValidator;
        private readonly IUnitOfWork _unitOfWork;

        public TroubleTypesController(
            ITroubleTypeRepository troubleTypeRepository,
            CreateTroubleTypeDtoValidator createValidator,
            UpdateTroubleTypeDtoValidator updateValidator,
            IUnitOfWork unitOfWork)
        {
            _troubleTypeRepository = troubleTypeRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TroubleTypeDto>>> GetAll()
        {
            var troubleTypes = await _troubleTypeRepository.GetBySortOrderAsync();
            var dtos = troubleTypes.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<TroubleTypeDto>>> GetActive()
        {
            var troubleTypes = await _troubleTypeRepository.GetActiveAsync();
            var dtos = troubleTypes.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TroubleTypeDto>> GetById(int id)
        {
            var troubleType = await _troubleTypeRepository.GetByIdAsync(id);
            if (troubleType == null)
                return NotFound();

            return Ok(MapToDto(troubleType));
        }

        [HttpPost]
        public async Task<ActionResult<TroubleTypeDto>> Create(CreateTroubleTypeDto createDto)
        {
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            if (await _troubleTypeRepository.ExistsByNameAsync(createDto.Name))
                return BadRequest("指定された名称は既に存在します");

            var troubleType = new TroubleType(
                createDto.Name,
                createDto.Description,
                createDto.Color,
                createDto.SortOrder
            );

            await _troubleTypeRepository.AddAsync(troubleType);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = troubleType.Id }, MapToDto(troubleType));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTroubleTypeDto updateDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var troubleType = await _troubleTypeRepository.GetByIdAsync(id);
            if (troubleType == null)
                return NotFound();

            if (await _troubleTypeRepository.ExistsByNameAsync(updateDto.Name, id))
                return BadRequest("指定された名称は既に存在します");

            troubleType.Update(updateDto.Name, updateDto.Description, updateDto.Color, updateDto.SortOrder);
            troubleType.SetActive(updateDto.IsActive);

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var troubleType = await _troubleTypeRepository.GetByIdAsync(id);
            if (troubleType == null)
                return NotFound();

            // 論理削除（無効化）
            troubleType.SetActive(false);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        private static TroubleTypeDto MapToDto(TroubleType troubleType)
        {
            return new TroubleTypeDto
            {
                Id = troubleType.Id,
                Name = troubleType.Name,
                Description = troubleType.Description,
                Color = troubleType.Color,
                SortOrder = troubleType.SortOrder,
                IsActive = troubleType.IsActive
            };
        }
    }
}
