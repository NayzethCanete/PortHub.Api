using PortHub.Api.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortHub.Api.Interfaces
{
    public interface IBoardingService
    {
        Task<BoardingRegistrationResponse> RegisterBoardingAsync(BoardingRegistrationRequest request);
        Task<IEnumerable<ResponseBoardingDto>> GetAllBoardingsAsync();
    }
}
