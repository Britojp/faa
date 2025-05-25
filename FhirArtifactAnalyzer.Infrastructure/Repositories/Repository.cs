using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Linq.Expressions;

namespace FhirArtifactAnalyzer.Infrastructure.Repositories
{
    public sealed class Repository<TEntity>(IRavenDBContext context) : IDisposable, IRepository<TEntity> where TEntity : class
    {
        private readonly Lazy<IDocumentSession> _lazySession = new(context.DocumentStore.OpenSession);

        private IDocumentSession Session => _lazySession.Value;

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null)
        {
            var query = Session.Query<TEntity>().Customize(c => c.NoTracking());

            if (predicate is not null)
                query = query.Where(predicate);

            return [.. query];
        }

        public TEntity? Get(string id)
        {
            var entity = Session.Load<TEntity>(id);
            Session.Advanced.Evict(entity);
            return entity;
        }

        public TEntity? Get(Expression<Func<TEntity, bool>> predicate)
        {
            var query = Session.Query<TEntity>().Customize(opt => opt.NoTracking());

            return query.Where(predicate).FirstOrDefault();
        }

        public void Add(TEntity entity)
        {
            Session.Store(entity);
        }

        public void Update(string id, TEntity entity)
        {
            Session.Store(entity, id);
        }

        public void Delete(TEntity entity)
        {
            Session.Delete(entity);
        }

        public AttachmentFile? GetAttachment(string documentId, string attachmentName)
        {
            var document = Session.Load<TEntity>(documentId);
            var attachment = Session.Advanced.Attachments.Get(document, attachmentName);

            if (attachment is null)
            {
                return null;
            }

            return new(attachment.Details.Name, attachment.Stream, attachment.Details.ContentType);
        }

        public void Attach(TEntity entity, AttachmentFile file)
        {
            Session.Advanced.Attachments.Store(entity, file.Name, file.Stream, file.ContentType);
        }

        public void Commit()
        {
            Session.SaveChanges();
        }

        public void Dispose()
        {
            if (_lazySession.IsValueCreated)
            {
                Session.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
