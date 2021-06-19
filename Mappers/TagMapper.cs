using System.Threading.Tasks;
using DbContexts;
using Models;
using ViewModels.Views;

namespace Mappers
{
    public class TagMapper: IMapper<Tag, TagView>
    {
        private readonly DocumentStorageContext _dbContext;

        public TagMapper(DocumentStorageContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TagView> ToView(Tag model)
        {
            return new TagView()
            {
                Id = model.Id,
                Name = model.Name
            };
        }

        public async Task<Tag> ToModel(TagView view)
        {
            var existingTag = await _dbContext.FindTagByName(view.Name);
            if (existingTag != null)
                return existingTag;

            existingTag = await _dbContext.GetFullTag(view.Id);
            return new Tag()
            {
                Id = view.Id,
                Name = view.Name,
                Documents = existingTag?.Documents
            };
        }
    }
}