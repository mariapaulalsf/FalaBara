using MediatR;
using Falabara.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

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

            complaint.UpdateStatus(request.NewStatus, request.OfficialResponse);
            await _repository.UpdateAsync(complaint);

            string statusName = request.NewStatus switch {
                ComplaintStatus.EmAnalise => "Em Análise",
                ComplaintStatus.EmAndamento => "Em Andamento",
                ComplaintStatus.Resolvido => "Resolvido",
                ComplaintStatus.Cancelado => "Cancelado",
                _ => "Aberto"
            };

            string message;
            if (!string.IsNullOrEmpty(request.OfficialResponse))
            {
                message = $"A Prefeitura respondeu sua reclamação '{complaint.Title}': \"{request.OfficialResponse}\"";
            }
            else
            {
                message = $"O status da sua reclamação '{complaint.Title}' mudou para: {statusName}.";
            }

            var notification = new Notification(complaint.UserId, message);
            await _notificationRepository.AddAsync(notification);

            return true;
        }
    }
}