using Microsoft.EntityFrameworkCore;
using SchoolLibrary.Data;

namespace SchoolLibrary.Services;

/// <summary>
/// Генератор ISBN-13 с корректной контрольной цифрой.
/// Формат: 978-5-XXXX-XXXX-X (префикс 978-5 = Россия).
/// </summary>
public static class IsbnGenerator
{
    // 978 — префикс книги, 5 — регистрационная группа Россия
    private const string Prefix = "9785";

    /// <summary>Генерирует ISBN без проверки уникальности.</summary>
    public static string Generate()
    {
        var rnd = Random.Shared;
        var sb = new System.Text.StringBuilder(Prefix); // 4 цифры
        for (int i = 0; i < 8; i++)
            sb.Append(rnd.Next(0, 10)); // всего 12 цифр

        var first12 = sb.ToString();
        var check = ComputeCheckDigit(first12);

        // 978-5-XXXX-XXXX-X
        return $"{first12[..3]}-{first12[3]}-{first12.Substring(4, 4)}-{first12.Substring(8, 4)}-{check}";
    }

    /// <summary>Генерирует ISBN, гарантированно отсутствующий в таблице books.</summary>
    public static async Task<string> GenerateUniqueAsync(AppDbContext db)
    {
        // 10^8 комбинаций — коллизии практически невозможны,
        // но на всякий случай делаем несколько попыток.
        for (int attempt = 0; attempt < 25; attempt++)
        {
            var isbn = Generate();
            var exists = await db.Books.AnyAsync(b => b.Isbn == isbn);
            if (!exists) return isbn;
        }

        throw new InvalidOperationException(
            "Не удалось сгенерировать уникальный ISBN за 25 попыток — это крайне маловероятно, попробуйте ещё раз.");
    }

    /// <summary>Контрольная цифра ISBN-13 (алгоритм EAN-13).</summary>
    private static int ComputeCheckDigit(string first12)
    {
        if (first12.Length != 12)
            throw new ArgumentException("Ожидалось 12 цифр");

        int sum = 0;
        for (int i = 0; i < 12; i++)
        {
            var digit = first12[i] - '0';
            sum += digit * (i % 2 == 0 ? 1 : 3);
        }
        return (10 - (sum % 10)) % 10;
    }
}
