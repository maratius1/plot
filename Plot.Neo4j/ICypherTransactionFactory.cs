﻿using System.Collections.Generic;
using Plot.Logging;
using Plot.Neo4j.Cypher;

namespace Plot.Neo4j
{
    public interface ICypherTransactionFactory
    {
        ICypherTransaction Create(IGraphSession session);
        IList<T> Run<T>(ICypherQuery<T> query);
        ILogger Logger { get; }
    }
}
