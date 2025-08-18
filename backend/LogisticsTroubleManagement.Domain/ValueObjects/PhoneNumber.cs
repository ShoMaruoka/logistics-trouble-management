using System.Text.RegularExpressions;

namespace LogisticsTroubleManagement.Domain.ValueObjects;

public class PhoneNumber
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        if (!IsValidPhoneNumber(phoneNumber))
            throw new ArgumentException("Invalid phone number format", nameof(phoneNumber));

        return new PhoneNumber(NormalizePhoneNumber(phoneNumber));
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        // 日本の電話番号形式に対応
        // 固定電話: 0X-XXXX-XXXX または 0X-XXX-XXXX
        // 携帯電話: 0X0-XXXX-XXXX
        var normalized = NormalizePhoneNumber(phoneNumber);
        var regex = new Regex(@"^0[0-9]{1,4}[0-9]{1,4}[0-9]{4}$");
        return regex.IsMatch(normalized);
    }

    private static string NormalizePhoneNumber(string phoneNumber)
    {
        // ハイフン、スペース、括弧を除去
        return Regex.Replace(phoneNumber, @"[\s\-\(\)]", "");
    }

    public string ToFormattedString()
    {
        if (Value.Length == 10)
        {
            // 固定電話: 0X-XXXX-XXXX
            return $"{Value.Substring(0, 2)}-{Value.Substring(2, 4)}-{Value.Substring(6, 4)}";
        }
        else if (Value.Length == 11)
        {
            // 携帯電話: 0X0-XXXX-XXXX
            return $"{Value.Substring(0, 3)}-{Value.Substring(3, 4)}-{Value.Substring(7, 4)}";
        }
        return Value;
    }

    public override string ToString()
    {
        return Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is PhoneNumber other)
            return Value.Equals(other.Value);
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(PhoneNumber? left, PhoneNumber? right)
    {
        if (left is null && right is null)
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }

    public static bool operator !=(PhoneNumber? left, PhoneNumber? right)
    {
        return !(left == right);
    }

    public static implicit operator string(PhoneNumber phoneNumber)
    {
        return phoneNumber.Value;
    }
}
