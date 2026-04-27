using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NorthwindConsole.Model;

public partial class Category
{
  [StringLength(50)]
  public int CategoryId { get; set; }
  [Required(ErrorMessage = "Category Name is required.")]
  public string CategoryName { get; set; } = null!;

  public string? Description { get; set; }

  public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}