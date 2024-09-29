using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace MyApp.Infrastructure.Database;

public static class CustomModelBuilderNamingConventionExtensions
{
    public static void ApplyCustomNamingConvention(this ModelBuilder modelBuilder)
    {
        var modelEntityTypes = modelBuilder.Model.GetEntityTypes();

        foreach (var tableConfiguration in modelEntityTypes)
        {
            // Table Naming
            tableConfiguration.SetTableName(tableConfiguration.GetTableName()!.ToLowerUnderscoreName());

            // Column Naming
            var columnsProperties = tableConfiguration.GetProperties();

            foreach (var columnsProperty in columnsProperties)
            {
                columnsProperty.SetColumnName(columnsProperty.Name.ToLowerUnderscoreName());
            }

            // Find primary key
            var pk = tableConfiguration.FindPrimaryKey();
            pk!.SetName(pk!.GetName()!.ToLowerUnderscoreName());

            // Foreign keys
            var fks = tableConfiguration.GetForeignKeys();

            foreach (var fk in fks)
            {
                var fkName = fk.GetConstraintName()!.ToLowerUnderscoreName();
                fk.SetConstraintName(fkName);
            }

            // Indexes
            var idxs = tableConfiguration.GetIndexes();

            foreach (var idx in idxs)
            {
                idx.SetDatabaseName(idx.GetDatabaseName()!.ToLowerUnderscoreName());
            }
        }
    }

    public static string ToLowerUnderscoreName(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var builder = new StringBuilder(text.Length + Math.Min(2, text.Length / 5));
        var previousCategory = default(UnicodeCategory?);

        for (var currentIndex = 0; currentIndex < text.Length; currentIndex++)
        {
            var currentChar = text[currentIndex];

            if (currentChar == '_')
            {
                builder.Append('_');
                previousCategory = null;
                continue;
            }

            var currentCategory = char.GetUnicodeCategory(currentChar);

            switch (currentCategory)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                    if (previousCategory == UnicodeCategory.SpaceSeparator ||
                            previousCategory == UnicodeCategory.LowercaseLetter ||
                            previousCategory != UnicodeCategory.DecimalDigitNumber &&
                            previousCategory != null &&
                            currentIndex > 0 &&
                            currentIndex + 1 < text.Length &&
                            char.IsLower(text[currentIndex + 1]))
                    {
                        builder.Append('_');
                    }

                    currentChar = char.ToLower(currentChar, CultureInfo.InvariantCulture);
                    break;

                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    if (previousCategory == UnicodeCategory.SpaceSeparator)
                    {
                        builder.Append('_');
                    }
                    break;

                default:
                    if (previousCategory != null)
                    {
                        previousCategory = UnicodeCategory.SpaceSeparator;
                    }
                    continue;
            }

            builder.Append(currentChar);
            previousCategory = currentCategory;
        }

        return builder.ToString();
    }
}

