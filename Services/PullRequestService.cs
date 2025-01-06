using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class PullRequestService
{
    private readonly ApplicationDbContext _dbContext;

    public PullRequestService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SavePullRequestsAsync(List<PullRequest> pullRequests)
    {
        foreach (var pr in pullRequests)
        {
            // Verifica si el Pull Request ya está en la base de datos
            if (!_dbContext.PullRequests.Any(p => p.Id == pr.Id))
            {
                _dbContext.PullRequests.Add(pr); // Agregamos solo los nuevos
            }
        }
        await _dbContext.SaveChangesAsync(); // Guardamos los cambios en la base de datos
    }
}
