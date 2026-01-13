using MediatR;
using Falabara.Domain.Entities;
using Falabara.Infrastructure.Context; 

namespace Falabara.Application.Commands.Feedback
{
    public class CreateFeedbackCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public FeedbackType Type { get; set; }
        public int Rating { get; set; }
    }

    public class CreateFeedbackHandler : IRequestHandler<CreateFeedbackCommand, bool>
    {
        private readonly FalabaraContext _context;

        public CreateFeedbackHandler(FalabaraContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
        {
            var feedback = new Falabara.Domain.Entities.Feedback(
                request.UserId, 
                request.Message, 
                request.Type, 
                request.Rating
            );

            _context.Add(feedback); 
            await _context.SaveChangesAsync();
            return true;
        }
    }
}