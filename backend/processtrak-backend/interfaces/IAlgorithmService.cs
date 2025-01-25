using processtrak_backend.Dto;
using processtrak_backend.Models;

namespace processtrak_backend.interfaces
{
    public interface IAlgorithmService
    {
        Task<Algorithm> AddAlgorithmAsync(AddAlgorithmDTO dto);

        // Static methods can't be declared in an interface.
        // We can include them in a static class.
    }
}
