using Microsoft.EntityFrameworkCore;
using NorthwindConsole.Model;
using NLog;

namespace NorthwindConsole.Services;

public static class ProductService
{
    public static void DisplayProducts(Logger logger) //Choice 1
    {
        Console.WriteLine("Display Products:");
        Console.WriteLine("1) All Products");
        Console.WriteLine("2) Active Products");
        Console.WriteLine("3) Discontinued Products");

        string? filter = Console.ReadLine();

        using var db = new DataContext();

        var query = db.Products.AsQueryable();

        if (filter == "2")
        {
            query = query.Where(p => !p.Discontinued);
        }
        else if (filter == "3")
        {
            query = query.Where(p => p.Discontinued);
        }
        else if (filter != "1")
        {
            ConsoleHelper.WriteError("Invalid filter option.");
            logger.Warn("Invalid product filter selected");
            return;
        }

        query = query.OrderBy(p => p.ProductName);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{query.Count()} records returned");
        Console.ForegroundColor = ConsoleColor.White;

        foreach (var product in query)
        {
            if (product.Discontinued)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{product.ProductName} (DISCONTINUED)");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(product.ProductName);
            }
        }

        Console.ForegroundColor = ConsoleColor.White;
        logger.Info("Displayed products with filter {filter}", filter);
    }
    public static void DisplaySpecificProduct(Logger logger) //Choice 2
    {
        using var db = new DataContext();

        Console.WriteLine("Enter Product ID:");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            ConsoleHelper.WriteError("Invalid Product ID.");
            logger.Warn("Invalid Product ID entered.");
            return;
        }

        Product? product = db.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefault(p => p.ProductId == id);

        if (product == null)
        {
            ConsoleHelper.WriteError("Product not found.");
            logger.Warn("Product ID {id} not found.", id);
            return;
        }

        Console.WriteLine($"Product ID: {product.ProductId}");
        Console.WriteLine($"Product Name: {product.ProductName}");
        Console.WriteLine($"Supplier ID: {product.SupplierId}");
        Console.WriteLine($"Supplier: {product.Supplier?.CompanyName ?? "None"}");
        Console.WriteLine($"Category ID: {product.CategoryId}");
        Console.WriteLine($"Category: {product.Category?.CategoryName ?? "None"}");
        Console.WriteLine($"Quantity Per Unit: {product.QuantityPerUnit}");
        Console.WriteLine($"Unit Price: {product.UnitPrice:C}");
        Console.WriteLine($"Units In Stock: {product.UnitsInStock}");
        Console.WriteLine($"Units On Order: {product.UnitsOnOrder}");
        Console.WriteLine($"Reorder Level: {product.ReorderLevel}");
        Console.WriteLine($"Discontinued: {(product.Discontinued ? "Yes" : "No")}");

        logger.Info("Displayed Product ID {id}.", id);
    }
}