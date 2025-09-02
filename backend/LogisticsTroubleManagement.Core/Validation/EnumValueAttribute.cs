using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LogisticsTroubleManagement.Core.Validation;

/// <summary>
/// 指定されたenum型に対してint値の検証を行うカスタムバリデーション属性
/// null値は許可し、非null整数値はEnum.IsDefinedで検証
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class EnumValueAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public EnumValueAttribute(Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException($"Type {enumType.Name} must be an enum type.", nameof(enumType));
        }
        
        _enumType = enumType;
    }

    public override bool IsValid(object? value)
    {
        // null値は許可
        if (value == null)
        {
            return true;
        }

        // int型でない場合は無効
        if (value is not int intValue)
        {
            return false;
        }

        // Enum.IsDefinedで値が有効かチェック
        return Enum.IsDefined(_enumType, intValue);
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a valid value for {_enumType.Name} enum or null.";
    }
}
