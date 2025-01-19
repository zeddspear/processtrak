using processtrak_backend.Dto;
using processtrak_backend.Models;

namespace processtrak_backend.interfaces
{
    public interface IAlgorithmService
    {
        Task<Algorithm> AddAlgorithmAsync(AddAlgorithmDTO dto);
    }
}
