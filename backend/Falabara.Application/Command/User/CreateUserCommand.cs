using MediatR;
using Falabara.Domain.Entities;

namespace Falabara.Application.Commands.User
{
    public class CreateUserCommand : IRequest<bool>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Cpf { get; set; }
        public UserType Type { get; set; }
        public string? Department { get; set; }
        public string? FoneNumber { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUserCpf = await _userRepository.GetByCpfAsync(request.Cpf);
            if (existingUserCpf != null)
                throw new Exception("Já existe um usuário com este CPF.");

            var existingUserEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUserEmail != null)
                throw new Exception("Já existe um usuário com este Email.");

            var newUser = new Falabara.Domain.Entities.User(
                request.Name,
                request.Email,
                request.Password,
                request.Cpf,
                request.Type,
                request.Department,
                request.FoneNumber
            );

            await _userRepository.AddAsync(newUser);

            return true;
        }
    }
}