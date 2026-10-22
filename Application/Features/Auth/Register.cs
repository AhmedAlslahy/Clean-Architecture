using Application.Common.Abstraction;
using Application.Common.Errors;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Entities.Identity;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth;

public static class Register
{
    public sealed record Command(
        string FullName,
        string UserName,
        string Email,
        string Password,
        string ConfirmPassword
    ) : IRequest<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FullName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Full name is required.")
                .Must(x => !string.IsNullOrWhiteSpace(x))
                    .WithMessage("Full name cannot contain only spaces.")
                .MaximumLength(100)
                    .WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Username is required.")
                .MinimumLength(3)
                    .WithMessage("Username must be at least 3 characters.")
                .MaximumLength(20)
                    .WithMessage("Username must not exceed 20 characters.")
                .Matches(@"^[a-zA-Z0-9_]+$")
                    .WithMessage(
                        "Username can contain only letters, numbers, and underscores."
                    );

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Email is required.")
                .MaximumLength(254)
                    .WithMessage("Email must not exceed 254 characters.")
                .EmailAddress()
                    .WithMessage("Please enter a valid email address.");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Password is required.")
                .MinimumLength(8)
                    .WithMessage("Password must be at least 8 characters.")
                .MaximumLength(128)
                    .WithMessage("Password must not exceed 128 characters.")
                .Matches("[A-Z]")
                    .WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]")
                    .WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]")
                    .WithMessage("Password must contain at least one digit.")
                .Matches(@"[^a-zA-Z0-9]")
                    .WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.ConfirmPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Password confirmation is required.")
                .Equal(x => x.Password)
                    .WithMessage("Password confirmation does not match.");
        }
    }

    public sealed class Handler(IApplicationDbContext context, IPasswordService passwordService)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command req, CancellationToken cancellationToken)
        {
            var existingUser = await context.Users.FindByEmailAsync(req.Email, cancellationToken);
            if (existingUser != null)
                return UserErrors.AlreadyExists;

            var checkUser = await context.Users.FindByUserNameAsync(req.UserName, cancellationToken);
            if (checkUser != null)
                return UserErrors.UserNameAlreadyExists;

            var user = new Domain.Entities.Identity.User
            {
                Email = req.Email,
                FullName = req.FullName,
                UserName = req.UserName,
                PasswordHashed = passwordService.HashPassword(req.Password),
                EmailConfirmed = false,
                UserSetting = new UserSetting()
            };
            var role = await context.Roles.FirstOrDefaultAsync(
                   r => r.NormalizedName == "USER", cancellationToken);

            if (role is null)
                return RoleErrors.NotFound;

            user.UserRoles.Add(new UserRole
            {
                Role = role,
                User = user
            });

            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}