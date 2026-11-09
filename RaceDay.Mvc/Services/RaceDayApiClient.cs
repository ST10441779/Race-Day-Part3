using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Mvc.Services;

public class RaceDayApiClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _accessor;

    public RaceDayApiClient(
        HttpClient http,
        IHttpContextAccessor accessor)
    {
        _http = http;
        _accessor = accessor;
    }

    private HttpContext Context =>
        _accessor.HttpContext
        ?? throw new InvalidOperationException(
            "HTTP context is unavailable.");

    private async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request)
    {
        var cookie = Context.Session.GetString("ApiCookie");

        if (!string.IsNullOrWhiteSpace(cookie))
        {
            request.Headers.TryAddWithoutValidation(
                "Cookie", cookie);
        }

        var response = await _http.SendAsync(request);

        // Preserve the API's session cookie after login.
        if (response.Headers.TryGetValues(
            "Set-Cookie", out var cookies))
        {
            var sessionCookie = cookies.FirstOrDefault(
                x => x.StartsWith(
                    ".RaceDay.Session=",
                    StringComparison.OrdinalIgnoreCase));

            if (sessionCookie != null)
            {
                var cookiePair = sessionCookie.Split(';')[0];

                Context.Session.SetString(
                    "ApiCookie", cookiePair);
            }
        }

        return response;
    }

    private static async Task CheckResponse(
        HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var message =
                await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(message)
                    ? "The API request could not be completed."
                    : message);
        }
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        using var request =
            new HttpRequestMessage(HttpMethod.Get, endpoint);

        using var response = await SendAsync(request);

        await CheckResponse(response);

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint, TRequest data)
    {
        using var request =
            new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = JsonContent.Create(data)
            };

        using var response = await SendAsync(request);

        await CheckResponse(response);

        if (response.Content.Headers.ContentLength == 0)
            return default;

        return await response.Content
            .ReadFromJsonAsync<TResponse>();
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(
        string endpoint, TRequest data)
    {
        using var request =
            new HttpRequestMessage(HttpMethod.Put, endpoint)
            {
                Content = JsonContent.Create(data)
            };

        using var response = await SendAsync(request);

        await CheckResponse(response);

        return await response.Content
            .ReadFromJsonAsync<TResponse>();
    }

    public async Task DeleteAsync(string endpoint)
    {
        using var request =
            new HttpRequestMessage(HttpMethod.Delete, endpoint);

        using var response = await SendAsync(request);

        await CheckResponse(response);
    }
}