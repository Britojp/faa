using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirArtifactAnalyzer.Domain.Models
{
    public enum InputType
    {
        SingleFile,
        Directory,
        Tgz,
        Zip,
        Url
    }
}
