using System;
using AutoMapper;

namespace BookStore.API.Profiles
{
    public class BooksProfile : Profile
    {
        public BooksProfile()
        {
            CreateMap<Entities.Book, Models.BookDto>();
        }
    }
}
