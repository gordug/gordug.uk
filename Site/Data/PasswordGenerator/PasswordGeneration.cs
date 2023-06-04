using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using gordug.uk.Models;

namespace gordug.uk.Data.PasswordGenerator;

internal class PasswordGeneration : IPasswordGeneration
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PasswordGeneration> _logger;

    public PasswordGeneration(IHttpClientFactory httpClientFactory, ILogger<PasswordGeneration> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("PasswordGeneration");
    }

    public async Task<string> GeneratePasswordAsync(PasswordOptions options)
    {
        var jsonOptions = MapOptions(options);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("/api/GeneratePassword", UriKind.Relative),
            Content = new StringContent(JsonSerializer.Serialize(jsonOptions), Encoding.UTF8, "application/json")
        };
        _logger.LogTrace("Sending request to password generation service.\n {Request}",
            request.Content.ReadAsStringAsync().Result);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private static JsonOptions MapOptions(PasswordOptions options)
    {
        return new JsonOptions(options);
    }

    private class JsonOptions
    {
        public enum PasswordType
        {
            Special,
            Numbers,
            Lowercase,
            Uppercase
        }

        private static readonly Requirement Required = new() { Required = true };
        private static readonly Requirement NotRequired = new() { Required = false };

        // ReSharper disable once UnusedMember.Local
        // Required for Json Deserialization
        public JsonOptions()
        {
            Length = 10;
            PasswordTypes = new List<Dictionary<PasswordType, Requirement>>();
        }

        public JsonOptions(PasswordOptions options)
        {
            Length = options.Length;
            var jsonOptions = new List<Dictionary<PasswordType, Requirement>>();
            if (options.IncludeNumbers)
                jsonOptions.Add(new Dictionary<PasswordType, Requirement>
                    { { PasswordType.Numbers, options.RequireNumbers ? Required : NotRequired } });
            if (options.IncludeSpecialCharacters)
                jsonOptions.Add(new Dictionary<PasswordType, Requirement>
                    { { PasswordType.Special, options.RequireSpecialCharacters ? Required : NotRequired } });
            if (options.IncludeLowercase)
                jsonOptions.Add(new Dictionary<PasswordType, Requirement>
                    { { PasswordType.Lowercase, options.RequireLowercase ? Required : NotRequired } });
            if (options.IncludeUppercase)
                jsonOptions.Add(new Dictionary<PasswordType, Requirement>
                    { { PasswordType.Uppercase, options.RequireUppercase ? Required : NotRequired } });
            PasswordTypes = jsonOptions;
            NoAmbiguous = options.ExcludeAmbiguousCharacters;
            NoSimilar = options.ExcludeSimilarCharacters;
            NoSequential = options.ExcludeSequentialCharacters;
        }

        [JsonPropertyName("length")] public int Length { get; set; }

        [JsonPropertyName("password_type")]
        public List<Dictionary<PasswordType, Requirement>> PasswordTypes { get; set; }

        [JsonPropertyName("no_ambiguous")] public bool? NoAmbiguous { get; set; }
        [JsonPropertyName("no_similar")] public bool? NoSimilar { get; set; }
        [JsonPropertyName("no_sequential")] public bool? NoSequential { get; set; }

        public class Requirement
        {
            [JsonPropertyName("required")] public bool? Required { get; init; }
        }
    }
}