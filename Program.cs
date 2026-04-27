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
    var db = new DataContext();
    var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
    foreach (var item in query)
    {
      Console.WriteLine($"{item.CategoryName}");
      foreach (Product p in item.Products)
      {
        Console.WriteLine($"\t{p.ProductName}");
      }
    }
  }
  else if (choice == "5") // Delete Product
  {

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