using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IBookLibraryRepository _bookLibraryRepository;

        public AuthorsController(IBookLibraryRepository bookLibraryRepository)
        {
            _bookLibraryRepository = bookLibraryRepository ??
                throw new ArgumentNullException(nameof(bookLibraryRepository));
        }

        [HttpGet()]
        public IActionResult GetAuthors()
        {
            var authorsFromRep = _bookLibraryRepository.GetAuthors();
            return Ok(authorsFromRep);
        }

        [HttpGet("{authorId}")]
        public IActionResult GetAuthor(Guid authorId)
        {
            var author = _bookLibraryRepository.GetAuthor(authorId);
            if(author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }
    }
}