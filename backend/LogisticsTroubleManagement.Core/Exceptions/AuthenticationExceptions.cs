namespace LogisticsTroubleManagement.Core.Exceptions;

/// <summary>
/// 認証・認可に関する例外クラス
/// </summary>

/// <summary>
/// アクセス権限がない場合の例外
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "アクセス権限がありません") : base(message) { }
    
    public ForbiddenException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// トークンが無効な場合の例外
/// </summary>
public class InvalidTokenException : Exception
{
    public InvalidTokenException(string message = "トークンが無効です") : base(message) { }
    
    public InvalidTokenException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// トークンの有効期限が切れている場合の例外
/// </summary>
public class TokenExpiredException : Exception
{
    public TokenExpiredException(string message = "トークンの有効期限が切れています") : base(message) { }
    
    public TokenExpiredException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// 認証が必要な場合の例外
/// </summary>
public class AuthenticationRequiredException : Exception
{
    public AuthenticationRequiredException(string message = "認証が必要です") : base(message) { }
    
    public AuthenticationRequiredException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// パスワードが無効な場合の例外
/// </summary>
public class InvalidPasswordException : Exception
{
    public InvalidPasswordException(string message = "パスワードが無効です") : base(message) { }
    
    public InvalidPasswordException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// ユーザーが存在しない場合の例外
/// </summary>
public class UserNotFoundException : Exception
{
    public UserNotFoundException(string message = "ユーザーが存在しません") : base(message) { }
    
    public UserNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// ユーザーが無効な場合の例外
/// </summary>
public class UserInactiveException : Exception
{
    public UserInactiveException(string message = "ユーザーが無効です") : base(message) { }
    
    public UserInactiveException(string message, Exception innerException) : base(message, innerException) { }
}
