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

            return Ok(document);
        }

        //http://localhost:5000/api/documents
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Document document)
        {
            document.Id = Guid.NewGuid();
            document.CreationTime = DateTime.Now;

            var savedDocument = _documentStorage.Add(document).Entity;
            await _documentStorage.SaveChangesAsync();

            return Ok(savedDocument);
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
    }
}
