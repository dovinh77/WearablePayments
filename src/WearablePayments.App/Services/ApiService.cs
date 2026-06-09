namespace WearablePayments.App.Services;

using System.Net.Http.Headers;
using System.Net.Http.Json;

public class ApiService
{
    private readonly HttpClient _http;
    private string? _token;

    public ApiService(HttpClient http) => _http = http;

    public void SetToken(string token)
    {
        _token = token;
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public Task<T?> GetAsync<T>(string path) => _http.GetFromJsonAsync<T>(path);

    public async Task<T?> PostAsync<T>(string path, object body)
    {
        var response = await _http.PostAsJsonAsync(path, body);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task DeleteAsync(string path)
    {
        var response = await _http.DeleteAsync(path);
        response.EnsureSuccessStatusCode();
    }
}
