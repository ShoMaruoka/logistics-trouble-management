using System.ComponentModel.DataAnnotations;
using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Core.Validation;

/// <summary>
/// 特定のロールに対して倉庫IDを必須とするバリデーション属性
/// </summary>
public class WarehouseRequiredForRoleAttribute : ValidationAttribute
{
    private readonly int _requiredRoleId;

    public WarehouseRequiredForRoleAttribute(int requiredRoleId)
    {
        _requiredRoleId = requiredRoleId;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // リフレクションを使用してRoleIdプロパティを取得
        var roleIdProperty = validationContext.ObjectType.GetProperty("RoleId");
        if (roleIdProperty == null)
        {
            return ValidationResult.Success; // RoleIdプロパティが存在しない場合は検証をスキップ
        }

        var roleId = (int)roleIdProperty.GetValue(validationContext.ObjectInstance)!;
        
        // 指定されたロールIDの場合のみ倉庫IDを必須とする
        if (roleId == _requiredRoleId)
        {
            if (value == null || (value is int intValue && intValue <= 0))
            {
                return new ValidationResult(ErrorMessage ?? "倉庫担当ユーザーの場合は担当倉庫を選択してください。");
            }
        }

        return ValidationResult.Success;
    }
}
