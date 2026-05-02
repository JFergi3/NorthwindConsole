using Microsoft.EntityFrameworkCore;
using NorthwindConsole.Model;
using NLog;

namespace NorthwindConsole.Services;

public static class SearchReport
{
    public static void Run(Logger logger)
    {
        Console.WriteLine("What do you want to search?");
        Console.WriteLine("1) Products");
        Console.WriteLine("2) Categories");
        Console.Write("Choose an option: ");

        string? choice = Console.ReadLine();

        if (choice == "1")
        {
            ProductSearch(logger);
        }
        else if (choice == "2")
        {
            CategorySearch(logger);
        }
        else
        {
            ConsoleHelper.WriteError("Invalid search option.");
            logger.Warn("Invalid search report option selected: {choice}", choice);
        }
    }

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
                Console.ForegroundColor = ConsoleColor.Red;
            else if (lowStock)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.White;

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
    

    private static void CategorySearch(Logger logger)
    {
        using var db = new DataContext();

        Console.WriteLine("Enter part of a category name to search:");
        string? search = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(search))
        {
            ConsoleHelper.WriteError("Search cannot be empty.");
            logger.Warn("Empty category search attempted.");
            return;
        }

        var categories = db.Categories
            .Include(c => c.Products.Where(p => !p.Discontinued))
            .Where(c => c.CategoryName.Contains(search))
            .OrderBy(c => c.CategoryName)
            .ToList();

        if (!categories.Any())
        {
            ConsoleHelper.WriteWarning("No categories found.");
            logger.Info("No categories found for search: {search}", search);
            return;
        }

        ConsoleHelper.WriteSuccess($"{categories.Count} category/categories found");

        foreach (var category in categories)
        {
            Console.WriteLine();
            Console.WriteLine($"{category.CategoryName} - {category.Description}");

            if (!category.Products.Any())
            {
                Console.WriteLine("\tNo active products found.");
            }
            else
            {
                foreach (var product in category.Products.OrderBy(p => p.ProductName))
                {
                    Console.WriteLine($"\t{product.ProductName} | {product.UnitPrice:C} | Stock: {product.UnitsInStock}");
                }
            }
        }

        logger.Info("Category search report completed for search: {search}", search);
    }
}