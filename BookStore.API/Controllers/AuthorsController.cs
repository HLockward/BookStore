using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertycheckerService _propertycheckerService;

        public AuthorsController(IBookLibraryRepository bookLibraryRepository, IMapper mapper,
            IPropertyMappingService propertyMappingService,
            IPropertycheckerService propertycheckerService)
        {
            _bookLibraryRepository = bookLibraryRepository ??
                throw new ArgumentNullException(nameof(bookLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
            _propertycheckerService = propertycheckerService ??
                throw new ArgumentNullException(nameof(propertycheckerService));
        }

        [HttpGet(Name ="GetAuthors")]
        [HttpHead]
        public IActionResult GetAuthors(
            [FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistingFor<AuthorDto, Author>
                (authorsResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_propertycheckerService.TypeHasProperties<AuthorDto>
                (authorsResourceParameters.Fields))
            {
                return BadRequest();
            }

            var authorsFromRep = _bookLibraryRepository.
                GetAuthors(authorsResourceParameters);

           

            var paginationMetadata = new
            {
                totalCount = authorsFromRep.TotalCount,
                pageSize = authorsFromRep.PageSize,
                currentPage = authorsFromRep.CurrentPage,
                totalPages = authorsFromRep.TotalPages

            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForAuthors(authorsResourceParameters,
                authorsFromRep.HasNext, authorsFromRep.HasPrevious);

            var shapedAuthors = _mapper.Map<IEnumerable<AuthorDto>>(authorsFromRep)
                .ShapeData(authorsResourceParameters.Fields);

            var shapedAuthorsWithLinks = shapedAuthors.Select(author =>
            {
                var authorAsDictionary = author as IDictionary<string, object>;
                var authorLink = CreateLinksForAuthor((Guid)authorAsDictionary["Id"], null);
                authorAsDictionary.Add("links", authorLink);
                return authorAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedAuthorsWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [HttpGet("{authorId}", Name ="GetAuthor")]
        [HttpHead]
        public IActionResult GetAuthor(Guid authorId, string fields)
        {
            if (!_propertycheckerService.TypeHasProperties<AuthorDto>(fields))
            {
                return BadRequest();
            }

            var author = _bookLibraryRepository.GetAuthor(authorId);
            if(author == null)
            {
                return NotFound();
            }

            var links = CreateLinksForAuthor(authorId, fields);

            var linkedResourceToReturn =
                _mapper.Map<AuthorDto>(author).ShapeData(fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        [HttpPost(Name = "CreateAuthor")]
        public IActionResult CreateAuthor(AuthorForCreationDto authorForCreationDto)
        {
            var authorEntity = _mapper.Map<Author>(authorForCreationDto);
            _bookLibraryRepository.AddAuthor(authorEntity);
            _bookLibraryRepository.Save();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

            var links = CreateLinksForAuthor(authorToReturn.Id, null);

            var linkedResourceToReturn = _mapper.Map<AuthorDto>(authorEntity)
                .ShapeData(null) as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetAuthor", new { authorId = linkedResourceToReturn["Id"]},
                linkedResourceToReturn);


        }

        [HttpOptions]
        public IActionResult GetAuthorsOption()
        {
            Response.Headers.Add("Allow", "GET, POST, HEAD, OPTIONS");
            return Ok();
        }

        [HttpDelete("{authorId}", Name = "DeleteAuthor")]
        public IActionResult DeleteAuthor(Guid authorId)
        {
            var authorFromRepo = _bookLibraryRepository.GetAuthor(authorId);

            if(authorFromRepo == null)
            {
                return NotFound();
            }

            _bookLibraryRepository.DeleteAuthor(authorFromRepo);
            _bookLibraryRepository.Save();

            return NoContent();
        }

        private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters,
            ResourceUriType resouceUriType)
        {
            switch (resouceUriType)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            pageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuery = authorsResourceParameters.SearchQuery
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber + 1,
                            pageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuery = authorsResourceParameters.SearchQuery
                        });

                case ResourceUriType.Current:
                default:
                    return Url.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber + 1,
                            pageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuery = authorsResourceParameters.SearchQuery
                        });
            }
            
        }

        private IEnumerable<LinkDto> CreateLinksForAuthor(Guid authorId, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(Url.Link("GetAuthor", new { authorId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(new LinkDto(Url.Link("GetAuthors", new { authorId, fields }),
                    "self",
                    "GET"));
            }

            links.Add(new LinkDto(Url.Link("DeleteAuthor", new { authorId }),
                "delete_author",
                "DELETE"));

            links.Add(new LinkDto(Url.Link("CreateBookForAuthor", new { authorId }),
                "create_book_for_author",
                "POST"));

            links.Add(new LinkDto(Url.Link("GetBooksForAuthor", new { authorId }),
               "books",
               "GET"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForAuthors(
            AuthorsResourceParameters authorsResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateAuthorsResourceUri(
                authorsResourceParameters, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNext)
            {
                links.Add(new LinkDto(CreateAuthorsResourceUri(
                authorsResourceParameters, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateAuthorsResourceUri(
                authorsResourceParameters, ResourceUriType.PreviousPage),
                "previous_page",
                "GET"));
            }

            return links;
        }
    }
}