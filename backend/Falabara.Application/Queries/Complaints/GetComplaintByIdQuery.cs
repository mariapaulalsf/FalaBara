using MediatR;
using Falabara.Domain.Entities;

namespace Falabara.Application.Queries.Complaint
{
    public class GetComplaintByIdQuery : IRequest<Falabara.Domain.Entities.Complaint?>
    {
        public Guid Id { get; set; }

        public GetComplaintByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetComplaintByIdQueryHandler : IRequestHandler<GetComplaintByIdQuery, Falabara.Domain.Entities.Complaint?>
    {
        private readonly IComplaintRepository _repository;

        public GetComplaintByIdQueryHandler(IComplaintRepository repository)
        {
            _repository = repository;
        }

        public async Task<Falabara.Domain.Entities.Complaint?> Handle(GetComplaintByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(request.Id);
        }
    }
}