using System;
using System.Collections.Generic;
using System.Linq;
using BookStore.API.DbContexts;
using BookStore.API.Entities;
using BookStore.API.ResourceParameters;

namespace BookStore.API.Services
{
    public class BookLibraryRepository : IBookLibraryRepository, IDisposable
    {
        private readonly BookLibraryContext _context;

        public BookLibraryRepository(BookLibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddAuthor(Author author)
        {
            if(author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // the repository fills the id (instead of using identity columns)
            author.Id = Guid.NewGuid();

            foreach (var course in author.Books)
            {
                course.Id = Guid.NewGuid();
            }

            _context.Authors.Add(author);
        }

        public void AddBook(Guid authorId, Book book)
        {
            if(authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if(book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }
            // always set the AuthorId to the passed-in authorId
            book.AuthorId = authorId;
            _context.Books.Add(book);
        }

        public bool AuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            if(author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _context.Authors.Remove(author);
        }

        public void DeleteBook(Book book)
        {
            if(book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            _context.Books.Remove(book);
        }

        public Author GetAuthor(Guid authorId)
        {
            if(authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.ToList<Author>();
        }

        public IEnumerable<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            if(authorsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(authorsResourceParameters));
            }

            if(String.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory)
                && String.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            {
                return GetAuthors();
            }

            var collection = _context.Authors as IQueryable<Author>;

            if (!String.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory))
            {
                var mainCategory = authorsResourceParameters.MainCategory.Trim();
                collection = collection.Where(a => a.MainCategory == mainCategory);
            }

            if (!String.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            {
                var searchQuery = authorsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.MainCategory.Contains(searchQuery)
                || a.LastName.Contains(searchQuery)
                || a.Name.Contains(searchQuery));
            }


            return collection.ToList();
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.Name)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public Book GetBook(Guid authorId, Guid bookId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if(bookId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(bookId));
            }

            return _context.Books
                .Where(b => b.Id == bookId && b.AuthorId == authorId).FirstOrDefault();
        }

        public IEnumerable<Book> GetBooks(Guid authorId)
        {
            if(authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Books
                .Where(b => b.AuthorId == authorId)
                .OrderBy(b => b.Title).ToList();
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        public void UpdateBook(Book book)
        {
            // no code in this implementation
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}
