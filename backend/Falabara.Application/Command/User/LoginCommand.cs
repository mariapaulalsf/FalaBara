using MediatR;
using Falabara.Domain.Entities;

namespace Falabara.Application.Commands.User
{
    public class LoginCommand : IRequest<Falabara.Domain.Entities.User>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Falabara.Domain.Entities.User>
    {
        private readonly IUserRepository _userRepository;

        public LoginCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Falabara.Domain.Entities.User> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                return null; 

            if (!user.ValidatePassword(request.Password))
                return null; 

            return user;
        }
    }
}