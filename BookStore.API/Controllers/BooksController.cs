using System;
using System.Collections.Generic;
using AutoMapper;
using BookStore.API.Entities;
using BookStore.API.Models;
using BookStore.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BookStore.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookLibraryRepository _bookLibraryRepository;
        private readonly IMapper _mapper;

        public BooksController(IBookLibraryRepository bookLibraryRepository, IMapper mapper)
        {
            _bookLibraryRepository = bookLibraryRepository ??
                throw new ArgumentNullException(nameof(bookLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<BookDto>> GetBooksForAuthor(Guid authorId)
        {
            if (!_bookLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksForAuthorFromRepo = _bookLibraryRepository.GetBooks(authorId);

            return Ok(_mapper.Map<IEnumerable<BookDto>>(booksForAuthorFromRepo));
        }

        [HttpGet("{bookId}", Name = "GetBookForAuthor")]
        [HttpHead]
        public ActionResult<BookDto> GetBookForAuthor(Guid authorId, Guid bookId)
        {
            if (!_bookLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _bookLibraryRepository.GetBook(authorId, bookId);

            if(bookForAuthorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BookDto>(bookForAuthorFromRepo));
        }

        [HttpPost]
        public ActionResult<BookDto> CreateBookForAuthor(Guid authorId, BookForCreationDto bookForCreationDto)
        {
            if (!_bookLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookEntity = _mapper.Map<Book>(bookForCreationDto);
            _bookLibraryRepository.AddBook(authorId, bookEntity);
            _bookLibraryRepository.Save();


            var bookToReturn = _mapper.Map<BookDto>(bookEntity);
            return CreatedAtRoute("GetBookForAuthor", new { authorId, bookId = bookToReturn.Id },
                bookToReturn);
        }

        [HttpPut("{bookId}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid bookId, BookForUpdateDto bookForUpdateDto)
        {
            if (!_bookLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = _bookLibraryRepository.GetBook(authorId, bookId);
            if(bookFromRepo == null)
            {
                var bookEntity = _mapper.Map<Book>(bookForUpdateDto);
                bookEntity.Id = bookId;
                _bookLibraryRepository.AddBook(authorId, bookEntity);
                _bookLibraryRepository.Save();


                var bookToReturn = _mapper.Map<BookDto>(bookEntity);
                return CreatedAtRoute("GetBookForAuthor", new { authorId, bookId = bookToReturn.Id },
                    bookToReturn);
            }

            _mapper.Map(bookForUpdateDto, bookFromRepo);
            _bookLibraryRepository.UpdateBook(bookFromRepo);
            _bookLibraryRepository.Save();

            return NoContent();
        }

        [HttpPatch("{bookId}")]
        public ActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid bookId,
            JsonPatchDocument<BookForUpdateDto> patchDocument)
        {
            if (!_bookLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = _bookLibraryRepository.GetBook(authorId, bookId);
            if (bookFromRepo == null)
            {
                var bookForUpdateDto = new BookForUpdateDto();
                patchDocument.ApplyTo(bookForUpdateDto, ModelState);

                if (!TryValidateModel(bookForUpdateDto))
                {
                    return ValidationProblem(ModelState);
                }

                var bookToAdd = _mapper.Map<Book>(bookForUpdateDto);
                bookToAdd.Id = bookId;

                _bookLibraryRepository.AddBook(authorId, bookToAdd);
                _bookLibraryRepository.Save();


                var bookToReturn = _mapper.Map<BookDto>(bookToAdd);
                return CreatedAtRoute("GetBookForAuthor", new { authorId, bookId = bookToReturn.Id },
                    bookToReturn);


            }

            var BookToPatch = _mapper.Map<BookForUpdateDto>(bookFromRepo);

            patchDocument.ApplyTo(BookToPatch, ModelState);

            if (!TryValidateModel(BookToPatch))
            {
                return ValidationProblem(ModelState);
            }


            _mapper.Map(BookToPatch, bookFromRepo);
            _bookLibraryRepository.UpdateBook(bookFromRepo);
            _bookLibraryRepository.Save();

            return NoContent();
        }

        [HttpDelete("{bookId}")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid bookId)
        {
            if (!_bookLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = _bookLibraryRepository.GetBook(authorId, bookId);

            if(bookFromRepo == null)
            {
                return NotFound();
            }

            _bookLibraryRepository.DeleteBook(bookFromRepo);
            _bookLibraryRepository.Save();

            return NoContent();
        }

    }
}
