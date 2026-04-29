using NLog;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NorthwindConsole.Model;
using System.ComponentModel.DataAnnotations;
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

do
{
  Console.WriteLine("NORTHWIND DATABASE MENU");
  Console.WriteLine("-----------------------");

  Console.WriteLine("PRODUCTS");
  Console.WriteLine("1) Display products");
  Console.WriteLine("2) Display specific product");
  Console.WriteLine("3) Add product");
  Console.WriteLine("4) Edit product");
  Console.WriteLine("5) Delete product");

  Console.WriteLine();

  Console.WriteLine("CATEGORIES");
  Console.WriteLine("6) Display categories");
  Console.WriteLine("7) Display specific category with active products");
  Console.WriteLine("8) Display all categories with active products");
  Console.WriteLine("9) Add category");
  Console.WriteLine("10) Edit category");
  Console.WriteLine("11) Delete category");

  Console.WriteLine();

  Console.WriteLine("EXTRA");
  Console.WriteLine("12) Product search/report");

  Console.WriteLine();
  Console.WriteLine("Press Enter to quit");

  Console.Write("Choose an option: ");
  string? choice = Console.ReadLine();

  Console.Clear();
  logger.Info("Option {choice} selected", choice);

  if (choice == "1") // Display Products
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

    query = query.OrderBy(p => p.ProductName);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{query.Count()} records returned");
    Console.ForegroundColor = ConsoleColor.White;

    foreach (var product in query)
    {
      if (product.Discontinued)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{product.ProductId}) {product.ProductName} (DISCONTINUED)");
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{product.ProductId}) {product.ProductName}");
      }
    }

    Console.ForegroundColor = ConsoleColor.White;
    logger.Info("Displayed products with filter {filter}", filter);
  }

  else if (choice == "2") // Display Specific Product
  {
    using var db = new DataContext();

    Console.WriteLine("Enter Product ID:");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Invalid Product ID.");
      Console.ForegroundColor = ConsoleColor.White;

      logger.Warn("Invalid Product ID entered.");
      continue;
    }

    Product? product = db.Products
        .Include(p => p.Category)
        .Include(p => p.Supplier)
        .FirstOrDefault(p => p.ProductId == id);

    if (product == null)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Product not found.");
      Console.ForegroundColor = ConsoleColor.White;

      logger.Warn("Product ID {id} not found.", id);
      continue;
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

  else if (choice == "3") // Add Product
  {
    using var db = new DataContext();

    Product product = new();

    Console.WriteLine("Enter Product Name:");
    product.ProductName = Console.ReadLine()!;

    Console.WriteLine("Enter Quantity Per Unit:");
    product.QuantityPerUnit = Console.ReadLine();

    Console.WriteLine("Enter Unit Price:");
    if (decimal.TryParse(Console.ReadLine(), out decimal unitPrice))
    {
      product.UnitPrice = unitPrice;
    }
    else
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Invalid price. Product was not added.");
      Console.ForegroundColor = ConsoleColor.White;
      logger.Warn("Invalid UnitPrice entered when adding product.");
      continue;
    }

    Console.WriteLine("Enter Units In Stock:");
    if (short.TryParse(Console.ReadLine(), out short unitsInStock))
    {
      product.UnitsInStock = unitsInStock;
    }
    else
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Invalid units in stock. Product was not added.");
      Console.ForegroundColor = ConsoleColor.White;
      logger.Warn("Invalid UnitsInStock entered when adding product.");
      continue;
    }

    Console.WriteLine("Select Category ID:");
    var categories = db.Categories.OrderBy(c => c.CategoryId);

    foreach (var category in categories)
    {
      Console.WriteLine($"{category.CategoryId}) {category.CategoryName}");
    }

    if (int.TryParse(Console.ReadLine(), out int categoryId))
    {
      product.CategoryId = categoryId;
    }
    else
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Invalid category ID. Product was not added.");
      Console.ForegroundColor = ConsoleColor.White;
      logger.Warn("Invalid CategoryId entered when adding product.");
      continue;
    }

    Console.WriteLine("Is this product discontinued? y/n");
    string? discontinuedChoice = Console.ReadLine();

    product.Discontinued = discontinuedChoice?.ToLower() == "y";

    db.Products.Add(product);
    db.SaveChanges();

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Product added successfully.");
    Console.ForegroundColor = ConsoleColor.White;

    logger.Info("Product added: {name}", product.ProductName);
  }

  else if (choice == "4") // Edit Product
  {
    using var db = new DataContext();

    Console.WriteLine("Enter Product ID to edit:");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Invalid Product ID.");
      Console.ForegroundColor = ConsoleColor.White;
      logger.Warn("Invalid Product ID entered for edit.");
      continue;
    }

    Product? product = db.Products.FirstOrDefault(p => p.ProductId == id);

    if (product == null)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Product not found.");
      Console.ForegroundColor = ConsoleColor.White;
      logger.Warn("Product ID {id} not found for edit.", id);
      continue;
    }

    Console.WriteLine($"Editing: {product.ProductName}");
    Console.WriteLine("Press Enter to keep the current value.");

    Console.WriteLine($"Product Name ({product.ProductName}):");
    string? productName = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(productName))
    {
      product.ProductName = productName;
    }

    Console.WriteLine($"Quantity Per Unit ({product.QuantityPerUnit}):");
    string? quantityPerUnit = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(quantityPerUnit))
    {
      product.QuantityPerUnit = quantityPerUnit;
    }

    Console.WriteLine($"Unit Price ({product.UnitPrice}):");
    string? unitPriceInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(unitPriceInput))
    {
      if (decimal.TryParse(unitPriceInput, out decimal unitPrice))
      {
        product.UnitPrice = unitPrice;
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Invalid price. Product was not updated.");
        Console.ForegroundColor = ConsoleColor.White;
        logger.Warn("Invalid UnitPrice entered while editing Product ID {id}.", id);
        continue;
      }
    }

    Console.WriteLine($"Units In Stock ({product.UnitsInStock}):");
    string? unitsInStockInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(unitsInStockInput))
    {
      if (short.TryParse(unitsInStockInput, out short unitsInStock))
      {
        product.UnitsInStock = unitsInStock;
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Invalid units in stock. Product was not updated.");
        Console.ForegroundColor = ConsoleColor.White;
        logger.Warn("Invalid UnitsInStock entered while editing Product ID {id}.", id);
        continue;
      }
    }

    Console.WriteLine($"Units On Order ({product.UnitsOnOrder}):");
    string? unitsOnOrderInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(unitsOnOrderInput))
    {
      if (short.TryParse(unitsOnOrderInput, out short unitsOnOrder))
      {
        product.UnitsOnOrder = unitsOnOrder;
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Invalid units on order. Product was not updated.");
        Console.ForegroundColor = ConsoleColor.White;
        logger.Warn("Invalid UnitsOnOrder entered while editing Product ID {id}.", id);
        continue;
      }
    }

    Console.WriteLine($"Reorder Level ({product.ReorderLevel}):");
    string? reorderLevelInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(reorderLevelInput))
    {
      if (short.TryParse(reorderLevelInput, out short reorderLevel))
      {
        product.ReorderLevel = reorderLevel;
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Invalid reorder level. Product was not updated.");
        Console.ForegroundColor = ConsoleColor.White;
        logger.Warn("Invalid ReorderLevel entered while editing Product ID {id}.", id);
        continue;
      }
    }

    Console.WriteLine($"Discontinued ({(product.Discontinued ? "Yes" : "No")}) y/n or Enter to keep:");
    string? discontinuedInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(discontinuedInput))
    {
      product.Discontinued = discontinuedInput.ToLower() == "y";
    }

    db.SaveChanges();

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Product updated successfully.");
    Console.ForegroundColor = ConsoleColor.White;

    logger.Info("Product ID {id} updated.", id);
  }
  // --------------------------------------------------------Grade C complete----------------------------------------------------


  else if (choice == "5") // Delete Product
  {
    using var db = new DataContext();

    Console.WriteLine("Enter Product ID to delete:");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Invalid Product ID.");
      Console.ForegroundColor = ConsoleColor.White;
      logger.Warn("Invalid Product ID entered for delete.");
      continue;
    }

    Product? product = db.Products
        .Include(p => p.OrderDetails)
        .FirstOrDefault(p => p.ProductId == id);

    if (product == null)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Product not found.");
      Console.ForegroundColor = ConsoleColor.White;
      logger.Warn("Product ID {id} not found for delete.", id);
      continue;
    }

    if (product.OrderDetails.Any())
    {
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine("This product cannot be deleted because it is connected to existing order details.");
      Console.WriteLine("To avoid orphaned records, mark it as discontinued instead? y/n");
      Console.ForegroundColor = ConsoleColor.White;

      string? discontinueChoice = Console.ReadLine();

      if (discontinueChoice?.ToLower() == "y")
      {
        product.Discontinued = true;
        db.SaveChanges();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Product marked as discontinued.");
        Console.ForegroundColor = ConsoleColor.White;

        logger.Info("Product ID {id} marked as discontinued instead of deleted due to related OrderDetails.", id);
      }
      else
      {
        Console.WriteLine("Product was not deleted.");
        logger.Info("Delete canceled for Product ID {id} because related OrderDetails exist.", id);
      }

      continue;
    }

    Console.WriteLine($"Are you sure you want to delete {product.ProductName}? y/n");
    string? confirm = Console.ReadLine();

    if (confirm?.ToLower() == "y")
    {
      db.Products.Remove(product);
      db.SaveChanges();

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("Product deleted successfully.");
      Console.ForegroundColor = ConsoleColor.White;

      logger.Info("Product ID {id} deleted.", id);
    }
    else
    {
      Console.WriteLine("Delete canceled.");
      logger.Info("Delete canceled for Product ID {id}.", id);
    }
  }
  else if (choice == "6") // Display categories
  {

  }
  else if (choice == "7") // Display specific category with active products
  {

  }
  else if (choice == "8") // Display all categories with active products
  {

  }
  else if (choice == "9") // Add category
  {

  }
  else if (choice == "10")// Edit category
  {

  }
  else if (choice == "11") // Delete category
  {
    using var db = new DataContext();

    Console.WriteLine("Enter Category ID to delete:");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Invalid Category ID.");
      Console.ForegroundColor = ConsoleColor.White;
      logger.Warn("Invalid Category ID entered for delete.");
      continue;
    }

    Category? category = db.Categories
        .Include(c => c.Products)
        .FirstOrDefault(c => c.CategoryId == id);

    if (category == null)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("Category not found.");
      Console.ForegroundColor = ConsoleColor.White;
      logger.Warn("Category ID {id} not found for delete.", id);
      continue;
    }

    if (category.Products.Any())
    {
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine("This category cannot be deleted because it has related products.");
      Console.WriteLine("To avoid orphaned product records, delete or move the products first.");
      Console.ForegroundColor = ConsoleColor.White;

      logger.Info("Delete blocked for Category ID {id} because related products exist.", id);
      continue;
    }

    Console.WriteLine($"Are you sure you want to delete {category.CategoryName}? y/n");
    string? confirm = Console.ReadLine();

    if (confirm?.ToLower() == "y")
    {
      db.Categories.Remove(category);
      db.SaveChanges();

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("Category deleted successfully.");
      Console.ForegroundColor = ConsoleColor.White;

      logger.Info("Category ID {id} deleted.", id);
    }
    else
    {
      Console.WriteLine("Delete canceled.");
      logger.Info("Delete canceled for Category ID {id}.", id);
    }
  }
  else if (choice == "12") // Exceptional feature / product search report
  {

  }
  else if (String.IsNullOrEmpty(choice))
  {
    break;
  }
  Console.WriteLine();
} while (true);

logger.Info("Program ended");