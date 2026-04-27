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
    
    var db = new DataContext();
    var query = db.Categories.OrderBy(p => p.CategoryName);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{query.Count()} records returned");
    Console.ForegroundColor = ConsoleColor.Magenta;
    foreach (var item in query)
    {
      Console.WriteLine($"{item.CategoryName} - {item.Description}");
    }
    Console.ForegroundColor = ConsoleColor.White;
  }
  else if (choice == "2") // Display Specific Product
{
    Category category = new();

    Console.WriteLine("Enter Category Name:");
    category.CategoryName = Console.ReadLine()!;

    Console.WriteLine("Enter the Category Description:");
    category.Description = Console.ReadLine();

    ValidationContext context = new(category, null, null);
    List<ValidationResult> results = new();

    var isValid = Validator.TryValidateObject(category, context, results, true);

    using var db = new DataContext();

    if (isValid)
    {
        if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
        {
            isValid = false;
            results.Add(new ValidationResult("Category name already exists.", ["CategoryName"]));
        }
        else
        {
            db.Categories.Add(category);
            db.SaveChanges();

            logger.Info("Category added: {name}", category.CategoryName);
            Console.WriteLine("Category added successfully.");
        }
    }

    if (!isValid)
    {
        foreach (var result in results)
        {
            logger.Error("{member} : {message}", result.MemberNames.First(), result.ErrorMessage);
            Console.WriteLine($"{result.MemberNames.First()} : {result.ErrorMessage}");
        }
    }
}
  else if (choice == "3") // Add Product 
  {
    var db = new DataContext();
    var query = db.Categories.OrderBy(p => p.CategoryId);

    Console.WriteLine("Select the category whose products you want to display:");
    Console.ForegroundColor = ConsoleColor.DarkRed;
    foreach (var item in query)
    {
      Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
    }
    Console.ForegroundColor = ConsoleColor.White;
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
      Console.WriteLine("Invalid categoryID.");
      logger.Warn("Invalid category ID entered");
      continue;
    }
    Console.Clear();
    logger.Info($"CategoryId {id} selected");
    Category? category = db.Categories
        .Include(c => c.Products.Where(p => !p.Discontinued))
        .FirstOrDefault(c => c.CategoryId == id);
    if (category == null)
    {
      Console.WriteLine("Category not found.");
      logger.Warn("CategoryId {id} not found", id);
      continue;
    }

    Console.WriteLine($"{category.CategoryName} - {category.Description}");
    foreach (Product p in category.Products)
    {
      Console.WriteLine($"\t{p.ProductName}");
    }
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