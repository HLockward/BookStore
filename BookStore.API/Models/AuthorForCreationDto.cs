using System;
using System.Collections.Generic;

namespace BookStore.API.Models
{
    public class AuthorForCreationDto
    {
        public String Name { get; set; }

        public String LastName { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public String MainCategory { get; set; }

        public ICollection<BookForCreationDto> books { get; set; }
            = new List<BookForCreationDto>();
    }
}
