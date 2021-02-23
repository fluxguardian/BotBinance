using Model.Enums;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface IApiClient
    {
        Task<T> CallAsync<T>(ApiMethod method, string endpoint, bool isSigned = false, string parameters = null);
    }
}
