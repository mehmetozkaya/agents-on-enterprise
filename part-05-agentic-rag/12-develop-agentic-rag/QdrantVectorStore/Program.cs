using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;
using QdrantVectorStore;
using QdrantVectorStoreType = Microsoft.SemanticKernel.Connectors.Qdrant.QdrantVectorStore;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

var credential = new AzureCliCredential();

// 1. Initialize the Chat Client (The Agent's Brain)
var chatClient = new AzureOpenAIClient(new Uri(endpoint), credential)
    .GetChatClient(deploymentName)
    .AsIChatClient();

// 2. Initialize the Embedding Generator (The Translator)
var embeddingGenerator = new AzureOpenAIClient(new Uri(endpoint), credential)
    .GetEmbeddingClient("text-embedding-3-small")
    .AsIEmbeddingGenerator();

// 3. Connect to the Qdrant Vector Database
var qdrantClient = new QdrantClient("localhost", 6334);
var vectorStore = new QdrantVectorStoreType(qdrantClient, ownsClient: true);

// Get the specific collection containing our Architectural records
var adrCollection = vectorStore.GetCollection<Guid, ArchitectureDecision>("enterprise_adrs");
await adrCollection.EnsureCollectionExistsAsync();

// 4. Seed the collection with sample ADR records
var sampleAdrs = new List<ArchitectureDecision>
{
    new()
    {
        DocumentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Title = "ADR-001: gRPC for Internal Microservices Communication",
        Content = "In January 2024, we decided to adopt gRPC over REST for internal microservices communication. Key reasons: 1) Binary protocol (Protocol Buffers) provides 10x better performance than JSON, 2) Strong typing with .proto contracts reduces integration bugs, 3) Native support for bidirectional streaming enables real-time data flows, 4) HTTP/2 multiplexing reduces connection overhead. Trade-offs accepted: steeper learning curve and reduced browser compatibility (acceptable for internal services)."
    },
    new()
    {
        DocumentId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        Title = "ADR-002: PostgreSQL as Primary Database",
        Content = "In March 2024, we selected PostgreSQL over MongoDB for our primary database. Reasons: ACID compliance required for financial transactions, mature tooling ecosystem, excellent JSON support for semi-structured data, and lower operational costs. We use MongoDB only for specific document-heavy workloads."
    },
    new()
    {
        DocumentId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
        Title = "ADR-003: Event-Driven Architecture with Kafka",
        Content = "In February 2024, we adopted Apache Kafka for event-driven communication between bounded contexts. This decouples services, enables event sourcing, and provides replay capability for debugging. RabbitMQ was rejected due to limited horizontal scaling."
    }
};

// Generate embeddings and upsert records
foreach (var adr in sampleAdrs)
{
    var embedding = await embeddingGenerator.GenerateAsync(adr.Content);
    adr.ContentVector = embedding.Vector;
    await adrCollection.UpsertAsync(adr);
}

Console.WriteLine($"Seeded {sampleAdrs.Count} ADR records into Qdrant.\n");

// 6. Configure the TextSearchProvider options for RAG behavior
TextSearchProviderOptions textSearchOptions = new()
{
    SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
};

// 7. Create the Vector Search Adapter
async Task<IEnumerable<TextSearchProvider.TextSearchResult>> VectorSearchAdapter(string query, CancellationToken cancellationToken)
{
    // Generate embedding for the user's query
    var queryEmbedding = await embeddingGenerator.GenerateAsync(query, cancellationToken: cancellationToken);
    var queryVector = queryEmbedding.Vector;

    // Search the Qdrant vector store for semantically similar ADRs (top 3 results)
    var searchOptions = new VectorSearchOptions<ArchitectureDecision>();
    var searchResults = adrCollection.SearchAsync(queryVector, 3, searchOptions, cancellationToken);

    // Convert Qdrant results to TextSearchProvider results
    var results = new List<TextSearchProvider.TextSearchResult>();
    await foreach (var result in searchResults)
    {
        results.Add(new TextSearchProvider.TextSearchResult
        {
            SourceName = $"ADR: {result.Record.Title}",
            SourceLink = $"adr://{result.Record.DocumentId}",
            Text = $"Title: {result.Record.Title}\nContent: {result.Record.Content}"
        });
    }

    return results;
}

// 8. Initialize the Agent with the Qdrant-backed RAG capability
AIAgent architectAgent = chatClient.AsAIAgent(new ChatClientAgentOptions()
{
    Name = "EnterpriseArchitect",
    ChatOptions = new()
    {
        Instructions = "You are a senior enterprise architect. Always reference the provided ADR context to answer questions about past architectural decisions."
    },
    AIContextProviders = [new TextSearchProvider(VectorSearchAdapter, textSearchOptions)]
});

Console.WriteLine("--- Enterprise Architecture Swarm Online ---\n");

// The active researcher loop begins
string query = "Why did we choose gRPC over REST for the internal microservices communication in 2024 ?";
Console.WriteLine($"User: {query}");

// The agent will autonomously:
// 1. Invoke the VectorSearchAdapter with the user's query.
// 2. The adapter will embed the query and search Qdrant.
// 3. Qdrant will return the top semantic matches.
// 4. The agent will read the retrieved ADRs and synthesize the final answer.
AgentResponse response = await architectAgent.RunAsync(query);

Console.WriteLine($"\nAgent: {response.Text}");
