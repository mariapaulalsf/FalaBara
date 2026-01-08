using MediatR;
using System;
using Falabara.Domain.Entities;

namespace Falabara.Application.Commands.User
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public Guid Id { get; set; } 
        public string Name { get; set; }
        public string? Department { get; set; }
        public string? FoneNumber { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user == null)
                throw new Exception("Usuário não encontrado.");

            user.Update(request.Name, request.Department, request.FoneNumber);

            await _userRepository.UpdateAsync(user);

            return true;
        }
    }
}