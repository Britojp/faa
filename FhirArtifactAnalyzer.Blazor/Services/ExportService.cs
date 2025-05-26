using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FhirArtifactAnalyzer.Models;
using Microsoft.JSInterop;

namespace FhirArtifactAnalyzer.Services
{
    /// <summary>
    /// Service to handle exporting data in various formats
    /// </summary>
    public class ExportService
    {
        private readonly IJSRuntime _jsRuntime;

        public ExportService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Export resources to JSON format and trigger download
        /// </summary>
        public async Task ExportToJsonAsync(List<FhirResource> resources, string fileName = "fhir_resources.json")
        {
            if (resources == null || !resources.Any())
                return;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(resources, options);
            await DownloadFileAsync(json, fileName, "application/json");
        }

        /// <summary>
        /// Export resources to CSV format and trigger download
        /// </summary>
        public async Task ExportToCsvAsync(List<FhirResource> resources, string fileName = "fhir_resources.csv")
        {
            if (resources == null || !resources.Any())
                return;

            var csvBuilder = new StringBuilder();
            
            // Write header
            csvBuilder.AppendLine("ResourceType,Name,CanonicalUrl,Version,ValidationStatus,References");
            
            // Write data rows
            foreach (var resource in resources)
            {
                string validationStatus = resource.ValidationResults?.IsValid == true ? "Valid" : 
                                         resource.ValidationResults?.IsValid == false ? "Invalid" : "Unknown";
                
                string references = resource.References != null ? 
                    $"\"{string.Join(", ", resource.References.Select(r => r.Reference).Distinct())}\"" : 
                    "";
                
                csvBuilder.AppendLine(
                    $"{EscapeCsvField(resource.ResourceType)}," +
                    $"{EscapeCsvField(resource.Name)}," +
                    $"{EscapeCsvField(resource.CanonicalUrl)}," +
                    $"{EscapeCsvField(resource.Version)}," +
                    $"{validationStatus}," +
                    $"{references}"
                );
            }
            
            await DownloadFileAsync(csvBuilder.ToString(), fileName, "text/csv");
        }

        /// <summary>
        /// Export graph visualization to PNG format
        /// </summary>
        public async Task ExportGraphToPngAsync(string fileName = "fhir_graph.png")
        {
            try
            {
                // Call the JavaScript function to export the graph as PNG
                var dataUrl = await _jsRuntime.InvokeAsync<string>("d3Interop.exportPng");
                
                if (!string.IsNullOrEmpty(dataUrl))
                {
                    // The data URL includes the MIME type, we need to extract the base64 part
                    var base64Data = dataUrl.Split(',')[1];
                    
                    // Trigger download
                    await _jsRuntime.InvokeVoidAsync(
                        "downloadFileFromBase64", 
                        fileName, 
                        base64Data, 
                        "image/png"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error exporting graph: {ex.Message}");
            }
        }

        /// <summary>
        /// Download a file with the given content
        /// </summary>
        private async Task DownloadFileAsync(string content, string fileName, string contentType)
        {
            try
            {
                // Create a safe file name
                var safeFileName = Path.GetFileNameWithoutExtension(fileName).Replace(" ", "_");
                var extension = Path.GetExtension(fileName);
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var finalFileName = $"{safeFileName}_{timestamp}{extension}";
                
                byte[] bytes = Encoding.UTF8.GetBytes(content);
                
                // Convert to base64
                var base64 = Convert.ToBase64String(bytes);
                
                // Use JSInterop to trigger download
                await _jsRuntime.InvokeVoidAsync(
                    "downloadFileFromBase64", 
                    finalFileName, 
                    base64, 
                    contentType
                );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error downloading file: {ex.Message}");
            }
        }

        /// <summary>
        /// Escape a field for CSV format
        /// </summary>
        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";
                
            // If the field contains comma, newline or quote, enclose it in quotes
            if (field.Contains(",") || field.Contains("\n") || field.Contains("\""))
            {
                // Replace any existing quotes with double quotes
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            
            return field;
        }
    }
}
