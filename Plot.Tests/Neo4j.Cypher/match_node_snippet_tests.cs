using Plot.Metadata;
using Plot.Neo4j.Cypher;
using Xunit;

namespace Plot.Tests.Neo4j.Cypher
{
    public class match_node_snippet_tests
    {
        [Fact]
        public void it_generates_valid_cypher()
        {
            var metadata = new NodeMetadata
            {
                Labels = new[] { "Node" }
            };
            var entity = new
            {
                Id = "1"
            };
            var nodeSnippet = new NodeSnippet(metadata, entity);
            var identifierNameSnippet = new NodeIdentifierSnippet(metadata, entity);
            var matchNodeSnippet = new MatchPropertySnippet(nodeSnippet, identifierNameSnippet);
            Assert.Equal("(Node_1:Node {Id:{Node_1}})", matchNodeSnippet.ToString());
        }

        [Fact]
        public void it_generates_valid_cypher_when_multiple_lables_supplied()
        {
            var metadata = new NodeMetadata
            {
                Labels = new[] { "Node", "ParentType" }
            };
            var entity = new
            {
                Id = "1"
            };
            var nodeSnippet = new NodeSnippet(metadata, entity);
            var identifierNameSnippet = new NodeIdentifierSnippet(metadata, entity);
            var matchNodeSnippet = new MatchPropertySnippet(nodeSnippet, identifierNameSnippet);
            Assert.Equal("(Node_1:Node:ParentType {Id:{Node_1}})", matchNodeSnippet.ToString());

        }
    }
}
