using Application.Common.Abstraction;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Features.Admin.Roles;

public static class GetAllRoles
{
    public sealed record GetAllRolesRes(int Id, string Name);

    public sealed record Query : IRequest<Result<List<GetAllRolesRes>>>;

    public sealed class Handler(
        IApplicationDbContext context,
         ILogger<Handler> logger,
        ICacheService cacheService)
        : IRequestHandler<Query, Result<List<GetAllRolesRes>>>
    {
        public async Task<Result<List<GetAllRolesRes>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var totalTimer = Stopwatch.StartNew();

            const string cacheKey = "roles:all";

            // 1. Measure Redis GET
            var cacheTimer = Stopwatch.StartNew();

            var cachedRoles =
                await cacheService.GetAsync<List<GetAllRolesRes>>(
                    cacheKey,
                    cancellationToken);

            cacheTimer.Stop();

            logger.LogInformation(
                "Redis GET took {ElapsedMs} ms",
                cacheTimer.Elapsed.TotalMilliseconds);

            if (cachedRoles is not null)
            {
                totalTimer.Stop();

                logger.LogInformation(
                    "CACHE HIT | Roles: {Count} | Total: {ElapsedMs} ms",
                    cachedRoles.Count,
                    totalTimer.Elapsed.TotalMilliseconds);

                return Result<List<GetAllRolesRes>>
                    .Success(cachedRoles);
            }

            logger.LogInformation("CACHE MISS | Querying database");

            // 2. Measure SQL Server query
            var dbTimer = Stopwatch.StartNew();

            var roles = await context.Roles
                .AsNoTracking()
                .OrderBy(r => r.Id)
                .Select(r => new GetAllRolesRes(
                    r.Id,
                    r.Name!))
                .ToListAsync(cancellationToken);

            dbTimer.Stop();

            logger.LogInformation(
                "DATABASE QUERY took {ElapsedMs} ms",
                dbTimer.Elapsed.TotalMilliseconds);

            // 3. Measure Redis SET
            var setTimer = Stopwatch.StartNew();

            await cacheService.SetAsync(cacheKey, roles, TimeSpan.FromMinutes(10), cancellationToken);

            setTimer.Stop();

            logger.LogInformation("Redis SET took {ElapsedMs} ms",
                setTimer.Elapsed.TotalMilliseconds);

            // 4. Total time
            totalTimer.Stop();

            logger.LogInformation(
                "CACHE MISS | Roles: {Count} | Total: {ElapsedMs} ms", roles.Count,
                totalTimer.Elapsed.TotalMilliseconds);

            return Result<List<GetAllRolesRes>>.Success(roles);
        }
    }
}