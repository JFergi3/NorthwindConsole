using NorthwindConsole.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using NLog;

namespace NorthwindConsole.Services;

public static class CategoryService
{
    public static void DisplayCategories(Logger logger) //Choice 6
    {
        using var db = new DataContext();

        var categories = db.Categories
            .OrderBy(c => c.CategoryName)
            .ToList();

        ConsoleHelper.WriteSuccess($"{categories.Count} records returned");

        foreach (var category in categories)
        {
            Console.WriteLine($"{category.CategoryName} - {category.Description}");
        }

        logger.Info("Displayed all categories.");
    }
    public static void DisplaySpecificCategoryWithProducts(Logger logger) //Choice 7
    {
        using var db = new DataContext();

        Console.WriteLine("Select the category whose active products you want to display:");

        var categories = db.Categories
            .OrderBy(c => c.CategoryId)
            .ToList();

        foreach (var category in categories)
        {
            Console.WriteLine($"{category.CategoryId}) {category.CategoryName}");
        }

        Console.WriteLine("Enter Category ID:");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            ConsoleHelper.WriteError("Invalid Category ID.");
            logger.Warn("Invalid Category ID entered for specific category display.");
            return;
        }

        Category? selectedCategory = db.Categories
            .Include(c => c.Products.Where(p => !p.Discontinued))
            .FirstOrDefault(c => c.CategoryId == id);

        if (selectedCategory == null)
        {
            ConsoleHelper.WriteError("Category not found.");
            logger.Warn("Category ID {id} not found for specific category display.", id);
            return;
        }

        Console.WriteLine(selectedCategory.CategoryName);

        if (!selectedCategory.Products.Any())
        {
            Console.WriteLine("\tNo active products found.");
        }
        else
        {
            foreach (Product product in selectedCategory.Products.OrderBy(p => p.ProductName))
            {
                Console.WriteLine($"\t{product.ProductName}");
            }
        }

        logger.Info("Displayed Category ID {id} with active products.", id);
    }
    public static void DisplayAllCategoriesWithProducts(Logger logger) //Choice 8
    {
        using var db = new DataContext();

        var categories = db.Categories
            .Include(c => c.Products.Where(p => !p.Discontinued))
            .OrderBy(c => c.CategoryName)
            .ToList();

        foreach (var category in categories)
        {
            Console.WriteLine(category.CategoryName);

            if (!category.Products.Any())
            {
                Console.WriteLine("\tNo active products.");
            }
            else
            {
                foreach (Product product in category.Products.OrderBy(p => p.ProductName))
                {
                    Console.WriteLine($"\t{product.ProductName}");
                }
            }
        }

        logger.Info("Displayed all categories with active products.");
    }
    public static void AddCategory(Logger logger) //Choice 9
    {
        using var db = new DataContext();

        Category category = new();

        Console.WriteLine("Enter Category Name:");
        category.CategoryName = Console.ReadLine()!;

        Console.WriteLine("Enter Category Description:");
        category.Description = Console.ReadLine();

        ValidationContext context = new(category, null, null);
        List<ValidationResult> results = new();

        bool isValid = Validator.TryValidateObject(category, context, results, true);

        if (isValid)
        {
            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
            {
                isValid = false;
                results.Add(new ValidationResult("Category name already exists.", new[] { "CategoryName" }));
            }
            else
            {
                db.Categories.Add(category);
                db.SaveChanges();

                ConsoleHelper.WriteSuccess("Category added successfully.");
                logger.Info("Category added: {name}", category.CategoryName);
            }
        }

        if (!isValid)
        {
            foreach (var result in results)
            {
                ConsoleHelper.WriteError($"{result.MemberNames.First()} : {result.ErrorMessage}");
                logger.Error("{member} : {message}", result.MemberNames.First(), result.ErrorMessage);
            }
        }
    }
    public static void EditCategory(Logger logger) //Choice 10
    {
        using var db = new DataContext();

        Console.WriteLine("Enter Category ID to edit:");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            ConsoleHelper.WriteError("Invalid Category ID.");
            logger.Warn("Invalid Category ID entered for edit.");
            return;
        }

        Category? category = db.Categories.FirstOrDefault(c => c.CategoryId == id);

        if (category == null)
        {
            ConsoleHelper.WriteError("Category not found.");
            logger.Warn("Category ID {id} not found for edit.", id);
            return;
        }

        Console.WriteLine($"Editing: {category.CategoryName}");
        Console.WriteLine("Press Enter to keep the current value.");

        Console.WriteLine($"Category Name ({category.CategoryName}):");
        string? categoryName = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            bool nameExists = db.Categories.Any(c =>
                c.CategoryName == categoryName &&
                c.CategoryId != category.CategoryId);

            if (nameExists)
            {
                ConsoleHelper.WriteError("That category name already exists.");
                logger.Warn("Duplicate category name attempted while editing Category ID {id}.", id);
                return;
            }

            category.CategoryName = categoryName;
        }

        Console.WriteLine($"Description ({category.Description}):");
        string? description = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(description))
        {
            category.Description = description;
        }

        db.SaveChanges();

        ConsoleHelper.WriteSuccess("Category updated successfully.");
        logger.Info("Category ID {id} updated.", id);
    }
    public static void DeleteCategory(Logger logger)
    {
        using var db = new DataContext();

        Console.WriteLine("Enter Category ID to delete:");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            ConsoleHelper.WriteError("Invalid Category ID.");
            logger.Warn("Invalid Category ID entered for delete.");
            return;
        }

        Category? category = db.Categories
            .Include(c => c.Products)
            .FirstOrDefault(c => c.CategoryId == id);

        if (category == null)
        {
            ConsoleHelper.WriteError("Category not found.");
            logger.Warn("Category ID {id} not found for delete.", id);
            return;
        }

        if (category.Products.Any())
        {
            ConsoleHelper.WriteWarning("This category cannot be deleted because it has related products.");
            Console.WriteLine("To avoid orphaned product records, delete or move the products first.");

            logger.Info("Delete blocked for Category ID {id} because related products exist.", id);
            return;
        }

        Console.WriteLine($"Are you sure you want to delete {category.CategoryName}? y/n");
        string? confirm = Console.ReadLine();

        if (confirm?.ToLower() == "y")
        {
            db.Categories.Remove(category);
            db.SaveChanges();

            ConsoleHelper.WriteSuccess("Category deleted successfully.");
            logger.Info("Category ID {id} deleted.", id);
        }
        else
        {
            Console.WriteLine("Delete canceled.");
            logger.Info("Delete canceled for Category ID {id}.", id);
        }
    }
}