using Microsoft.EntityFrameworkCore;
using NorthwindConsole.Model;
using NLog;

namespace NorthwindConsole.Services;

public static class SearchReport
{
public static void ProductSearch(Logger logger)
{
    using var db = new DataContext();

    Console.WriteLine("Enter part of a product name to search:");
    string? search = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(search))
    {
        ConsoleHelper.WriteError("Search cannot be empty.");
        logger.Warn("Empty product search attempted.");
        return;
    }

    var products = db.Products
        .Include(p => p.Category)
        .Where(p => p.ProductName.Contains(search))
        .OrderBy(p => p.ProductName)
        .ToList();

    if (!products.Any())
    {
        ConsoleHelper.WriteWarning("No products found.");
        logger.Info("No products found for search: {search}", search);
        return;
    }

    ConsoleHelper.WriteSuccess($"{products.Count} product(s) found");

    Console.WriteLine();
    Console.WriteLine("ID | Product | Category | Price | Stock | Status");
    Console.WriteLine("------------------------------------------------");

    foreach (var product in products)
    {
        string status = product.Discontinued ? "Discontinued" : "Active";
        bool lowStock = product.UnitsInStock.HasValue && product.UnitsInStock.Value < 5;

        if (product.Discontinued)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        else if (lowStock)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        Console.WriteLine(
            $"{product.ProductId} | " +
            $"{product.ProductName} | " +
            $"{product.Category?.CategoryName ?? "No Category"} | " +
            $"{product.UnitPrice:C} | " +
            $"{product.UnitsInStock} | " +
            $"{status}"
        );

        Console.ForegroundColor = ConsoleColor.White;
    }

    Console.WriteLine();
    Console.WriteLine("Legend:");
    ConsoleHelper.WriteWarning("Yellow = Low stock");
    ConsoleHelper.WriteError("Red = Discontinued");

    logger.Info("Product search report completed for search: {search}", search);
    }
}