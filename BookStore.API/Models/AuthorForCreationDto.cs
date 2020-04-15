using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Models
{
    public class AuthorForCreationDto
    {
        [Required]
        [MaxLength(50)]
        public String Name { get; set; }

        [Required]
        [MaxLength(50)]
        public String LastName { get; set; }

        [Required]
        public DateTimeOffset DateOfBirth { get; set; }

        [Required]
        [MaxLength(50)]
        public String MainCategory { get; set; }

        public ICollection<BookForCreationDto> books { get; set; }
            = new List<BookForCreationDto>();
    }
}
