using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using ViewModels.Views;

namespace Mappers
{
    public class DocumentMapper: IMapper<Document, DocumentView>
    {
        private readonly IMapper<Tag, TagView> _tagMapper;
        private readonly IMapper<Role, RoleView> _roleMapper;

        public DocumentMapper(IMapper<Tag, TagView> tagMapper, IMapper<Role, RoleView> roleMapper)
        {
            _tagMapper = tagMapper;
            _roleMapper = roleMapper;
        }

        public async Task<DocumentView> ToView(Document model)
        {
            var tagViews = new List<TagView>();
            foreach (var tag in model.Tags)
            {
                tagViews.Add(await _tagMapper.ToView(tag));
            }
            
            var roleViews = new List<RoleView>();
            foreach (var role in model.OwnRoles)
            {
                roleViews.Add(await _roleMapper.ToView(role));
            }
            
            return new DocumentView()
            {
                Id = model.Id,
                Name = model.Name,
                CreationTime = model.CreationTime,
                Tags = tagViews,
                OwnRoles = roleViews
            };
        }

        public async Task<Document> ToModel(DocumentView view)
        {
            var tags = new List<Tag>();
            if (view.Tags != null)
            {
                foreach (var tagView in view.Tags)
                {
                    tags.Add(await _tagMapper.ToModel(tagView));
                }
            }

            var roles = new List<Role>();
            if (view.OwnRoles != null)
            {
                foreach (var roleView in view.OwnRoles)
                {
                    roles.Add(await _roleMapper.ToModel(roleView));
                }
            }

            return new Document()
            {
                Id = view.Id,
                Name = view.Name,
                CreationTime = view.CreationTime,
                Tags = tags,
                OwnRoles = roles
            };
        }
    }
}