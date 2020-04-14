using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore.API.Entities;
using BookStore.API.Helpers;
using BookStore.API.Models;
using BookStore.API.ResourceParameters;
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
        private readonly IMapper _mapper;

        public AuthorsController(IBookLibraryRepository bookLibraryRepository, IMapper mapper)
        {
            _bookLibraryRepository = bookLibraryRepository ??
                throw new ArgumentNullException(nameof(bookLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors(
            [FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {
            var authorsFromRep = _bookLibraryRepository.GetAuthors(authorsResourceParameters);


            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRep));
        }

        [HttpGet("{authorId}", Name ="GetAuthor")]
        [HttpHead]
        public ActionResult<AuthorDto> GetAuthor(Guid authorId)
        {
            var author = _bookLibraryRepository.GetAuthor(authorId);
            if(author == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorDto>(author));
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto authorForCreationDto)
        {
            var authorEntity = _mapper.Map<Author>(authorForCreationDto);
            _bookLibraryRepository.AddAuthor(authorEntity);
            _bookLibraryRepository.Save();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("GetAuthor", new { authorId = authorToReturn.Id },
                authorToReturn);


        }

        [HttpOptions]
        public IActionResult GetAuthorsOption()
        {
            Response.Headers.Add("Allow", "GET, POST, HEAD, OPTIONS");
            return Ok();
        }
    }
}