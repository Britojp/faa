﻿using FhirArtifactAnalyzer.Domain.Constants;

namespace FhirArtifactAnalyzer.Domain.Models
{
    public class FhirArtifactInfo
    {
        public string FilePath { get; set; }
        public bool IsJson => IsJsonFile(FilePath);
        public bool IsWellFormedJson { get; set; }
        public bool IsRelevantFhirResource { get; set; }
        public string? ResourceType { get; set; }
        public string? ReasonIgnored { get; set; }

        private static bool IsJsonFile(string filePath)
        {
            return Path.GetExtension(filePath).Equals(FileExtensions.Json, StringComparison.OrdinalIgnoreCase);
        }
    }
}