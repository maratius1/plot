﻿using System.Linq;
using Plot.Metadata;
using Plot.Tests.Model;
using Xunit;

namespace Plot.Tests
{
    public class metadata_factory_tests
    {
        [Fact]
        public void factory_creates_meta_data()
        {
            var factory = new AttributeMetadataFactory();
            var node = factory.Create(typeof(Person));
            Assert.Equal("Person", node.Name);
            Assert.Equal(3, node.Properties.Count());

            Assert.Equal("Id", node["Id"].Name);
            Assert.True(node["Id"].IsPrimitive);

            Assert.Equal("Name", node["Name"].Name);
            Assert.True(node["Name"].IsPrimitive);

            Assert.Equal("Contacts", node["Contacts"].Name);
            Assert.True(node["Contacts"].IsList);
            Assert.False(node["Contacts"].IsPrimitive);
            Assert.NotNull(node["Contacts"].Relationship);
            Assert.Equal("LINKED_TO", node["Contacts"].Relationship.Name);
        }
    }
}