using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsTroubleManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IIncidentRepository _incidentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AttachmentsController> _logger;

    public AttachmentsController(
        IUnitOfWork unitOfWork,
        IAttachmentRepository attachmentRepository,
        IIncidentRepository incidentRepository,
        IUserRepository userRepository,
        ILogger<AttachmentsController> logger)
    {
        _unitOfWork = unitOfWork;
        _attachmentRepository = attachmentRepository;
        _incidentRepository = incidentRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    // GET: api/attachments
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<AttachmentDto>>> GetAttachments([FromQuery] AttachmentSearchDto searchDto)
    {
        try
        {
            _logger.LogInformation("ファイル一覧取得リクエスト");

            var predicate = BuildSearchPredicate(searchDto);
            var orderBy = GetOrderByExpression(searchDto.SortBy);

            var (attachments, totalCount) = await _attachmentRepository.GetPagedAsync(
                predicate,
                orderBy,
                searchDto.Ascending,
                searchDto.Page,
                searchDto.PageSize);

            var attachmentDtos = new List<AttachmentDto>();
            foreach (var attachment in attachments)
            {
                var dto = await ConvertToDtoAsync(attachment);
                attachmentDtos.Add(dto);
            }

            var result = new PagedResultDto<AttachmentDto>(
                attachmentDtos,
                totalCount,
                searchDto.Page,
                searchDto.PageSize);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ファイル一覧取得中にエラーが発生しました。");
            return StatusCode(500, new { Error = "ファイル一覧取得中にエラーが発生しました。" });
        }
    }

    // GET: api/attachments/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<AttachmentDto>> GetAttachment(int id)
    {
        try
        {
            _logger.LogInformation($"ファイル詳細取得リクエスト (ID: {id})");

            var attachment = await _attachmentRepository.GetByIdWithIncludeAsync(id, a => a.Incident, a => a.UploadedBy);
            if (attachment == null)
            {
                return NotFound(new { Error = "指定されたファイルが見つかりません。" });
            }

            var dto = await ConvertToDtoAsync(attachment);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"ファイル詳細取得中にエラーが発生しました。 (ID: {id})");
            return StatusCode(500, new { Error = "ファイル詳細取得中にエラーが発生しました。" });
        }
    }

    // GET: api/attachments/incident/{incidentId}
    [HttpGet("incident/{incidentId}")]
    public async Task<ActionResult<IEnumerable<AttachmentDto>>> GetAttachmentsByIncident(int incidentId)
    {
        try
        {
            _logger.LogInformation($"インシデント別ファイル一覧取得リクエスト (IncidentID: {incidentId})");

            var attachments = await _attachmentRepository.GetAttachmentsByIncidentAsync(incidentId);
            var attachmentDtos = new List<AttachmentDto>();

            foreach (var attachment in attachments)
            {
                var dto = await ConvertToDtoAsync(attachment);
                attachmentDtos.Add(dto);
            }

            return Ok(attachmentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"インシデント別ファイル一覧取得中にエラーが発生しました。 (IncidentID: {incidentId})");
            return StatusCode(500, new { Error = "インシデント別ファイル一覧取得中にエラーが発生しました。" });
        }
    }

    // POST: api/attachments
    [HttpPost]
    public async Task<ActionResult<AttachmentDto>> CreateAttachment([FromBody] CreateAttachmentDto createDto)
    {
        try
        {
            _logger.LogInformation("ファイル登録リクエスト");

            // インシデントの存在確認
            var incident = await _incidentRepository.GetByIdAsync(createDto.IncidentId);
            if (incident == null)
            {
                return BadRequest(new { Error = "指定されたインシデントが見つかりません。" });
            }

            // ユーザーの存在確認
            var user = await _userRepository.GetByIdAsync(createDto.UploadedById);
            if (user == null)
            {
                return BadRequest(new { Error = "指定されたユーザーが見つかりません。" });
            }

            // ファイルパスの生成（モック版では仮のパス）
            var filePath = $"/uploads/{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid()}_{createDto.FileName}";

            var attachment = Domain.Entities.Attachment.Create(
                createDto.IncidentId,
                createDto.FileName,
                filePath,
                createDto.FileSize,
                createDto.ContentType,
                createDto.UploadedById);

            var createdAttachment = await _attachmentRepository.AddAsync(attachment);
            await _unitOfWork.SaveChangesAsync();

            var dto = await ConvertToDtoAsync(createdAttachment);
            return CreatedAtAction(nameof(GetAttachment), new { id = dto.Id }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ファイル登録中にエラーが発生しました。");
            return StatusCode(500, new { Error = "ファイル登録中にエラーが発生しました。" });
        }
    }

    // DELETE: api/attachments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAttachment(int id)
    {
        try
        {
            _logger.LogInformation($"ファイル削除リクエスト (ID: {id})");

            var attachment = await _attachmentRepository.GetByIdAsync(id);
            if (attachment == null)
            {
                return NotFound(new { Error = "指定されたファイルが見つかりません。" });
            }

            await _attachmentRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"ファイル削除中にエラーが発生しました。 (ID: {id})");
            return StatusCode(500, new { Error = "ファイル削除中にエラーが発生しました。" });
        }
    }

    // GET: api/attachments/{id}/download
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadAttachment(int id)
    {
        try
        {
            _logger.LogInformation($"ファイルダウンロードリクエスト (ID: {id})");

            var attachment = await _attachmentRepository.GetByIdAsync(id);
            if (attachment == null)
            {
                return NotFound(new { Error = "指定されたファイルが見つかりません。" });
            }

            // モック版では、実際のファイルは存在しないため、ダミーデータを返す
            var dummyContent = $"This is a mock file content for {attachment.FileName}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(dummyContent);

            return File(bytes, attachment.ContentType, attachment.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"ファイルダウンロード中にエラーが発生しました。 (ID: {id})");
            return StatusCode(500, new { Error = "ファイルダウンロード中にエラーが発生しました。" });
        }
    }

    private async Task<AttachmentDto> ConvertToDtoAsync(Domain.Entities.Attachment attachment)
    {
        var incident = await _incidentRepository.GetByIdAsync(attachment.IncidentId);
        var uploadedBy = await _userRepository.GetByIdAsync(attachment.UploadedById);

        return new AttachmentDto
        {
            Id = attachment.Id,
            FileName = attachment.FileName,
            ContentType = attachment.ContentType,
            FileSize = attachment.FileSize,
            UploadedAt = attachment.CreatedAt,
            IncidentId = attachment.IncidentId,
            IncidentTitle = incident?.Title ?? "不明",
            UploadedBy = uploadedBy?.GetFullName() ?? "不明"
        };
    }

    private System.Linq.Expressions.Expression<Func<Domain.Entities.Attachment, bool>>? BuildSearchPredicate(AttachmentSearchDto searchDto)
    {
        if (searchDto.IncidentId.HasValue)
        {
            return a => a.IncidentId == searchDto.IncidentId.Value;
        }

        if (!string.IsNullOrEmpty(searchDto.FileName))
        {
            return a => a.FileName.Contains(searchDto.FileName);
        }

        if (!string.IsNullOrEmpty(searchDto.ContentType))
        {
            return a => a.ContentType.Contains(searchDto.ContentType);
        }

        if (searchDto.UploadedById.HasValue)
        {
            return a => a.UploadedById == searchDto.UploadedById.Value;
        }

        if (searchDto.UploadedFrom.HasValue || searchDto.UploadedTo.HasValue)
        {
            return a => (!searchDto.UploadedFrom.HasValue || a.CreatedAt >= searchDto.UploadedFrom.Value) &&
                       (!searchDto.UploadedTo.HasValue || a.CreatedAt <= searchDto.UploadedTo.Value);
        }

        return null;
    }

    private System.Linq.Expressions.Expression<Func<Domain.Entities.Attachment, object>>? GetOrderByExpression(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "filename" => a => a.FileName,
            "filesize" => a => a.FileSize,
            "contenttype" => a => a.ContentType,
            "incidentid" => a => a.IncidentId,
            "uploadedby" => a => a.UploadedById,
            _ => a => a.CreatedAt
        };
    }
}
