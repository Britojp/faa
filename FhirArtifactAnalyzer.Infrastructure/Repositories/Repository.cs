using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Models;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Linq.Expressions;

namespace FhirArtifactAnalyzer.Infrastructure.Repositories
{
    public class Repository<TEntity>(IDocumentSession session) : IDisposable, IRepository<TEntity> where TEntity : class
    {
        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null)
        {
            var query = predicate != null
                ? session.Query<TEntity>().Where(predicate)
                : session.Query<TEntity>();

            return query.Customize(x => x.NoTracking()).ToList();
        }

        public TEntity? Get(string? id)
        {
            return session.Load<TEntity>(id);
        }

        public TEntity? Get(Expression<Func<TEntity, bool>> predicate)
        {
            return session.Query<TEntity>().Where(predicate).FirstOrDefault();
        }

        public void Add(TEntity entity)
        {
            session.Store(entity);
        }

        public void Delete(TEntity entity)
        {
            session.Delete(entity);
        }

        public void Attach(TEntity entity, AttachmentFile file)
        {
            session.Advanced.Attachments.Store(entity, file.Name, file.Stream, file.ContentType);
        }

        public AttachmentFile? GetAttachmentFor(TEntity document)
        {
            var attachmentName = session.Advanced.Attachments.GetNames(document).FirstOrDefault();

            if (attachmentName == null)
            {
                return null;
            }

            var attachment = session.Advanced.Attachments.Get(document, attachmentName.Name);

            return new(attachment.Details.Name, attachment.Stream, attachment.Details.ContentType);
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
