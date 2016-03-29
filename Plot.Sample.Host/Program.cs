﻿using System;
using System.Collections.Generic;
using Plot.Logging;
using Plot.Neo4j;
using Plot.Sample.Data.Mappers;
using Plot.Sample.Queries;

namespace Plot.Sample.Host
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var uri = new Uri("http://localhost:7474/db/data");
            Configuration.Logger = () => new ConsoleLogger();
            var factory = Configuration.CreateGraphSessionFactory(uri, "neo4j", "trackmatic", typeof (UserMapper).Assembly);
            using (var session = factory.OpenSession())
            {
                //var organisation = session.Get<Organisation>("e08811bf-775f-432c-ba5f-b50150ef8964");

                var asset = session.Get<Asset>("1");

                //asset.Type = session.Create(new Vehicle());

                session.SaveChanges();
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}