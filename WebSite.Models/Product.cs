﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebSite.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [MaxLength(5000)]
        public string Description { get; set; }

        [Required]
        [Range(1, 100000)]
        public double Price {  get; set; }

        [Required]
        [Range(1, 1000)]
        public int Weight { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category  Category  { get; set; }
        [ValidateNever]
        public string? ImageURL { get; set; }
    }
}
