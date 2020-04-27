using System;
using System.Collections.Generic;
using BookStore.API.Entities;
using BookStore.API.Helpers;
using BookStore.API.ResourceParameters;

namespace BookStore.API.Services
{
    public interface IBookLibraryRepository
    {
        IEnumerable<Book> GetBooks(Guid authorId);
        Book GetBook(Guid authorId, Guid bookId);
        void AddBook(Guid authorId, Book book);
        void UpdateBook(Book book);
        void DeleteBook(Book book);
        IEnumerable<Author> GetAuthors();
        public PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters);
        Author GetAuthor(Guid authorId);
        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);
        void AddAuthor(Author author);
        void DeleteAuthor(Author author);
        void UpdateAuthor(Author author);
        bool AuthorExists(Guid authorId);
        bool Save();
    }
}
