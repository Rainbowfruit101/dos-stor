using DbContexts;
using Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModels.Views;

namespace DocumentStorage.Controllers
{
    [ApiController, Route("api/tags")]
    public class TagsController: ControllerBase
    {
        private readonly DocumentStorageContext _dbContext;

        private readonly IMapper<Tag, TagView> _tagMapper;

        public TagsController(DocumentStorageContext dbContext, IMapper<Tag, TagView> tagMapper)
        {
            _dbContext = dbContext;
            _tagMapper = tagMapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tags = await _dbContext.GetFullTags();
            
            return Ok(await ConvertTagsToTagViews(tags));
        }

        //http://localhost:5000/api/tags/89809-ajsd-aksd43
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tag = await _dbContext.GetFullTag(id);
            if (tag == null)
            {
                return NotFound();
            }

            return Ok(await _tagMapper.ToView(tag));
        }

        //http://localhost:5000/api/tags
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TagView tagView)
        {
            var tag = await _tagMapper.ToModel(tagView);
            
            var savedTag = _dbContext.Add(tag).Entity;
            await _dbContext.SaveChangesAsync();

            return Ok(await _tagMapper.ToView(savedTag));
        }

        //http://localhost:5000/api/tags
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TagView tagView)
        {
            if (tagView.Id == Guid.Empty)
            {
                return BadRequest("Не задан ID редактируемого тега");
            }

            var existingTag = await _dbContext.GetFullTag(tagView.Id);
            
            if (existingTag == null)
            {
                return NotFound();
            }

            existingTag.Name = tagView.Name;
            await _dbContext.SaveChangesAsync();

            return Ok(await _tagMapper.ToView(existingTag));
        }

        //http://localhost:5000/api/tags/89809-ajsd-aksd43
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedTag = await _dbContext.GetFullTag(id);
            if (deletedTag == null)
            {
                return NotFound();
            }

            _dbContext.Remove(deletedTag);
            await _dbContext.SaveChangesAsync();

            return Ok(await _tagMapper.ToView(deletedTag));
        }
      
        private async Task<List<TagView>> ConvertTagsToTagViews(List<Tag> tags)
        {
            var result = new List<TagView>();

            foreach (var tag in tags)
            {
                result.Add(await _tagMapper.ToView(tag));
            }

            return result;
        }
    }
}
