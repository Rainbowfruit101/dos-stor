using DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mappers;
using ViewModels;
using ViewModels.Views;

namespace DocumentStorage.Controllers
{
    //CRUD - create read update delete
    [ApiController, Route("api/documents")]
    public class DocumentsController: ControllerBase
    {
        private readonly DocumentStorageContext _documentStorage;
        private readonly DocumentFileService _documentFileService;

        private readonly IMapper<Document, DocumentView> _documentMapper;

        public DocumentsController(DocumentStorageContext documentStorage, DocumentFileService documentFileService, IMapper<Document, DocumentView> documentMapper)
        {
            _documentStorage = documentStorage;
            _documentFileService = documentFileService;
            _documentMapper = documentMapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var documents = await _documentStorage.GetFullDocuments();
            return Ok(await ConvertDocumentsToDocumentViews(documents));
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

            return Ok(await _documentMapper.ToView(document));
        }

        //http://localhost:5000/api/documents
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DocumentView documentView)
        {
            var document = await _documentMapper.ToModel(documentView);

            var savedDocument = _documentStorage.Add(document).Entity;
            await _documentStorage.SaveChangesAsync();
            
            return Ok(await _documentMapper.ToView(savedDocument));
        }

        //http://localhost:5000/api/documents
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DocumentView documentView)
        {
            var document = await _documentMapper.ToModel(documentView);
            var updatedDocument = _documentStorage.Update(document).Entity;
            await _documentStorage.SaveChangesAsync();

            return Ok(await _documentMapper.ToView(updatedDocument));
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

            return Ok(await _documentMapper.ToView(deletedDocument));
        }

        private async Task<List<DocumentView>> ConvertDocumentsToDocumentViews(List<Document> documents)
        {
            var result = new List<DocumentView>();

            foreach (var document in documents)
            {
                result.Add(await _documentMapper.ToView(document));
            }

            return result;
        }
    }
}
