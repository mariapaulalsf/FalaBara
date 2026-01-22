using MediatR;
using Falabara.Domain.Entities;
using System.Collections.Generic; 

namespace Falabara.Application.Commands.Vote
{
    public class ToggleVoteCommand : IRequest<string>
    {
        public Guid ComplaintId { get; set; }
        public Guid UserId { get; set; }
        public bool IsLike { get; set; }
    }

    public class ToggleVoteHandler : IRequestHandler<ToggleVoteCommand, string>
    {
        private readonly IVoteRepository _voteRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IComplaintRepository _complaintRepository;

        public ToggleVoteHandler(
            IVoteRepository voteRepository, 
            INotificationRepository notificationRepository,
            IComplaintRepository complaintRepository)
        {
            _voteRepository = voteRepository;
            _notificationRepository = notificationRepository;
            _complaintRepository = complaintRepository;
        }

        public async Task<string> Handle(ToggleVoteCommand request, CancellationToken cancellationToken)
        {
            var existingVote = await _voteRepository.GetByUserAndComplaintAsync(request.UserId, request.ComplaintId);
            string mensagemRetorno = "";
            bool isNewLike = false; 

            if (existingVote == null)
            {
                var newVote = new Falabara.Domain.Entities.Vote(request.ComplaintId, request.UserId, request.IsLike);
                await _voteRepository.AddAsync(newVote);
                mensagemRetorno = request.IsLike ? "Você apoiou esta reclamação!" : "Você discordou desta reclamação.";
                
                if (request.IsLike) isNewLike = true;
            }
            else
            {
                if (existingVote.IsLike == request.IsLike)
                {
                    await _voteRepository.RemoveAsync(existingVote);
                    mensagemRetorno = "Voto removido.";
                }
                else
                {
                    existingVote.UpdateVote(request.IsLike);
                    await _voteRepository.UpdateAsync(existingVote);
                    mensagemRetorno = request.IsLike ? "Alterado para Apoio." : "Alterado para Discordo.";
                    
                    if (request.IsLike) isNewLike = true;
                }
            }

            if (isNewLike)
            {
                int totalLikes = await _voteRepository.GetLikesCountAsync(request.ComplaintId);

                var milestones = new List<int> { 1, 5, 10, 20, 50, 100, 500, 1000 };

                if (milestones.Contains(totalLikes))
                {
                    var complaint = await _complaintRepository.GetByIdAsync(request.ComplaintId);

                    if (complaint != null && complaint.UserId != request.UserId) 
                    {
                        var message = $"Parabéns! Sua reclamação '{complaint.Title}' está ganhando força e atingiu {totalLikes} apoiadores!";
                        var notification = new Notification(complaint.UserId, message);
                        await _notificationRepository.AddAsync(notification);
                    }
                }
            }

            return mensagemRetorno;
        }
    }
}