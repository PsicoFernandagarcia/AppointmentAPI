﻿using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Security;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AuthUseCases.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<User, ResultError>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ICrypt _crypt;
        private readonly IOutputCacheStore _cachingStore;


        public CreateUserHandler(IUserRepository userRepository, IRoleRepository roleRepository, ICrypt crypt, IOutputCacheStore cachingStore)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _crypt = crypt;
            _cachingStore = cachingStore;
        }

        public async Task<Result<User, ResultError>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetUserByName(request.UserName);
            if (existingUser != null) return Result.Failure<User, ResultError>(new CreationError("User already exists"));
            if (string.IsNullOrWhiteSpace(request.Password)) return Result.Failure<User, ResultError>(new CreationError("Password empty"));

            var password = _crypt.DecryptStringFromBytes_Aes(request.Password);
            var (passwordHash, passwordSalt) = PassUtilities.CreatePasswordHash(password);
            var role = (await _roleRepository.GetRoles()).Where(x => x.Name == "COMMON");
            var userEntityResult = User.Create(0, request.UserName, request.Email, passwordHash, passwordSalt,
                role.ToList(), request.IsExternal, request.Name, request.LastName, request.TimezoneOffset
                );

            if (userEntityResult.IsFailure) return Result.Failure<User, ResultError>(userEntityResult.Error);
            var userEntity = userEntityResult.Value;
            await _userRepository.CreateUser(userEntity);
            await _cachingStore.EvictByTagAsync(CacheKeys.Users, cancellationToken);
            return userEntity;
        }
    }
}