using System;
using System.ComponentModel.DataAnnotations;
using BookStore.API.ValidationAttributes;

namespace BookStore.API.Models
{
    [BookTitleMustBeDifferentFromDescriptionAttribute(
        ErrorMessage ="Title must be different from description")]
    public abstract class BookForManipulationDto
    {
        [Required]
        [MaxLength(100)]
        public String Title { get; set; }

        
        [MaxLength(1500)]
        public virtual String Description { get; set; }
    }
}
