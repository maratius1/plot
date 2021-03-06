[![NuGet Status](https://img.shields.io/nuget/v/Plot.Neo4j.svg)](https://www.nuget.org/packages/Plot.Neo4j/)

# Plot

.Net Object Graph Mapper for Neo4j

## Install With NuGet

    install-package Plot.Neo4j

## Overview
Plot is an application framework designed to assist in managing CRUD operations for an application domain backed by a graph database. Plot is not designed to cover all features of the Neo4j data store, for that it is best to look at the [Neo4jClient](https://github.com/Readify/Neo4jClient) project.

## Creating a session factory
A graph session factory is required to start interacting with the framwork. A factory is created using the static Configuration class in the Plot.Neo4j assembly. You need to supply one or more assemblies which contain your mappers and queries.

There should only be one instance of the session factory per application.

    var uri = new Uri("http://neo4j:neo4j@localhost:7474/db/data");
    var factory = Configuration.CreateGraphSessionFactory(uri, typeof(OrganisationMapper).Assembly);

## Creating a session
A session is created using the session factory, and is the entry point to working with your data. The session is a lightweight object which should be created for each unit of work.

    using (var session = factory.OpenSession())
    {
    }

## Creating an entity
The session object is used to create new entities. The session.Create method tells the session to track the entity and save it when session.SaveChanges() is called.

    using (var session = factory.OpenSession())
    {
        var entity = session.Create<Organisation>(new Organisation
        {
            Id = "1",
            Name = "Organisation A"
        });
        session.SaveChanges();
    }

## Retrieving an entity by id
Use the session.Get<> method to retrieve an entity based on it's id. The entity is changed tracked, so whatever changes are made to the entity will be flushed to the underlying data store when session.SaveChanges() is called.

    using (var session = factory.OpenSession())
    {
        var entity = session.Get<Organisation>("1");
        entity.Name = "Acme";
        session.SaveChanges();
    }

## Executing queries
You can define custom queries which will be executed against the datastore. The results of these queries can be loaded into the session by setting the enlist parameter to true. This is useful if you need to modify objects which are returned from the query and require those changes to be persisted. You can choose not to enlist the object in order to prevent unescessary resources being used when the objects are for read only purposes

    using (var session = factory.OpenSession())
    {
        var results = session.Get(new GetUsersByUsername
        {
            Username = "my username"
        }, enlist: true);
    }
