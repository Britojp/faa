using FhirArtifactAnalyzer.Domain.Enums;
using Raven.Client.Documents.Session;

namespace FhirArtifactAnalyzer.Infrastructure.Extensions
{
    public static class DocumentQueryExtensions
    {
        public static IDocumentQuery<T> UseOperator<T>(
            this IDocumentQuery<T> query, 
            SearchQueryOperator @operator) where T : class
        {
            return @operator switch
            {
                SearchQueryOperator.And => query.AndAlso(),
                SearchQueryOperator.Or => query.OrElse(),
                _ => query
            };
        }
    }
}
