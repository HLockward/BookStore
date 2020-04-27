using System;
using System.ComponentModel.DataAnnotations;
using BookStore.API.Models;

namespace BookStore.API.ValidationAttributes
{
    public class BookTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            var book = (BookForManipulationDto)validationContext.ObjectInstance;

            if (book.Title == book.Description)
            {
                return new ValidationResult(ErrorMessage,
                    new[] { nameof(BookForManipulationDto) });
            }

            return ValidationResult.Success;
        }
    }
    
}
