using MediatR;
using Falabara.Application.Services; 
using Falabara.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Falabara.Domain.Entities;

namespace Falabara.Application.Commands.Complaint
{
    public class CreateComplaintCommand : IRequest<bool>
    {
        public Guid UserId { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Location { get; set; }
        public string Neighborhood { get; set; }
        public ComplaintCategory Category { get; set; }
        public IFormFile? Image { get; set; }
    }

   public class CreateComplaintCommandHandler : IRequestHandler<CreateComplaintCommand, bool>
{
    private readonly IComplaintRepository _repository;
    private readonly IFileService _fileService; 

    public CreateComplaintCommandHandler(IComplaintRepository repository, IFileService fileService)
    {
        _repository = repository;
        _fileService = fileService;
    }

    public async Task<bool> Handle(CreateComplaintCommand request, CancellationToken cancellationToken)
    {
        string? imageUrl = null;
        if (request.Image != null)
        {
            imageUrl = await _fileService.SaveFileAsync(request.Image);
        }

        var complaint = new Falabara.Domain.Entities.Complaint(
            request.Title,
            request.Description,
            request.Location,      
            request.Neighborhood,   
            request.Latitude,       
            request.Longitude,     
            imageUrl,              
            request.Category,
            request.UserId
        );

        await _repository.AddAsync(complaint);
        return true;
    }
}
}