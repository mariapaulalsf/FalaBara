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
        private readonly INotificationRepository _notificationRepository; 

        public UpdateComplaintStatusHandler(IComplaintRepository repository, INotificationRepository notificationRepository)
        {
            _repository = repository;
            _notificationRepository = notificationRepository;
        }

        public async Task<bool> Handle(UpdateComplaintStatusCommand request, CancellationToken cancellationToken)
        {
            var complaint = await _repository.GetByIdAsync(request.ComplaintId);
            if (complaint == null) throw new Exception("Reclamação não encontrada.");

            // 1. Atualiza o Status
            complaint.UpdateStatus(request.NewStatus, request.OfficialResponse);
            await _repository.UpdateAsync(complaint);

            var message = $"O status da sua reclamação '{complaint.Title}' mudou para: {request.NewStatus}.";
            var notification = new Notification(complaint.UserId, message);
            
            await _notificationRepository.AddAsync(notification);

            return true;
        }
    }
}