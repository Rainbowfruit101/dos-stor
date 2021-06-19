using System;
using System.IO;
using System.Threading.Tasks;
using DbContexts;
using Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Models;
using Services;
using ViewModels.Views;

namespace DocumentStorage.Controllers
{
    [ApiController, Route("api/documents/files")]
    public class DocumentFileController: ControllerBase
    {
        private readonly DocumentFileService _documentFileService;
        private readonly DocumentStorageContext _dbContext;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        private readonly IMapper<Document, DocumentView> _documentMapper;

        public DocumentFileController(DocumentFileService documentFileService, DocumentStorageContext dbContext, FileExtensionContentTypeProvider contentTypeProvider, IMapper<Document, DocumentView> documentMapper)
        {
            _documentFileService = documentFileService;
            _dbContext = dbContext;
            _contentTypeProvider = contentTypeProvider;
            _documentMapper = documentMapper;
        }

        [Authorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> Upload(Guid id)
        {
            var user = await GetUser();
            if (user == null)
                return Forbid();
            
            var formFile = Request.Form.Files["document"];
            if(formFile == null)
            {
                return BadRequest(new { ErrorMessage = "Не прикреплен файл документа" });
            }

            var document = await _dbContext.GetFullDocument(id);
            if(document == null)
            {
                return NotFound(new { ErrorMessage = "Документа с указанным ID не существует" });
            }

            document.Name = formFile.FileName;
            //TODO: add parsing filename and adding tags
            await _documentFileService.Save(document, formFile.OpenReadStream());

            await _dbContext.SaveChangesAsync();

            return Ok(await _documentMapper.ToView(document));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var user = await GetUser();
            if (user == null)
                return Forbid();

            var document = await _dbContext.GetFullDocument(id);
            if (document == null)
            {
                return NotFound(new { ErrorMessage = "Документа с указанным ID не существует" });
            }

            var fileStream = _documentFileService.Get(document);

            return File(fileStream, GetContentType(document.Name), document.Name);
        }

        private string GetContentType(string filename)
        {
            var extension = Path.GetExtension(filename);

            var contentTypeResult = "application/octet-stream";

            if (_contentTypeProvider.TryGetContentType(extension, out string contentType))
            {
                contentTypeResult = contentType;
            }

            return contentTypeResult;
        }
        
        private async Task<User> GetUser()
        {
            var userIdRaw = User.Identity?.Name;
            if (userIdRaw == null)
                return null;

            var userId = Guid.Parse(userIdRaw);
            return await _dbContext.GetFullUser(userId);
        }
    }
}
