using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class GitHubService
{
    private readonly HttpClient _httpClient;

    public GitHubService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "WorkPulseAPI");
        
        var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            throw new InvalidOperationException("El token de GitHub no está configurado.");
        }
    }

    public async Task<T> GetAsync<T>(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        // Verificamos si el contenido es nulo o vacío antes de deserializar
        if (string.IsNullOrEmpty(content))
        {
            throw new InvalidOperationException("The response content is empty.");
        }

        // Deserializamos y verificamos si el resultado es nulo
        var result = JsonSerializer.Deserialize<T>(content);

        if (result == null)
        {
            throw new InvalidOperationException("Failed to deserialize the response content.");
        }

        return result;
    }

    public async Task<List<PullRequest>> GetPullRequestsAsync(string owner, string repo)
    {
        // Cambiamos dynamic por List<PullRequest>
        return await GetAsync<List<PullRequest>>($"https://api.github.com/repos/{owner}/{repo}/pulls?state=closed");
    }

    public async Task<List<Commit>> GetCommitsAsync(string owner, string repo)
        {
            return await GetAsync<List<Commit>>($"https://api.github.com/repos/{owner}/{repo}/commits");
        }

    public async Task<List<Issue>> GetIssuesAsync(string owner, string repo)
    {
        return await GetAsync<List<Issue>>($"https://api.github.com/repos/{owner}/{repo}/issues");
    }

}
