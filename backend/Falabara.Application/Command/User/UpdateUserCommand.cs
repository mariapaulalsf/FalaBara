using MediatR;
using System;
using Falabara.Domain.Entities;

namespace Falabara.Application.Commands.User
{
    // O comando carrega TUDO que PODE ser alterado
    public class UpdateUserCommand : IRequest<bool>
    {
        public Guid Id { get; set; } 
        public string Name { get; set; }
        public string? Department { get; set; }
        public string? FoneNumber { get; set; }
        public string? NewEmail { get; set; }
        public string? NewPassword { get; set; }
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

            // Atualiza dados comuns 
            user.Update(request.Name, request.Department, request.FoneNumber);

            // Atualiza Email 
            if (!string.IsNullOrEmpty(request.NewEmail))
            {
                // Verifica se o email já existe em outra pessoa
                var userWithEmail = await _userRepository.GetByEmailAsync(request.NewEmail);
                if (userWithEmail != null && userWithEmail.Id != user.Id)
                    throw new Exception("Este email já está em uso por outro usuário.");

                user.SetEmail(request.NewEmail);
            }

            // Atualiza Senha (se foi enviada)
            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                user.SetPassword(request.NewPassword);
            }

            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}