using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.API.Entities
{
    public class Book
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public String Title { get; set; }

        [Required]
        [MaxLength(1500)]
        public String Description { get; set; }

        [ForeignKey("AuthorId")]
        public Author Author { get; set; }

        [Required]
        public Guid AuthorId { get; set; }

    }
}