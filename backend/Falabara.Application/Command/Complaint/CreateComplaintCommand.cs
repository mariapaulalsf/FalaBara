using MediatR;
using Falabara.Domain.Entities;

namespace Falabara.Application.Commands.Complaint
{
    public class CreateComplaintCommand : IRequest<bool>
    {
        public Guid UserId { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Neighborhood { get; set; }
        public ComplaintCategory Category { get; set; }
    }

    public class CreateComplaintCommandHandler : IRequestHandler<CreateComplaintCommand, bool>
    {
        private readonly IComplaintRepository _repository;

        public CreateComplaintCommandHandler(IComplaintRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(CreateComplaintCommand request, CancellationToken cancellationToken)
        {
            var complaint = new Falabara.Domain.Entities.Complaint(
                request.Title,
                request.Description,
                request.Location,
                request.Neighborhood,
                request.Category,
                request.UserId
            );

            await _repository.AddAsync(complaint);
            return true;
        }
    }
}