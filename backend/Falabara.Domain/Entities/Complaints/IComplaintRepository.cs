using System.Collections.Generic;
using System.Threading.Tasks;

namespace Falabara.Domain.Entities
{
    public interface IComplaintRepository
    {
        Task AddAsync(Complaint complaint);
        Task<Complaint?> GetByIdAsync(Guid id);
        Task<List<Complaint>> GetAllAsync();
        Task UpdateAsync(Complaint complaint);

        Task DeleteAsync(Complaint complaint);
    }
}