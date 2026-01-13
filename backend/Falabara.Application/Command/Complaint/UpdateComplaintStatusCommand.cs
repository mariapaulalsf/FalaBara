using MediatR;
using Falabara.Domain.Entities;

namespace Falabara.Application.Commands.Complaint
{
    public class UpdateComplaintStatusCommand : IRequest<bool>
    {
        public Guid ComplaintId { get; set; }
        public Guid UserId { get; set; } 
        public ComplaintStatus NewStatus { get; set; }
        public string? OfficialResponse { get; set; }
    }

    public class UpdateComplaintStatusHandler : IRequestHandler<UpdateComplaintStatusCommand, bool>
    {
        private readonly IComplaintRepository _repository;

        public UpdateComplaintStatusHandler(IComplaintRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateComplaintStatusCommand request, CancellationToken cancellationToken)
        {
            var complaint = await _repository.GetByIdAsync(request.ComplaintId);

            if (complaint == null)
                throw new Exception("Reclamação não encontrada.");

            complaint.UpdateStatus(request.NewStatus, request.OfficialResponse);

            await _repository.UpdateAsync(complaint);
            return true;
        }
    }
}