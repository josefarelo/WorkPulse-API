using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CommitsService
{
    private readonly ApplicationDbContext _dbContext;

    public CommitsService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveCommitsAsync(List<Commit> commits)
    {
        Console.WriteLine($"Total commits fetched: {commits.Count}");

        foreach (var commit in commits)
        {
            Console.WriteLine($"Processing commit: Author={commit.Author}, Message={commit.Message}, Sha={commit.Sha}");

            // Verificamos si el commit ya está en la base de datos
            if (!_dbContext.Commits.Any(c => c.Sha == commit.Sha))
            {
                _dbContext.Commits.Add(commit); // Agregamos solo los nuevos
                Console.WriteLine($"Added commit: Sha={commit.Sha}");
            }
            else
            {
                Console.WriteLine($"Commit already exists: Sha={commit.Sha}");
            }
        }

        await _dbContext.SaveChangesAsync();
        Console.WriteLine("Commits saved to the database.");
    }
}
