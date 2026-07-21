namespace FrontEnd.Data;

public class TaskItemClient
{
    private HttpClient _httpClient;
    private ILogger<TaskItemClient> _logger;

    public TaskItemClient(HttpClient httpClient, ILogger<TaskItemClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TaskItem[]> GetTasksAsync()
        => await _httpClient.GetFromJsonAsync<TaskItem[]>("/GetTaks") ?? [];

    public async Task<TaskItem[]> PostTaskAsync(string text)
    {
        var Dto = new {Text = text};
        var results = await _httpClient.PostAsJsonAsync("/AddTask", Dto);
        results.EnsureSuccessStatusCode();
        return await GetTasksAsync();
    }

    public async Task<TaskItem[]> DeleteTaskAsync(int id)
    {
        var results = await _httpClient.DeleteAsync($"/RemoveTask/{id}");
        results.EnsureSuccessStatusCode();
        return await GetTasksAsync();
    }
}
