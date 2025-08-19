using System.Net;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.API.Middleware;

public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;

	public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task Invoke(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex);
		}
	}

	private async Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		var traceId = context.TraceIdentifier;
		var (statusCode, message, code, errors) = MapException(exception);

		_logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}, Code: {Code}", traceId, code);

		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)statusCode;

		var response = new ErrorResponse
		{
			TraceId = traceId,
			Message = message,
			Code = code,
			Errors = errors
		};

		await context.Response.WriteAsJsonAsync(response);
	}

	private static (HttpStatusCode StatusCode, string Message, string Code, Dictionary<string, string[]>? Errors) MapException(Exception ex)
	{
		switch (ex)
		{
			case ValidationException validationException:
				return (
					HttpStatusCode.BadRequest,
					"入力値に誤りがあります。",
					"VALIDATION_ERROR",
					validationException.Errors
						.GroupBy(e => e.PropertyName)
						.ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
				);

			case ArgumentException argumentException:
				return (HttpStatusCode.BadRequest, argumentException.Message, "ARGUMENT_ERROR", null);

			case KeyNotFoundException keyNotFoundException:
				return (HttpStatusCode.NotFound, keyNotFoundException.Message, "NOT_FOUND", null);

			case UnauthorizedAccessException:
				return (HttpStatusCode.Unauthorized, "認証が必要です。", "UNAUTHORIZED", null);

			case DbUpdateException dbUpdateException:
				return (HttpStatusCode.Conflict, dbUpdateException.InnerException?.Message ?? dbUpdateException.Message, "DB_UPDATE_ERROR", null);

			case NotImplementedException:
				return (HttpStatusCode.NotImplemented, "未実装の機能です。", "NOT_IMPLEMENTED", null);

			default:
				return (HttpStatusCode.InternalServerError, "サーバー内部でエラーが発生しました。", "INTERNAL_SERVER_ERROR", null);
		}
	}
}

public class ErrorResponse
{
	public string TraceId { get; set; } = string.Empty;
	public string Message { get; set; } = string.Empty;
	public string Code { get; set; } = string.Empty;
	public Dictionary<string, string[]>? Errors { get; set; }
}
