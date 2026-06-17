using MediatR;
using Falabara.Domain.Entities;

namespace Falabara.Application.Queries.User
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public Guid Id { get; set; }

        public GetUserByIdQuery(Guid id)
        {
            Id = id;
        }
    }
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public UserType Type { get; set; }
        public string TypeName => Type.ToString(); 
        public string? Department { get; set; }
        public string? FoneNumber { get; set; }
        public bool Active { get; set; }
    }
}