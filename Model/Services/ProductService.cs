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
    public static void AddProduct(Logger logger) //Choice 3
    {
        using var db = new DataContext();

        Product product = new();

        Console.WriteLine("Enter Product Name:");
        product.ProductName = Console.ReadLine()!;

        Console.WriteLine("Enter Quantity Per Unit:");
        product.QuantityPerUnit = Console.ReadLine();

        Console.WriteLine("Enter Unit Price:");
        if (!decimal.TryParse(Console.ReadLine(), out decimal unitPrice))
        {
            ConsoleHelper.WriteError("Invalid price. Product was not added.");
            logger.Warn("Invalid UnitPrice entered when adding product.");
            return;
        }
        product.UnitPrice = unitPrice;

        Console.WriteLine("Enter Units In Stock:");
        if (!short.TryParse(Console.ReadLine(), out short unitsInStock))
        {
            ConsoleHelper.WriteError("Invalid units in stock. Product was not added.");
            logger.Warn("Invalid UnitsInStock entered when adding product.");
            return;
        }
        product.UnitsInStock = unitsInStock;

        Console.WriteLine("Select Category ID:");
        foreach (var category in db.Categories.OrderBy(c => c.CategoryId))
        {
            Console.WriteLine($"{category.CategoryId}) {category.CategoryName}");
        }

        if (!int.TryParse(Console.ReadLine(), out int categoryId))
        {
            ConsoleHelper.WriteError("Invalid category ID. Product was not added.");
            logger.Warn("Invalid CategoryId entered when adding product.");
            return;
        }

        if (!db.Categories.Any(c => c.CategoryId == categoryId))
        {
            ConsoleHelper.WriteError("Category does not exist. Product was not added.");
            logger.Warn("Nonexistent CategoryId {categoryId} entered when adding product.", categoryId);
            return;
        }

        product.CategoryId = categoryId;

        Console.WriteLine("Is this product discontinued? y/n");
        product.Discontinued = Console.ReadLine()?.ToLower() == "y";

        db.Products.Add(product);
        db.SaveChanges();

        ConsoleHelper.WriteSuccess("Product added successfully.");
        logger.Info("Product added: {name}", product.ProductName);
    }
    public static void EditProduct(Logger logger) //Choice 4
    {
        using var db = new DataContext();

        Console.WriteLine("Enter Product ID to edit:");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            ConsoleHelper.WriteError("Invalid Product ID.");
            logger.Warn("Invalid Product ID entered for edit.");
            return;
        }

        Product? product = db.Products.FirstOrDefault(p => p.ProductId == id);

        if (product == null)
        {
            ConsoleHelper.WriteError("Product not found.");
            logger.Warn("Product ID {id} not found for edit.", id);
            return;
        }

        Console.WriteLine($"Editing: {product.ProductName}");
        Console.WriteLine("Press Enter to keep current value.");

        Console.WriteLine($"Product Name ({product.ProductName}):");
        string? name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(name))
            product.ProductName = name;

        Console.WriteLine($"Quantity Per Unit ({product.QuantityPerUnit}):");
        string? qty = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(qty))
            product.QuantityPerUnit = qty;

        Console.WriteLine($"Unit Price ({product.UnitPrice}):");
        string? priceInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(priceInput))
        {
            if (decimal.TryParse(priceInput, out decimal price))
                product.UnitPrice = price;
            else
            {
                ConsoleHelper.WriteError("Invalid price.");
                logger.Warn("Invalid UnitPrice while editing Product ID {id}", id);
                return;
            }
        }

        Console.WriteLine($"Units In Stock ({product.UnitsInStock}):");
        string? stockInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(stockInput))
        {
            if (short.TryParse(stockInput, out short stock))
                product.UnitsInStock = stock;
            else
            {
                ConsoleHelper.WriteError("Invalid stock.");
                logger.Warn("Invalid UnitsInStock while editing Product ID {id}", id);
                return;
            }
        }

        Console.WriteLine($"Discontinued ({(product.Discontinued ? "Yes" : "No")}) y/n:");
        string? disc = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(disc))
            product.Discontinued = disc.ToLower() == "y";

        db.SaveChanges();

        ConsoleHelper.WriteSuccess("Product updated successfully.");
        logger.Info("Product ID {id} updated.", id);
    }
    public static void DeleteProduct(Logger logger) //Choice 5
    {
        using var db = new DataContext();

        Console.WriteLine("Enter Product ID to delete:");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            ConsoleHelper.WriteError("Invalid Product ID.");
            logger.Warn("Invalid Product ID entered for delete.");
            return;
        }

        Product? product = db.Products
            .Include(p => p.OrderDetails)
            .FirstOrDefault(p => p.ProductId == id);

        if (product == null)
        {
            ConsoleHelper.WriteError("Product not found.");
            logger.Warn("Product ID {id} not found for delete.", id);
            return;
        }

        if (product.OrderDetails.Any())
        {
            ConsoleHelper.WriteWarning("This product cannot be deleted because it is connected to existing order details.");
            Console.WriteLine("To avoid orphaned records, mark it as discontinued instead? y/n");

            string? choice = Console.ReadLine();

            if (choice?.ToLower() == "y")
            {
                product.Discontinued = true;
                db.SaveChanges();

                ConsoleHelper.WriteSuccess("Product marked as discontinued.");
                logger.Info("Product ID {id} marked as discontinued instead of deleted.", id);
            }
            else
            {
                Console.WriteLine("Product was not deleted.");
                logger.Info("Delete canceled for Product ID {id}.", id);
            }

            return;
        }

        Console.WriteLine($"Are you sure you want to delete {product.ProductName}? y/n");
        string? confirm = Console.ReadLine();

        if (confirm?.ToLower() == "y")
        {
            db.Products.Remove(product);
            db.SaveChanges();

            ConsoleHelper.WriteSuccess("Product deleted successfully.");
            logger.Info("Product ID {id} deleted.", id);
        }
        else
        {
            Console.WriteLine("Delete canceled.");
            logger.Info("Delete canceled for Product ID {id}.", id);
        }
    }
    //--------------Product ID Helper Method-------------------------------------------------
    
}