﻿using FhirArtifactAnalyzer.Domain.Models;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    public interface IJsonArtifactAnalyzer
    {
        /// <summary>
        /// Lê o arquivo fornecido e retorna uma série de informações sobre o arquivo,
        /// contendo o tipo de recurso FHIR relevante, se houver.
        /// </summary>
        Task<FhirArtifactInfo> AnalyzeAsync(string path, InputSource source);
    }
}