using NLog;

namespace NorthwindConsole.Services;

public static class MenuService
{
    public static void DisplayMenu()
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
    }

    public static bool HandleMenuChoice(string? choice, Logger logger)
    {
        Console.Clear();
        logger.Info("Option {choice} selected", choice);

        if (choice == "1")
        {
            ProductService.DisplayProducts(logger);
        }
        else if (choice == "2")
        {
            ProductService.DisplaySpecificProduct(logger);
        }
        else if (choice == "3")
        {
            ProductService.AddProduct(logger);
        }
        else if (choice == "4")
        {
            ProductService.EditProduct(logger);
        }
        else if (choice == "5")
        {
            ProductService.DeleteProduct(logger);
        }
        else if (choice == "6")
        {
            CategoryService.DisplayCategories(logger);
        }
        else if (choice == "7")
        {
            CategoryService.DisplaySpecificCategoryWithProducts(logger);
        }
        else if (choice == "8")
        {
            CategoryService.DisplayAllCategoriesWithProducts(logger);
        }
        else if (choice == "9")
        {
            CategoryService.AddCategory(logger);
        }
        else if (choice == "10")
        {
            CategoryService.EditCategory(logger);
        }
        else if (choice == "11")
        {
            CategoryService.DeleteCategory(logger);
        }
        else if (choice == "12")
        {
            SearchReport.Run(logger);
        }
        else if (string.IsNullOrWhiteSpace(choice))
        {
            return false;
        }
        else
        {
            ConsoleHelper.WriteError("Invalid option. Please choose a menu number.");
            logger.Warn("Invalid menu option entered: {choice}", choice);
        }

        Console.WriteLine();
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
        Console.Clear();

        return true;
    }
}