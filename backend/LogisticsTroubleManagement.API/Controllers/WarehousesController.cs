using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Validators;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsTroubleManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly CreateWarehouseDtoValidator _createValidator;
        private readonly UpdateWarehouseDtoValidator _updateValidator;

        public WarehousesController(
            IWarehouseRepository warehouseRepository,
            CreateWarehouseDtoValidator createValidator,
            UpdateWarehouseDtoValidator updateValidator)
        {
            _warehouseRepository = warehouseRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetAll()
        {
            var warehouses = await _warehouseRepository.GetBySortOrderAsync();
            var dtos = warehouses.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetActive()
        {
            var warehouses = await _warehouseRepository.GetActiveAsync();
            var dtos = warehouses.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseDto>> GetById(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
                return NotFound();

            return Ok(MapToDto(warehouse));
        }

        [HttpPost]
        public async Task<ActionResult<WarehouseDto>> Create(CreateWarehouseDto createDto)
        {
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            if (await _warehouseRepository.ExistsByNameAsync(createDto.Name))
                return BadRequest("指定された名称は既に存在します");

            var warehouse = new Warehouse(
                createDto.Name,
                createDto.Description,
                createDto.Location,
                createDto.ContactInfo,
                createDto.SortOrder
            );

            await _warehouseRepository.AddAsync(warehouse);
            await _warehouseRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = warehouse.Id }, MapToDto(warehouse));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateWarehouseDto updateDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
                return NotFound();

            if (await _warehouseRepository.ExistsByNameAsync(updateDto.Name, id))
                return BadRequest("指定された名称は既に存在します");

            warehouse.Update(updateDto.Name, updateDto.Description, updateDto.Location, 
                updateDto.ContactInfo, updateDto.SortOrder);
            warehouse.SetActive(updateDto.IsActive);

            await _warehouseRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
                return NotFound();

            // 論理削除（無効化）
            warehouse.SetActive(false);
            await _warehouseRepository.SaveChangesAsync();

            return NoContent();
        }

        private static WarehouseDto MapToDto(Warehouse warehouse)
        {
            return new WarehouseDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Description = warehouse.Description,
                Location = warehouse.Location,
                ContactInfo = warehouse.ContactInfo,
                SortOrder = warehouse.SortOrder,
                IsActive = warehouse.IsActive
            };
        }
    }
}
