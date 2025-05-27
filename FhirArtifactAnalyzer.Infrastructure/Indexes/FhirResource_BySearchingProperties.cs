using FhirArtifactAnalyzer.Domain.Models;
using Raven.Client.Documents.Indexes;

namespace FhirArtifactAnalyzer.Infrastructure.Indexes
{
    public class FhirResource_BySearchingProperties : AbstractIndexCreationTask<FhirResource>
    {
        public FhirResource_BySearchingProperties()
        {
            Map = resources
                => from resource in resources
                   select new FhirResource
                   {
                       Id = resource.Id,
                       Name = resource.Name,
                       TypeName = resource.TypeName,
                       Description = resource.Description,
                       Url = resource.Url,
                       Comment = resource.Comment,
                       Attachment = resource.Attachment,
                   };

            Index(x => x.Id, FieldIndexing.Exact);
            Index(x => x.Name, FieldIndexing.Search);
            Index(x => x.TypeName, FieldIndexing.Search);
            Index(x => x.Description, FieldIndexing.Search);
            Index(x => x.Url, FieldIndexing.Search);
            Index(x => x.Comment, FieldIndexing.Search);
            Index(x => x.Attachment, FieldIndexing.No);

            StoreAllFields(FieldStorage.Yes);
        }
    }
}
