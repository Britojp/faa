using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FhirArtifactAnalyzer.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace FhirArtifactAnalyzer.Services
{
    /// <summary>
    /// Service to interact with the FHIR Artifact Analyzer backend API
    /// </summary>
    public class FhirApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Endpoint configuration
        private const string BaseApiUrl = "http://localhost:8000/api";
        private const string UploadUrl = "/artifacts/upload";
        private const string DirectoryUrl = "/artifacts/scan-directory";
        private const string UrlUrl = "/artifacts/from-url";
        private const string ResourcesUrl = "/resources";
        private const string ValidationUrl = "/validation";
        private const string StatisticsUrl = "/statistics";

        public FhirApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
            // Configure base address if not already set
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(BaseApiUrl);
            }
        }

        /// <summary>
        /// Upload a file to the API for processing
        /// </summary>
        public async Task<bool> UploadFileAsync(IBrowserFile file)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream(maxAllowedSize: 100 * 1024 * 1024);
                using var ms = new MemoryStream();
                
                await fileStream.CopyToAsync(ms);
                ms.Position = 0;
                
                var fileContent = new ByteArrayContent(ms.ToArray());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                
                content.Add(fileContent, "file", file.Name);
                
                var response = await _httpClient.PostAsync($"{BaseApiUrl}{UploadUrl}", content);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error uploading file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Scan a directory for FHIR artifacts
        /// </summary>
        public async Task<bool> ScanDirectoryAsync(string path, int maxDepth = 5, string pattern = "")
        {
            try
            {
                var requestData = new
                {
                    Path = path,
                    MaxDepth = maxDepth,
                    Pattern = pattern
                };
                
                var response = await _httpClient.PostAsJsonAsync($"{BaseApiUrl}{DirectoryUrl}", requestData);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error scanning directory: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Fetch resources from a URL
        /// </summary>
        public async Task<bool> FetchFromUrlAsync(string url)
        {
            try
            {
                var requestData = new { Url = url };
                var response = await _httpClient.PostAsJsonAsync($"{BaseApiUrl}{UrlUrl}", requestData);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching from URL: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get all resources
        /// </summary>
        public async Task<List<FhirResource>> GetResourcesAsync()
        {
            try
            {
                var resources = await _httpClient.GetFromJsonAsync<List<FhirResource>>($"{BaseApiUrl}{ResourcesUrl}", _jsonOptions);
                return resources ?? new List<FhirResource>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error getting resources: {ex.Message}");
                return new List<FhirResource>();
            }
        }

        /// <summary>
        /// Get a specific resource by ID
        /// </summary>
        public async Task<FhirResource> GetResourceAsync(string id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<FhirResource>($"{BaseApiUrl}{ResourcesUrl}/{id}", _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error getting resource: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Validate a specific resource
        /// </summary>
        public async Task<ValidationResult> ValidateResourceAsync(string id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ValidationResult>($"{BaseApiUrl}{ValidationUrl}/{id}", _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error validating resource: {ex.Message}");
                return new ValidationResult { IsValid = false };
            }
        }

        /// <summary>
        /// Get analysis statistics
        /// </summary>
        public async Task<Statistics> GetStatisticsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Statistics>($"{BaseApiUrl}{StatisticsUrl}", _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error getting statistics: {ex.Message}");
                return new Statistics();
            }
        }
        
        /// <summary>
        /// Export data in specific format
        /// </summary>
        public async Task<string> ExportDataAsync(string format)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseApiUrl}/export?format={format}");
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error exporting data: {ex.Message}");
                return null;
            }
        }
    }
}
