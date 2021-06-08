using DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStorage.Controllers
{
    [ApiController, Route("api/documents/files")]
    public class DocumentFileController: ControllerBase
    {
        private readonly DocumentFileService _documentFileService;
        private readonly DocumentStorageContext _documentStorage;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public DocumentFileController(DocumentFileService documentFileService, DocumentStorageContext documentStorage, FileExtensionContentTypeProvider contentTypeProvider)
        {
            _documentFileService = documentFileService;
            _documentStorage = documentStorage;
            _contentTypeProvider = contentTypeProvider;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Upload(Guid id)
        {
            var formFile = Request.Form.Files["document"];
            if(formFile == null)
            {
                return BadRequest(new { ErrorMessage = "Не прикреплен файл документа" });
            }

            var document = await _documentStorage.GetFullDocument(id);
            if(document == null)
            {
                return NotFound(new { ErrorMessage = "Документа с указанным идентификатром не существует" });
            }

            document.Name = formFile.FileName;
            //TODO: add parsing filename and adding tags
            await _documentFileService.Save(document, formFile.OpenReadStream());

            await _documentStorage.SaveChangesAsync();

            return Ok(document);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var document = await _documentStorage.GetFullDocument(id);
            if (document == null)
            {
                return NotFound(new { ErrorMessage = "Документа с указанным идентификатром не существует" });
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
    }
}
