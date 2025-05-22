using System;
using System.Collections.Generic;
using System.Linq;
using FhirArtifactAnalyzer.Models;

namespace FhirArtifactAnalyzer.Services
{
    /// <summary>
    /// Service to manage graph data and operations for FHIR resources
    /// </summary>
    public class GraphService
    {
        /// <summary>
        /// Convert FHIR resources to graph nodes and links
        /// </summary>
        public (List<GraphNode> Nodes, List<GraphLink> Links) BuildGraphData(IEnumerable<FhirResource> resources)
        {
            var nodes = new List<GraphNode>();
            var links = new List<GraphLink>();
            var resourceLookup = new Dictionary<string, FhirResource>();
            
            if (resources == null)
                return (nodes, links);
                
            // First pass: create nodes and build lookup
            foreach (var resource in resources)
            {
                // Use canonical URL as ID if available, otherwise use resource ID
                var nodeId = resource.CanonicalUrl ?? resource.Id;
                
                if (!string.IsNullOrEmpty(nodeId))
                {
                    nodes.Add(GraphNode.FromFhirResource(resource));
                    resourceLookup[nodeId] = resource;
                }
            }
            
            // Second pass: create links
            foreach (var resource in resources)
            {
                var sourceId = resource.CanonicalUrl ?? resource.Id;
                
                if (string.IsNullOrEmpty(sourceId) || resource.References == null)
                    continue;
                    
                foreach (var reference in resource.References)
                {
                    var targetRef = reference.Reference;
                    
                    // Skip if target reference is empty
                    if (string.IsNullOrEmpty(targetRef))
                        continue;
                        
                    // Check if target exists in our resources
                    bool targetExists = resourceLookup.ContainsKey(targetRef);
                    
                    // Only create links between resources we have
                    if (targetExists)
                    {
                        links.Add(GraphLink.FromResourceReference(sourceId, targetRef, reference));
                    }
                }
            }
            
            return (nodes, links);
        }
        
        /// <summary>
        /// Apply filter to graph data
        /// </summary>
        public (List<GraphNode> Nodes, List<GraphLink> Links) ApplyFilter(
            List<GraphNode> nodes, 
            List<GraphLink> links, 
            ArtifactFilter filter)
        {
            if (filter == null)
                return (nodes, links);
                
            var filteredNodes = nodes.ToList();
            
            // Filter by resource type
            if (filter.ResourceTypes.Count > 0)
            {
                filteredNodes = filteredNodes.Where(n => filter.ResourceTypes.Contains(n.Type)).ToList();
            }
            
            // Filter by search text
            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                var searchTerm = filter.SearchText.ToLowerInvariant();
                filteredNodes = filteredNodes.Where(n => 
                    (n.Label?.ToLowerInvariant().Contains(searchTerm) ?? false) ||
                    (n.Description?.ToLowerInvariant().Contains(searchTerm) ?? false) ||
                    (n.Url?.ToLowerInvariant().Contains(searchTerm) ?? false)
                ).ToList();
            }
            
            // Filter by validation status
            if (!string.IsNullOrEmpty(filter.ValidationStatus))
            {
                filteredNodes = filteredNodes.Where(n => n.ValidationStatus == filter.ValidationStatus).ToList();
            }
            
            // Get IDs of filtered nodes
            var filteredNodeIds = filteredNodes.Select(n => n.Id).ToHashSet();
            
            // Filter links based on filtered nodes
            var filteredLinks = links.Where(l => 
                filteredNodeIds.Contains(l.Source) && 
                filteredNodeIds.Contains(l.Target)
            ).ToList();
            
            return (filteredNodes, filteredLinks);
        }
        
        /// <summary>
        /// Find the nodes connected to a specific node
        /// </summary>
        public List<string> FindConnectedNodes(List<GraphLink> links, string nodeId)
        {
            var connectedNodes = new HashSet<string>();
            
            foreach (var link in links)
            {
                if (link.Source == nodeId)
                {
                    connectedNodes.Add(link.Target);
                }
                else if (link.Target == nodeId)
                {
                    connectedNodes.Add(link.Source);
                }
            }
            
            return connectedNodes.ToList();
        }
    }
}
