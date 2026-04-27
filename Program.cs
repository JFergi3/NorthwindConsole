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
  Console.WriteLine("1) Display categories");
  Console.WriteLine("2) Add category");
  Console.WriteLine("3) Display Category and related products");
  Console.WriteLine("4) Display all Categories and their related products");
  Console.WriteLine("Enter to quit");
  string? choice = Console.ReadLine();
  Console.Clear();
  logger.Info("Option {choice} selected", choice);

  if (choice == "1")
  {
    // display categories
    
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
  else if (choice == "2")
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
  else if (choice == "3")
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
  else if (choice == "4")
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
  else if (String.IsNullOrEmpty(choice))
  {
    break;
  }
  Console.WriteLine();
} while (true);

logger.Info("Program ended");