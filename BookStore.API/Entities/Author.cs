using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Entities
{
    public class Author
    {
        [Key]
        public Guid Id { get; set; }

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

        public ICollection<Book> Books { get; set; }
            = new List<Book>();
    }
}
