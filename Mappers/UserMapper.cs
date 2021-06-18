using System.Threading.Tasks;
using DbContexts;
using Models;
using ViewModels.Views;

namespace Mappers
{
    public class UserMapper: IMapper<User, UserView>
    {
        private readonly DocumentStorageContext _dbContext;
        private readonly IMapper<Role, RoleView> _roleMapper;

        public UserMapper(DocumentStorageContext dbContext, IMapper<Role, RoleView> roleMapper)
        {
            _dbContext = dbContext;
            _roleMapper = roleMapper;
        }

        public async Task<UserView> ToView(User model)
        {
            return new UserView()
            {
                Id = model.Id,
                Username = model.Username,
                Role = await _roleMapper.ToView(model.Role)
            };
        }

        public async Task<User> ToModel(UserView view)
        {
            var existingUser = await _dbContext.GetFullUser(view.Id);
            var existingRole = await _dbContext.GetFullRole(view.Role.Id);

            return new User()
            {
                Id = view.Id,
                Username = view.Username,
                Password = existingUser?.Password,
                Role = existingRole
            };
        }
    }
}