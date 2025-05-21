/**
 * D3.js and Blazor interop functions for graph visualization
 */

// Store reference to the current graph visualization
let currentGraph = null;
let svgElement = null;
let simulation = null;
let width = 0;
let height = 0;
let zoom = null;

window.d3Interop = {
    /**
     * Initialize D3 graph visualization
     * @param {string} elementId - The HTML element ID where the graph will be rendered
     * @param {object} config - Configuration options for the graph
     */
    initializeGraph: function (elementId, config) {
        // Clear any existing graph
        const container = document.getElementById(elementId);
        if (!container) return;
        
        container.innerHTML = '';
        width = container.clientWidth;
        height = container.clientHeight;
        
        // Create SVG element
        svgElement = d3.select(container)
            .append("svg")
            .attr("width", "100%")
            .attr("height", "100%")
            .attr("viewBox", [0, 0, width, height])
            .attr("style", "font: 12px sans-serif");
            
        // Add zoom behavior
        zoom = d3.zoom()
            .scaleExtent([0.1, 8])
            .on("zoom", (event) => {
                graphGroup.attr("transform", event.transform);
            });
        
        svgElement.call(zoom);
        
        // Add a group for the graph elements
        const graphGroup = svgElement.append("g");
        
        // Add arrow markers for directed links
        svgElement.append("defs").append("marker")
            .attr("id", "arrow")
            .attr("viewBox", "0 -5 10 10")
            .attr("refX", 15)
            .attr("refY", 0)
            .attr("markerWidth", 6)
            .attr("markerHeight", 6)
            .attr("orient", "auto")
            .append("path")
            .attr("fill", "#999")
            .attr("d", "M0,-5L10,0L0,5");
            
        // Store reference to render elements
        currentGraph = {
            svg: svgElement,
            group: graphGroup,
            nodes: [],
            links: [],
            nodeElements: null,
            linkElements: null,
            textElements: null,
            config: config
        };
        
        return true;
    },
    
    /**
     * Render or update the graph with new data
     * @param {array} nodes - Array of node objects
     * @param {array} links - Array of link objects
     */
    renderGraph: function (nodes, links) {
        if (!currentGraph) return false;
        
        // Update stored data
        currentGraph.nodes = nodes;
        currentGraph.links = links;
        
        // Set up the simulation
        simulation = d3.forceSimulation(nodes)
            .force("link", d3.forceLink(links).id(d => d.id).distance(100))
            .force("charge", d3.forceManyBody().strength(-300))
            .force("center", d3.forceCenter(width / 2, height / 2))
            .force("collide", d3.forceCollide(30));
        
        const group = currentGraph.group;
        
        // Create links
        currentGraph.linkElements = group.selectAll(".link")
            .data(links)
            .join("line")
            .attr("class", "link")
            .attr("stroke", "#999")
            .attr("stroke-width", 1.5)
            .attr("marker-end", "url(#arrow)");
        
        // Create nodes
        currentGraph.nodeElements = group.selectAll(".node")
            .data(nodes)
            .join("circle")
            .attr("class", "node")
            .attr("r", 10)
            .attr("fill", d => this.getNodeColor(d.type))
            .call(this.setupDragBehavior());
            
        // Add labels to nodes
        currentGraph.textElements = group.selectAll(".label")
            .data(nodes)
            .join("text")
            .attr("class", "label")
            .attr("text-anchor", "middle")
            .attr("dy", 20)
            .text(d => d.label || d.id);
            
        // Add titles for hover tooltips
        currentGraph.nodeElements.append("title")
            .text(d => `${d.type}: ${d.id}\n${d.description || ""}`);
            
        // Start simulation
        simulation.on("tick", () => this.updatePositions());
        
        return true;
    },
    
    /**
     * Get color for a node based on resource type
     */
    getNodeColor: function(type) {
        const colorMap = {
            "StructureDefinition": "#4285F4", // Blue
            "CodeSystem": "#EA4335",         // Red
            "ValueSet": "#FBBC05",           // Yellow
            "CapabilityStatement": "#34A853", // Green
            "ImplementationGuide": "#9C27B0", // Purple
            "OperationDefinition": "#FF9800", // Orange
            "SearchParameter": "#00BCD4"      // Cyan
        };
        
        return colorMap[type] || "#607D8B"; // Default gray
    },
    
    /**
     * Update positions during simulation
     */
    updatePositions: function() {
        if (!currentGraph) return;
        
        currentGraph.linkElements
            .attr("x1", d => d.source.x)
            .attr("y1", d => d.source.y)
            .attr("x2", d => d.target.x)
            .attr("y2", d => d.target.y);
            
        currentGraph.nodeElements
            .attr("cx", d => d.x)
            .attr("cy", d => d.y);
            
        currentGraph.textElements
            .attr("x", d => d.x)
            .attr("y", d => d.y);
    },
    
    /**
     * Set up drag behavior for nodes
     */
    setupDragBehavior: function() {
        return d3.drag()
            .on("start", (event, d) => {
                if (!event.active) simulation.alphaTarget(0.3).restart();
                d.fx = d.x;
                d.fy = d.y;
            })
            .on("drag", (event, d) => {
                d.fx = event.x;
                d.fy = event.y;
            })
            .on("end", (event, d) => {
                if (!event.active) simulation.alphaTarget(0);
                d.fx = null;
                d.fy = null;
            });
    },
    
    /**
     * Highlight specific nodes and their connections
     */
    highlightNode: function (nodeId) {
        if (!currentGraph) return;
        
        // Reset all nodes and links to default appearance
        currentGraph.nodeElements.attr("opacity", 0.3);
        currentGraph.linkElements.attr("opacity", 0.3);
        currentGraph.textElements.attr("opacity", 0.3);
        
        // Find the selected node and its connected nodes
        const connectedNodes = new Set();
        currentGraph.links.forEach(link => {
            if (link.source.id === nodeId || link.target.id === nodeId) {
                connectedNodes.add(link.source.id);
                connectedNodes.add(link.target.id);
            }
        });
        
        // Highlight the selected node and its connections
        currentGraph.nodeElements
            .filter(d => d.id === nodeId || connectedNodes.has(d.id))
            .attr("opacity", 1)
            .attr("stroke", "#000")
            .attr("stroke-width", node => node.id === nodeId ? 2 : 1);
            
        currentGraph.linkElements
            .filter(link => link.source.id === nodeId || link.target.id === nodeId)
            .attr("opacity", 1)
            .attr("stroke-width", 2);
            
        currentGraph.textElements
            .filter(d => d.id === nodeId || connectedNodes.has(d.id))
            .attr("opacity", 1);
    },
    
    /**
     * Reset highlighting
     */
    resetHighlighting: function() {
        if (!currentGraph) return;
        
        currentGraph.nodeElements
            .attr("opacity", 1)
            .attr("stroke", null)
            .attr("stroke-width", null);
            
        currentGraph.linkElements
            .attr("opacity", 1)
            .attr("stroke-width", 1.5);
            
        currentGraph.textElements
            .attr("opacity", 1);
    },
    
    /**
     * Apply filter to graph
     */
    applyFilter: function(filter) {
        if (!currentGraph || !currentGraph.nodes) return;
        
        // Filter nodes based on type and other criteria
        const filteredNodes = currentGraph.nodes.filter(node => {
            // If type filter is specified, check if node type matches
            if (filter.types && filter.types.length > 0) {
                if (!filter.types.includes(node.type)) return false;
            }
            
            // If search text is specified, check if node contains text
            if (filter.searchText && filter.searchText.length > 0) {
                const searchLower = filter.searchText.toLowerCase();
                const nameMatch = (node.label || "").toLowerCase().includes(searchLower);
                const idMatch = (node.id || "").toLowerCase().includes(searchLower);
                const descMatch = (node.description || "").toLowerCase().includes(searchLower);
                
                if (!nameMatch && !idMatch && !descMatch) return false;
            }
            
            return true;
        });
        
        // Get filtered node IDs
        const filteredNodeIds = new Set(filteredNodes.map(node => node.id));
        
        // Filter links that connect filtered nodes
        const filteredLinks = currentGraph.links.filter(link => 
            filteredNodeIds.has(link.source.id || link.source) && 
            filteredNodeIds.has(link.target.id || link.target)
        );
        
        // Update visualization with filtered data
        this.renderGraph(filteredNodes, filteredLinks);
    },
    
    /**
     * Export the current graph view as PNG
     */
    exportPng: function() {
        if (!svgElement) return null;
        
        // Create a copy of the SVG with a white background
        const svgCopy = svgElement.node().cloneNode(true);
        const svgData = new XMLSerializer().serializeToString(svgCopy);
        
        // Create a canvas element
        const canvas = document.createElement("canvas");
        const bbox = svgElement.node().getBoundingClientRect();
        canvas.width = bbox.width;
        canvas.height = bbox.height;
        
        // Create an image and load the SVG data
        const img = new Image();
        const context = canvas.getContext("2d");
        
        // Return a promise that resolves with the data URL
        return new Promise((resolve, reject) => {
            img.onload = function() {
                // Draw white background
                context.fillStyle = "white";
                context.fillRect(0, 0, canvas.width, canvas.height);
                
                // Draw the image
                context.drawImage(img, 0, 0);
                
                // Convert to PNG
                const dataUrl = canvas.toDataURL("image/png");
                resolve(dataUrl);
            };
            
            img.onerror = reject;
            
            // Convert SVG to a data URL
            const svgBlob = new Blob([svgData], { type: "image/svg+xml;charset=utf-8" });
            const url = URL.createObjectURL(svgBlob);
            img.src = url;
        });
    },
    
    /**
     * Reset zoom and center the graph
     */
    resetZoom: function() {
        if (!svgElement || !zoom) return;
        
        svgElement.transition()
            .duration(750)
            .call(zoom.transform, d3.zoomIdentity);
    }
};
