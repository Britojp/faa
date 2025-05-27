using Elasticsearch.Net;
using FhirArtifactAnalyzer.Domain.Abstractions;
using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using FhirArtifactAnalyzer.Infrastructure.Repositories;
using Nest;
using System.Linq.Expressions;

namespace FhirArtifactAnalyzer.Infrastructure.Proxies
{
    public sealed class ElasticSyncRepository<T> : Domain.Abstractions.IRepository<T> where T : class, IEntity
    {
        private readonly Repository<T> _repository;
        private readonly IElasticClient _client;

        public ElasticSyncRepository(IRavenContext context, IElasticClient elasticClient)
        {
            _repository = new Repository<T>(context);
            _client = elasticClient;
        }

        public void Add(T entity)
        {
            _repository.Add(entity);
            _client.IndexDocument(entity);
        }

        public void Attach(T entity, AttachmentFile file)
        {
            _repository.Attach(entity, file);
        }

        public void Commit()
        {
            _repository.Commit();
        }

        public void Delete(T entity)
        {
            _repository.Delete(entity);
            _client.Delete<T>(entity.Id, d => d.Refresh(Refresh.True));
        }

        public T? Get(Expression<Func<T, bool>> predicate)
        {
            return _repository.Get(predicate);
        }

        public T? Get(string id)
        {
            return _repository.Get(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null)
        {
            return _repository.GetAll(predicate);
        }

        public AttachmentFile? GetAttachment(string documentId, string attachmentName)
        {
            return _repository.GetAttachment(documentId, attachmentName);
        }

        public void Update(string id, T entity)
        {
            _repository.Update(id, entity);
            _client.Update<T>(id, u => u.Doc(entity).Refresh(Refresh.True));
        }
    }
}
