using Elasticsearch.Net;
using FhirArtifactAnalyzer.Domain.Models;
using FhirArtifactAnalyzer.Infrastructure.Interfaces;
using FhirArtifactAnalyzer.Infrastructure.Repositories;
using Nest;
using System.Linq.Expressions;

namespace FhirArtifactAnalyzer.Infrastructure.Proxies
{
    public class FhirResourceElasticSyncRepository : Domain.Abstractions.IRepository<FhirResource>
    {
        private readonly Repository<FhirResource> _repository;
        private readonly ElasticClient _client;

        public FhirResourceElasticSyncRepository(IRavenDBContext context, ElasticClient elasticClient)
        {
            _repository = new Repository<FhirResource>(context);
            _client = elasticClient;
        }

        public void Add(FhirResource entity)
        {
            _repository.Add(entity);
            _client.IndexDocument(entity);
        }

        public void Attach(FhirResource entity, AttachmentFile file)
        {
            _repository.Attach(entity, file);
        }

        public void Commit()
        {
            _repository.Commit();
        }

        public void Delete(FhirResource entity)
        {
            _repository.Delete(entity);
            _client.Delete<FhirResource>(entity.Id, d => d.Refresh(Refresh.True));
        }

        public FhirResource? Get(Expression<Func<FhirResource, bool>> predicate)
        {
            return _repository.Get(predicate);
        }

        public FhirResource? Get(string id)
        {
            return _repository.Get(id);
        }

        public IEnumerable<FhirResource> GetAll(Expression<Func<FhirResource, bool>>? predicate = null)
        {
            return _repository.GetAll(predicate);
        }

        public AttachmentFile? GetAttachment(string documentId, string attachmentName)
        {
            return _repository.GetAttachment(documentId, attachmentName);
        }

        public void Update(string id, FhirResource entity)
        {
            _repository.Update(id, entity);
            _client.Update<FhirResource>(id, u => u.Doc(entity).Refresh(Refresh.True));
        }
    }
}
