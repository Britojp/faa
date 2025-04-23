using FhirArtifactAnalyzer.Domain.Models;
using System.Linq.Expressions;

namespace FhirArtifactAnalyzer.Domain.Abstractions
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null);
        TEntity? Get(Expression<Func<TEntity, bool>> predicate);
        TEntity? Get(string? id);
        void Add(TEntity entity);
        void Delete(TEntity entity);
        void Attach(TEntity entity, AttachmentFile file);
        AttachmentFile? GetAttachmentFor(TEntity document);
        void Commit();
    }
}
