using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class IssuesService
{
    private readonly ApplicationDbContext _dbContext;

    public IssuesService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveIssuesAsync(List<Issue> issues)
    {
        foreach (var issue in issues)
        {
            // Verificamos si el issue ya está en la base de datos
            if (!_dbContext.Issues.Any(i => i.Id == issue.Id))
            {
                _dbContext.Issues.Add(issue); // Agregamos solo los nuevos
            }
        }
        await _dbContext.SaveChangesAsync(); // Guardamos los cambios
    }
}
