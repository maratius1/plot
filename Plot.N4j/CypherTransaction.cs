﻿using System;
using System.Collections.Generic;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace Plot.N4j
{
    public class CypherTransaction : ICypherTransaction
    {
        private readonly IGraphClient _db;

        private readonly List<ICypherFluentQuery> _items;

        public CypherTransaction(IGraphClient db)
        {
            _db = db;
            _items = new List<ICypherFluentQuery>();
        }

        public void Commit()
        {
            foreach (var item in _items)
            {
                Log(item);
                item.ExecuteWithoutResults();
            }
            _items.Clear();
        }

        public void Enlist(IMapper mapper, Func<ICypherFluentQuery, ICypherFluentQuery> callback)
        {
            var query = _db.Cypher;

            query = callback(query);

            _items.Add(query);
        }

        private void Log(ICypherFluentQuery query)
        {
            Console.WriteLine(query.Query.DebugQueryText);
        }
    }
}
