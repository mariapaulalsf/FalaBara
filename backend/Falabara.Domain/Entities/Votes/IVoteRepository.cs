using System.Threading.Tasks;

namespace Falabara.Domain.Entities
{
    public interface IVoteRepository
    {
        Task<Vote?> GetByUserAndComplaintAsync(Guid userId, Guid complaintId);
        Task AddAsync(Vote vote);
        Task RemoveAsync(Vote vote);
        Task UpdateAsync(Vote vote);
    }
}