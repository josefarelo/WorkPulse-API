using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

[ApiController]
[Route("api/github")]
public class GitHubController : ControllerBase
{
    private readonly GitHubService _gitHubService;
    private readonly PullRequestService _pullRequestService;
    private readonly CommitsService _commitsService;
    private readonly IssuesService _issuesService;

    public GitHubController(
        GitHubService gitHubService,
        PullRequestService pullRequestService,
        CommitsService commitsService,
        IssuesService issuesService)
    {
        _gitHubService = gitHubService;
        _pullRequestService = pullRequestService;
        _commitsService = commitsService;
        _issuesService = issuesService;
    }

    [HttpPost("update-pulls/{owner}/{repo}")]
    public async Task<IActionResult> UpdatePullRequests(string owner, string repo)
    {
        var pulls = await _gitHubService.GetPullRequestsAsync(owner, repo);
        await _pullRequestService.SavePullRequestsAsync(pulls); // Guardar en la base de datos
        return Ok(new { message = "Pull requests updated successfully." });
    }

    [HttpPost("update-commits/{owner}/{repo}")]
    public async Task<IActionResult> UpdateCommits(string owner, string repo)
    {
        var commits = await _gitHubService.GetCommitsAsync(owner, repo);
        await _commitsService.SaveCommitsAsync(commits); // Guardar en la base de datos
        return Ok(new { message = "Commits updated successfully." });
    }

    [HttpPost("update-issues/{owner}/{repo}")]
    public async Task<IActionResult> UpdateIssues(string owner, string repo)
    {
        var issues = await _gitHubService.GetIssuesAsync(owner, repo);
        await _issuesService.SaveIssuesAsync(issues); // Guardar en la base de datos
        return Ok(new { message = "Issues updated successfully." });
    }

    [HttpGet("commits/{owner}/{repo}")]
    public async Task<IActionResult> GetCommits(string owner, string repo)
    {
        var commits = await _gitHubService.GetCommitsAsync(owner, repo);
        return Ok(commits);
    }

    [HttpGet("pulls/{owner}/{repo}")]
    public async Task<IActionResult> GetPullRequests(string owner, string repo)
    {
        var pulls = await _gitHubService.GetPullRequestsAsync(owner, repo);

        // Filtrar PRs fusionadas usando el modelo tipado
        var mergedPRs = pulls.Where(pr => pr.MergedAt != null).ToList();

        if (!mergedPRs.Any())
        {
            return NotFound("No pull requests have been merged for this repository.");
        }

        return Ok(mergedPRs);
    }

    [HttpGet("issues/{owner}/{repo}")]
    public async Task<IActionResult> GetIssues(string owner, string repo)
    {
        var issues = await _gitHubService.GetIssuesAsync(owner, repo);
        return Ok(issues);
    }
    
    // ENDPOINT PARA PROBAR EL FUNCIONAMIENTO DEL SERVICIO
    [HttpPost("sync-commits")]
    public async Task<IActionResult> SyncCommits(string owner, string repo)
    {
        try
        {
            var commits = await _gitHubService.GetCommitsAsync(owner, repo);
            await _commitsService.SaveCommitsAsync(commits);
            return Ok("Commits synced successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error syncing commits: {ex.Message}");
            return StatusCode(500, "An error occurred while syncing commits.");
        }
    }

}
