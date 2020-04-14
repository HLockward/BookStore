using System;
namespace BookStore.API.Models
{
    public class AuthorForCreationDto
    {
        public String Name { get; set; }

        public String LastName { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public String MainCategory { get; set; }
    }
}
