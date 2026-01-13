using MediatR;
using Falabara.Domain.Entities;

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

        public ToggleVoteHandler(IVoteRepository voteRepository)
        {
            _voteRepository = voteRepository;
        }

        public async Task<string> Handle(ToggleVoteCommand request, CancellationToken cancellationToken)
        {
            //Verifica se já votou antes
            var existingVote = await _voteRepository.GetByUserAndComplaintAsync(request.UserId, request.ComplaintId);

            if (existingVote == null)
            {
                // Nunca votou entao Cria Novo
                var newVote = new Falabara.Domain.Entities.Vote(request.ComplaintId, request.UserId, request.IsLike);
                await _voteRepository.AddAsync(newVote);
                return request.IsLike ? "Você apoiou esta reclamação!" : "Você discordou desta reclamação.";
            }
            else
            {
                // Já votou. fez o mesmo voto?
                if (existingVote.IsLike == request.IsLike)
                {
                    // Clicou Like e já tinha Like entao Remove 
                    await _voteRepository.RemoveAsync(existingVote);
                    return "Voto removido.";
                }
                else
                {
                    // Clicou Like mas tinha Dislike entao Troca
                    existingVote.UpdateVote(request.IsLike);
                    await _voteRepository.UpdateAsync(existingVote);
                    return request.IsLike ? "Alterado para Apoio." : "Alterado para Discordo.";
                }
            }
        }
    }
}