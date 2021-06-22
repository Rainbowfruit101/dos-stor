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
using ViewModels;
using ViewModels.Views;

namespace DocumentStorage.Controllers
{
    [ApiController, Route("api/documents/search")]
    public class SearchController: ControllerBase
    {
        private readonly DocumentSearchService _documentSearchService;
        private readonly IMapper<Document, TagView> _documentMapper;
        private readonly IMapper<Tag, TagView> _tagMapper;
        private readonly DocumentStorageContext _dbContext;

        public SearchController(DocumentSearchService documentSearchService, IMapper<Document, TagView> documentMapper, IMapper<Tag, TagView> tagMapper, DocumentStorageContext dbContext)
        {
            _documentSearchService = documentSearchService;
            _documentMapper = documentMapper;
            _tagMapper = tagMapper;
            _dbContext = dbContext;
        }

        //http://localhost:5000/api/documents/search?query=docname
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var user = await GetUser();
            if (user == null)
                return Forbid();

            if (query.Length <= 0)
            {
                return BadRequest("Пустой запрос");
            }
            
            var foundDocuments = await _documentSearchService.SearchDocumentByName(query);
            var ownedDocuments = foundDocuments.Where(document => document.OwnRoles.Contains(user.Role)).ToList();
            return Ok(await ConvertDocumentsToDocumentViews(ownedDocuments));
        }

        //http://localhost:5000/api/documents/search/by_date
        [Authorize]
        [HttpPost("by_date")]
        public async Task<IActionResult> SearchByDate([FromBody] DateSearchViewModel searchModel)
        {
            var user = await GetUser();
            if (user == null)
                return Forbid();

            var foundDocuments = await _documentSearchService.SearchDocumentByDate(searchModel);
            var ownedDocuments = foundDocuments.Where(document => document.OwnRoles.Contains(user.Role)).ToList();
            return Ok(await ConvertDocumentsToDocumentViews(ownedDocuments));
        }

        //http://localhost:5000/api/documents/search/by_tags
        [Authorize]
        [HttpPost("by_tags")]
        public async Task<IActionResult> SearchByTags([FromBody] TagSearchViewModel searchModel)
        {
            var user = await GetUser();
            if (user == null)
                return Forbid();

            if (searchModel.Tags.Count <= 0)
            {
                return BadRequest(new { ErrorMessage = "Нет запрашиваемых тегов" });
            }

            var tags = new List<Tag>();
            foreach (var tagView in searchModel.Tags)
            {
                tags.Add(await _tagMapper.ToModel(tagView));
            }

            var foundDocuments = await _documentSearchService.SearchDocumentsByTags(searchModel.Mode, tags);
            var ownedDocuments = foundDocuments.Where(document => document.OwnRoles.Contains(user.Role)).ToList();
            return Ok(await ConvertDocumentsToDocumentViews(ownedDocuments));
        }

        private async Task<List<TagView>> ConvertDocumentsToDocumentViews(List<Document> documents)
        {
            var result = new List<TagView>();

            foreach (var document in documents)
            {
                result.Add(await _documentMapper.ToView(document));
            }

            return result;
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