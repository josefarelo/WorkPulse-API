using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WorkPulse.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public MetricsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Endpoint para obtener commits por semana
        [HttpGet("commits-per-week")]
        public IActionResult GetCommitsPerWeek(string repository)
        {
            var commits = _dbContext.Commits
                .Where(c => c.Repository == repository)
                .AsEnumerable()
                .GroupBy(c => new
                {
                    Year = c.Date.Year,
                    Week = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                        c.Date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                })
                .Select(g => new
                {
                    Week = g.Key.Week,
                    Year = g.Key.Year,
                    CommitCount = g.Count()
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Week)
                .ToList();

            Console.WriteLine($"Commits retrieved for repository '{repository}': {commits.Count}");
            return Ok(commits);
        }

        // Endpoint para obtener el tiempo promedio de fusión de pull requests
        [HttpGet("avg-merge-time")]
        public IActionResult GetAverageMergeTime(string repository)
        {
            var pullRequests = _dbContext.PullRequests
                .Where(p => p.Repository == repository && p.MergedAt.HasValue) // Filtra solo los PRs con MergedAt
                .Select(p => new
                {
                    MergeTimeInHours = p.MergedAt.HasValue // Verifica si MergedAt tiene valor
                        ? EF.Functions.DateDiffHour(p.CreatedAt, p.MergedAt.Value)
                        : (int?)null // Devuelve null si MergedAt no tiene valor
                })
                .Where(p => p.MergeTimeInHours.HasValue)
                .ToList();

            Console.WriteLine($"Pull requests retrieved for repository '{repository}': {pullRequests.Count}");

            // Si no hay PRs con tiempo de fusión, devuelve 404
            if (pullRequests.Count == 0)
            {
                return NotFound("No pull requests have been merged for this repository.");
            }

            // Calcula el promedio
            var averageMergeTime = pullRequests.Average(p => p.MergeTimeInHours);

            return Ok(new { AverageMergeTime = averageMergeTime });
        }
    }
}
