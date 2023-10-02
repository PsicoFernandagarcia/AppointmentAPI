using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.UsersUseCase.GetUserByRole
{
    public class GetUserByRoleHandler : IRequestHandler<GetUserByRoleQuery, Result<IEnumerable<User>, ResultError>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByRoleHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<IEnumerable<User>, ResultError>> Handle(GetUserByRoleQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByRole(request.Role);
            if (user is null || !user.Any())
                return Result.Success<IEnumerable<User>, ResultError>(new List<User>());
            return Result.Success<IEnumerable<User>, ResultError>(user.OrderBy(u => u.FullName));
        }
    }
}
