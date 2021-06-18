using System.Collections.Generic;
using System.Threading.Tasks;
using Mappers;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using ViewModels;
using ViewModels.Views;

namespace DocumentStorage.Controllers
{
    [ApiController, Route("api/documents/search")]
    public class SearchController: ControllerBase
    {
        private readonly DocumentSearchService _documentSearchService;
        private readonly IMapper<Document, DocumentView> _documentMapper;

        public SearchController(DocumentSearchService documentSearchService, IMapper<Document, DocumentView> documentMapper)
        {
            _documentSearchService = documentSearchService;
            _documentMapper = documentMapper;
        }

        //http://localhost:5000/api/documents/search?query=docname
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (query.Length <= 0)
            {
                return BadRequest("Пустой запрос");
            }
            
            var foundDocuments = await _documentSearchService.SearchDocumentByName(query);
            return Ok(await ConvertDocumentsToDocumentViews(foundDocuments));
        }

        //http://localhost:5000/api/documents/search/by_date
        [HttpPost("by_date")]
        public async Task<IActionResult> SearchByDate([FromBody] DateSearchViewModel searchModel)
        {
            var foundDocuments = await _documentSearchService.SearchDocumentByDate(searchModel);
            return Ok(await ConvertDocumentsToDocumentViews(foundDocuments));
        }

        //http://localhost:5000/api/documents/search/by_tags
        [HttpPost("by_tags")]
        public async Task<IActionResult> SearchByTags([FromBody] TagSearchViewModel searchModel)
        {
            if(searchModel.Tags.Count <= 0)
            {
                return BadRequest(new { ErrorMessage = "Нет запрашиваемых тегов" });
            }

            var foundDocuments = await _documentSearchService.SearchDocumentsByTags(searchModel);
            return Ok(await ConvertDocumentsToDocumentViews(foundDocuments));
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