using Application.Common.Abstraction;
using Application.Common.Errors;
using Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using System.Security.Cryptography;

namespace Application.Features.User.Users;

public static class UpdateUser
{
    public sealed class Command : IRequest<Result>
    {
        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileDescription { get; set; }
        public string? PublicLink { get; set; }
        public Stream? ImageStream { get; set; }
        public string? ImageFileName { get; set; }
    }

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[0-9]{10,15}$")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.ProfileDescription)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.ProfileDescription));

            RuleFor(x => x.PublicLink)
                .MaximumLength(50)
                .Matches(@"^[a-zA-Z0-9_-]+$");
        }
    }

    public sealed class Handler(
        IApplicationDbContext context,
        IFileStorageService fileStorage,
        ICurrentUserService currentUser)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var user = await context.Users.FindAsync(
                [currentUser.UserId],
                cancellationToken);

            if (user is null)
                return UserErrors.NotFound;

            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;
            user.ProfileDescription = request.ProfileDescription;

            if (!string.IsNullOrWhiteSpace(request.PublicLink))
            {
                var uniqueNumber = RandomNumberGenerator
                    .GetInt32(1000, 10000)
                    .ToString();

                user.PublicLink = request.PublicLink + uniqueNumber;
            }

            if (request.ImageStream is not null &&
                !string.IsNullOrWhiteSpace(request.ImageFileName))
            {
                var storedFileName = await fileStorage.UploadAsync(
                    request.ImageStream,
                    request.ImageFileName,
                    "Photos",
                    cancellationToken);

                user.ImageUrl = $"Photos/{storedFileName}";
            }

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}