﻿using System;
using System.Collections.Generic;
using AutoMapper;
using BookStore.API.Entities;
using BookStore.API.Models;
using BookStore.API.Services;
using Microsoft.AspNetCore.Mvc;

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
            return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, bookId = bookToReturn.Id },
                bookToReturn);
        } 
    }
}
