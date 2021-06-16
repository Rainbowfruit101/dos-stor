using DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModels;

namespace DocumentStorage.Controllers
{
    //CRUD - create read update delete
    [ApiController, Route("api/documents")]
    public class DocumentsController: ControllerBase
    {
        private readonly DocumentStorageContext _documentStorage;
        private readonly DocumentSearchService _documentSearchService;
        private readonly DocumentFileService _documentFileService;

        public DocumentsController(DocumentStorageContext documentStorage, DocumentSearchService documentSearchService, DocumentFileService documentFileService)
        {
            _documentStorage = documentStorage;
            _documentSearchService = documentSearchService;
            _documentFileService = documentFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok(await _documentStorage.Documents.ToListAsync());
        }

        //http://localhost:5000/api/documents/89809-ajsd-aksd43
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var document = await _documentStorage.GetFullDocument(id);
            if(document == null)
            {
                return NotFound();
            }

            //TODO: Add View <-> Model mappers сейчас можешь наскринить то что сделал .ща покажу запросы в посмане
            return Ok(new
            {
                id = document.Id,
                name = document.Name,
                creationTime = document.CreationTime,
                tags = document.Tags.Select(t => new TagView() { Name = t.Name })
            });
        }

        //http://localhost:5000/api/documents
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DocumentView documentView)
        {
            var document = new Document
            {
                Id = Guid.NewGuid(),
                Name = documentView.Name,
                CreationTime = DateTime.Now,
                Tags = await ConvertTags(documentView.Tags)
            };

            var savedDocument = _documentStorage.Add(document).Entity;
            await _documentStorage.SaveChangesAsync();
            
            //TODO: Add View <-> Model mappers
            return Ok(new { 
                id = savedDocument.Id, 
                name = savedDocument.Name, 
                creationTime = savedDocument.CreationTime, 
                tags = savedDocument.Tags.Select(t => new TagView() { Name = t.Name })
            });
        }

        private async Task<List<Tag>> ConvertTags(List<TagView> tagViews)
        {
            var tags = new List<Tag>();
            foreach (var tagView in tagViews)
            {
                var tag = await _documentStorage.FindTagByName(tagView.Name);
                if(tag == null)
                {
                    tags.Add(new Tag()
                    {
                        Id = Guid.NewGuid(),
                        Name = tagView.Name
                    });
                }
                else
                {
                    tags.Add(tag);
                }
            }

            return tags;
        }

        //http://localhost:5000/api/documents
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Document document)
        {
            var updatedDocument = _documentStorage.Update(document).Entity;
            await _documentStorage.SaveChangesAsync();

            return Ok(updatedDocument);
        }

        //http://localhost:5000/api/documents/89809-ajsd-aksd43
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedDocument = await _documentStorage.GetFullDocument(id);
            if (deletedDocument == null)
            {
                return NotFound();
            }
            _documentFileService.Delete(deletedDocument);

            _documentStorage.Remove(deletedDocument);
            await _documentStorage.SaveChangesAsync();

            return Ok(deletedDocument);
        }

        /*
        вынести не забудь
        //TODO: Refactor to search controller (api/documents/search)
        //http://localhost:5000/api/documents/search?query=docname
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            return Ok(await _documentSearchService.SearchDocumentByName(query));
        }

        //http://localhost:5000/api/documents/search/by_date
        [HttpPost("by_date")]
        public async Task<IActionResult> SearchByDate([FromBody] DateSearchViewModel searchModel)
        {
            return Ok(await _documentSearchService.SearchDocumentByDate(searchModel));
        }

        //http://localhost:5000/api/documents/search/by_tags
        [HttpPost("by_tags")]
        public async Task<IActionResult> SearchByTags([FromBody] TagSearchViewModel searchModel)
        {
            if(searchModel.Tags.Count <= 0)
            {
                return BadRequest(new { ErrorMessage = "Нет запрашиваемых тегов" });
            }

            return Ok(await _documentSearchService.SearchDocumentsByTags(searchModel));
        }
        */
    }
}
