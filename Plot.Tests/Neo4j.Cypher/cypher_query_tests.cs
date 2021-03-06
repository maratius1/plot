﻿using Plot.Logging;
using Plot.Metadata;
using Plot.Neo4j.Cypher;
using Plot.Tests.Model;
using Xunit;

namespace Plot.Tests.Neo4j.Cypher
{
    public class cypher_query_tests
    {
        [Fact]
        public void DuplicateParametersAreOverwritten()
        {
            var query = new CypherQuery<Person>();
            var result = query.WithParam("param1", "value 1");
            Assert.Equal("value 1", result.Parameters["param1"]);
            result = query.WithParam("param1", "value 2");
            Assert.Equal(1, result.Parameters.Count);
            Assert.Equal("value 2", result.Parameters["param1"]);
        }

        [Fact]
        public void MatchShouldGenerateValidSyntax()
        {
            var factory = new AttributeMetadataFactory(null);
            var person = new Person { Id = "1" };
            var node = new Node(factory.Create(person), person);
            var statement = StatementFactory.Match(node, StatementFactory.Parameter(node));
            var query = new CypherQuery<Person>();
            var response = query.Match(statement);
            Assert.Equal("MATCH (Person_1:Person { Id:{Person_1}})", response.Statement);
        }
    }
}
