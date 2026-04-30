using NLog;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NorthwindConsole.Model;
using System.ComponentModel.DataAnnotations;
using NorthwindConsole.Services;
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
    ProductService.DisplayProducts(logger);
  }  
  else if (choice == "2") // Display Specific Product
  {
    ProductService.DisplaySpecificProduct(logger);
  }

  else if (choice == "3") // Add Product
  {
     ProductService.AddProduct(logger);
  }

  else if (choice == "4") // Edit Product
  {
    ProductService.EditProduct(logger);
  }
  
  else if (choice == "5") // Delete Product
  {
    ProductService.DeleteProduct(logger);
  }

  else if (choice == "6") // Display categories
  {
    CategoryService.DisplayCategories(logger);
  }
  else if (choice == "7") // Display specific category w/active products
  {
    CategoryService.DisplaySpecificCategoryWithProducts(logger);
  }
  else if (choice == "8") // Display all categories with active products 
  {
    CategoryService.DisplayAllCategoriesWithProducts(logger);
  }
  else if (choice == "9") // Add category
  {
    CategoryService.AddCategory(logger);
  }    
  else if (choice == "10")// Edit category
  {
    CategoryService.EditCategory(logger);
  }
  else if (choice == "11") // Delete category
  {
    CategoryService.DeleteCategory(logger);
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