using System.Threading.Tasks;
using DbContexts;
using Models;
using ViewModels.Views;

namespace Mappers
{
    public class RoleMapper: IMapper<Role, RoleView>
    {
        private readonly DocumentStorageContext _dbContext;

        public RoleMapper(DocumentStorageContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RoleView> ToView(Role model)
        {
            return new RoleView()
            {
                Id = model.Id,
                Name = model.Name
            };
        }

        public async Task<Role> ToModel(RoleView view)
        {
            var existingRole = await _dbContext.GetFullRole(view.Id);
            return new Role()
            {
                Id = view.Id,
                Name = view.Name,
                AllowDocuments = existingRole.AllowDocuments,
                Users = existingRole.Users
            };
        }
    }
}