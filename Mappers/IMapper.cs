using System.Threading.Tasks;

namespace Mappers
{
    public interface IMapper<TModel, TView>
    {
        Task<TView> ToView(TModel model);
        Task<TModel> ToModel(TView view);
    }
}