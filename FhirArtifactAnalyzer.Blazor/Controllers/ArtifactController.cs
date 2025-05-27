using Microsoft.AspNetCore.Mvc;
using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Domain.Abstractions;

namespace FhirArtifactAnalyzer.Blazor.Controllers
{
    /// <summary>
    /// Responsavel pela entrada Web, integrando com Blazor (ainda nao implementado).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ArtifactController : ControllerBase
    {
        private readonly IArtifactProcessor _processor;

        public ArtifactController(IArtifactProcessor processor, IInputIdentifier identifier)
        {
            _processor = processor;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze([FromBody] InputSource input)
        {
            var result = await _processor.ProcessInputAsync(input);
            return Ok(result);
        }
    }

}
