using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BookStore.API.Entities;
using BookStore.API.Helpers;
using BookStore.API.Models;
using BookStore.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [Route("api/authorCollections")]
    [ApiController]
    public class AuthorCollectionsController : ControllerBase
    {
        private readonly IBookLibraryRepository _bookLibraryRepository;
        private IMapper _mapper;

        public AuthorCollectionsController(IBookLibraryRepository bookLibraryRepository, IMapper mapper)
        {
            _bookLibraryRepository = bookLibraryRepository ??
                throw new ArgumentNullException(nameof(bookLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("({ids})",Name = "GetAuthorCollection")]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthorCollection(
            [FromRoute]
            [ModelBinder(binderType: typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if(ids == null)
            {
                return BadRequest();
            }

            var authorEntities = _bookLibraryRepository.GetAuthors(ids);

            if (authorEntities.Count() != ids.Count())
            {
                return NotFound();
            }

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            return Ok(authorsToReturn);

        }

        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthorCollection(
            IEnumerable<AuthorForCreationDto> authorCollection)
        {
            var authorEntity = _mapper.Map<IEnumerable<Author>>(authorCollection);
            foreach(var author in authorEntity)
            {
                _bookLibraryRepository.AddAuthor(author);
            }

            _bookLibraryRepository.Save();

            var authorCollectionToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntity);
            var idsAsString = string.Join(",", authorCollectionToReturn.Select(x => x.Id));

            return CreatedAtRoute("GetAuthorCollection", new {ids = idsAsString },
                authorCollectionToReturn);
        }
    }
}
