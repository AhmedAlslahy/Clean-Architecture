using Domain.Common;
using Domain.Errors;
using Infrastructure.Persistence;
using Infrastructure.Service.Files;
using Infrastructure.Service.Shared;
using MediatR;
using System.Security.Cryptography;

namespace Application.Features.User.Command.Update;

public class UpdateHandler(SarhneDbContext context, ICurrentUser currentUser) : IRequestHandler<UpdateUserCommand, Result>
{
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellation)
    {
        var user = await context.Users.FindAsync(currentUser.UserId);
        if (user == null)
        {
            return UserErrors.NotFound;
        }

        user.FullName = request.FullName!;
        user.ProfileDescription = request.ProfileDescription;
        user.PhoneNumber = request.PhoneNumber;
        var uniqueNumber = RandomNumberGenerator.GetInt32(1000, 9999).ToString();
        user.PublicLink = request.PublicLink + uniqueNumber;
        if (request.Image != null)
        {
            user.ImageUrl = FileService.UploadFile("Photos", request.Image);
        }

        await context.SaveChangesAsync(cancellation);
        return Result.Success();
    }
}