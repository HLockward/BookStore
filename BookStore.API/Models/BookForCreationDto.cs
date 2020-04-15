using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Models
{
    public class BookForCreationDto : IValidatableObject
    {
        [Required]
        [MaxLength(100)]
        public String Title { get; set; }

        [Required]
        [MaxLength(1500)]
        public String Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Title == Description)
            {
                yield return new ValidationResult(
                    "The provider description must be different from the title.",
                    new[] { "BookForCreationDto" });
            }
        }
    }
}
