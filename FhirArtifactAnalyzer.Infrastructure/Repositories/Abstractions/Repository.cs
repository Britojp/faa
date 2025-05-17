using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Models;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Linq.Expressions;

namespace FhirArtifactAnalyzer.Infrastructure.Repositories.Abstractions
{
    public abstract class Repository<TEntity>(IDocumentSession session) : IDisposable, IRepository<TEntity> where TEntity : class
    {
        public IEnumerable<TEntity> GetAll(
            Expression<Func<TEntity, bool>>? predicate = null, 
            bool disableTracking = false)
        {
            var query = session.Query<TEntity>();
            
            if (disableTracking)
                query = query.Customize(opt => opt.NoTracking());

            if (predicate is not null)
                session.Query<TEntity>().Where(predicate);

            return [.. query];
        }

        public TEntity? Get(string? id, bool disableTracking = false)
        {
            var entity = session.Load<TEntity>(id);

            if (disableTracking)
                session.Advanced.Evict(entity);

            return entity;
        }

        public TEntity? Get(Expression<Func<TEntity, bool>> predicate, bool disableTracking = false)
        {
            var query = session.Query<TEntity>();

            if (disableTracking)
                query = query.Customize(opt => opt.NoTracking());

            return query.Where(predicate).FirstOrDefault();
        }

        public void Add(TEntity entity)
        {
            session.Store(entity);
        }

        public void Delete(TEntity entity)
        {
            session.Delete(entity);
        }

        public AttachmentFile? GetAttachment(TEntity document, string attachmentName)
        {
            var attachment = session.Advanced.Attachments.Get(document, attachmentName);

            if (attachment is null)
            {
                return null;
            }

            return new(attachment.Details.Name, attachment.Stream, attachment.Details.ContentType);
        }

        public void Attach(TEntity entity, AttachmentFile file)
        {
            session.Advanced.Attachments.Store(entity, file.Name, file.Stream, file.ContentType);
        }

        public void Commit()
        {
            session.SaveChanges();
        }

        public void Dispose()
        {
            session.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
