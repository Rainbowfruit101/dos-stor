using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbContexts;
using Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using ViewModels.Views;

namespace DocumentStorage.Controllers
{
    //CRUD - create read update delete
    [ApiController, Route("api/documents")]
    public class DocumentsController: ControllerBase
    {
        private readonly DocumentStorageContext _dbContext;
        private readonly DocumentFileService _documentFileService;
        
        private readonly IMapper<Document, DocumentView> _documentMapper;

        public DocumentsController(DocumentStorageContext dbContext, DocumentFileService documentFileService, IMapper<Document, DocumentView> documentMapper)
        {
            _dbContext = dbContext;
            _documentFileService = documentFileService;
            _documentMapper = documentMapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await GetUser();
            if (user == null)
                return Forbid();
            
            var documents = await _dbContext.GetFullDocuments();
            var ownedDocuments = documents.Where(document => document.OwnRoles.Contains(user.Role)).ToList();
            return Ok(await ConvertDocumentsToDocumentViews(ownedDocuments));
        }

        //http://localhost:5000/api/documents/89809-ajsd-aksd43
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await GetUser();
            if (user == null)
                return Forbid();
            
            var document = await _dbContext.GetFullDocument(id);
            if(document == null)
            {
                return NotFound();
            }

            if (!document.OwnRoles.Contains(user.Role))
                return Forbid();
            
            return Ok(await _documentMapper.ToView(document));
        }

        //http://localhost:5000/api/documents
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DocumentView documentView)
        {
            var user = await GetUser();
            if (user == null)
                return Forbid();

            var document = await _documentMapper.ToModel(documentView);
            
            document.CreationTime = DateTime.UtcNow;
            document.OwnRoles.Add(user.Role);
            
            var savedDocument = _dbContext.Add(document).Entity;
            await _dbContext.SaveChangesAsync();
            
            return Ok(await _documentMapper.ToView(savedDocument));
        }

        //http://localhost:5000/api/documents
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DocumentView documentView)
        {
            var user = await GetUser();
            if (user == null)
                return Forbid();

            if (documentView.Id == Guid.Empty)
            {
                return BadRequest("Не задан ID редактируемого документа");
            }
            
            var existingDocument = await _dbContext.GetFullDocument(documentView.Id);

            if (existingDocument == null)
            {
                return NotFound();
            }
            
            if (!existingDocument.OwnRoles.Contains(user.Role))
                return Forbid();

            var document = await _documentMapper.ToModel(documentView);
            existingDocument.Name = document.Name; 
            var removedTags = existingDocument.Tags.Where(tag => !document.Tags.Contains(tag));
            var newTags = document.Tags.Where(tag => !existingDocument.Tags.Contains(tag));
            
            existingDocument.Tags.RemoveAll(tag => removedTags.Contains(tag));
            existingDocument.Tags.AddRange(newTags);
            
            await _dbContext.SaveChangesAsync();

            return Ok(await _documentMapper.ToView(existingDocument));
        }

        //http://localhost:5000/api/documents/89809-ajsd-aksd43
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await GetUser();
            if (user == null)
                return Forbid();
            
            var deletedDocument = await _dbContext.GetFullDocument(id);
            if (deletedDocument == null)
            {
                return NotFound();
            }

            if (!deletedDocument.OwnRoles.Contains(user.Role))
                return Forbid();
            
            _documentFileService.Delete(deletedDocument);

            _dbContext.Remove(deletedDocument);
            await _dbContext.SaveChangesAsync();

            return Ok(await _documentMapper.ToView(deletedDocument));
        }

        private async Task<User> GetUser()
        {
            var userIdRaw = User.Identity?.Name;
            if (userIdRaw == null)
                return null;

            var userId = Guid.Parse(userIdRaw);
            return await _dbContext.GetFullUser(userId);
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
