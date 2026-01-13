using MediatR;
using Falabara.Domain.Entities;

namespace Falabara.Application.Commands.Complaint
{
    public class DeleteComplaintCommand : IRequest<bool>
    {
        public Guid ComplaintId { get; set; }
        public Guid UserId { get; set; } 
        public bool IsAdmin { get; set; } 
    }

    public class DeleteComplaintCommandHandler : IRequestHandler<DeleteComplaintCommand, bool>
    {
        private readonly IComplaintRepository _repository;

        public DeleteComplaintCommandHandler(IComplaintRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteComplaintCommand request, CancellationToken cancellationToken)
        {
            var complaint = await _repository.GetByIdAsync(request.ComplaintId);

            if (complaint == null)
                throw new Exception("Reclamação não encontrada.");

            if (complaint.UserId != request.UserId && !request.IsAdmin)
            {
                throw new Exception("Você não tem permissão para deletar este post.");
            }

            await _repository.DeleteAsync(complaint);
            return true;
        }
    }
}