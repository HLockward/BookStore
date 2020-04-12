using System;
namespace BookStore.API.Models
{
    public class AuthorDto
    {
        public Guid Id { get; set; }

        public String Name { get; set; }

        public int Age { get; set; }

        public String MainCategory { get; set; }
    }
}
