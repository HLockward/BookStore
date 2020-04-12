using System;
namespace BookStore.API.Models
{
    public class BookDto
    {
        public Guid Id { get; set; }

        public String Title { get; set; }

        public String Description { get; set; }

        public Guid AuthorId { get; set; }
    }
}
