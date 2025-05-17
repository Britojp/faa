using FhirArtifactAnalyzer.Domain.Models;
using System.Linq.Expressions;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null, bool disableTracking = false);
        TEntity? Get(Expression<Func<TEntity, bool>> predicate, bool disableTracking = false);
        TEntity? Get(string? id, bool disableTracking = false);
        void Add(TEntity entity);
        void Delete(TEntity entity);
        AttachmentFile? GetAttachment(TEntity document, string attachmentName);
        void Attach(TEntity entity, AttachmentFile file);
        void Commit();
    }
}
