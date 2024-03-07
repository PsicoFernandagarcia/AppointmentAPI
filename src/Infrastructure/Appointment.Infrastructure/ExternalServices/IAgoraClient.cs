using Appointment.Domain.Entities;
using Appointment.Infrastructure.ExternalServices.Dtos;
using Refit;
using System.Threading.Tasks;

namespace Appointment.Infrastructure.ExternalServices
{

    public interface IAgoraClient
    {
        [Post("/getToken")]
        Task<ApiResponse<GetTokenModelResponse>> GetToken([Body] GetTokenModel getTokenModel);
    }
}
