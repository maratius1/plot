﻿using Neo4jClient;
using Neo4jClient.Cypher;
using Plot.Metadata;
using Plot.Neo4j;
using Plot.Neo4j.Queries;
using Plot.Queries;
using Plot.Sample.Data.Nodes;
using Plot.Sample.Data.Results;

namespace Plot.Sample.Data.Mappers
{
    public class AddressMapper : Mapper<Address>
    {
        public AddressMapper(GraphClient db, IGraphSession session, ICypherTransactionFactory transactionFactory, IMetadataFactory metadataFactory) 
            : base(db, session, transactionFactory, metadataFactory)
        {
        }

        protected override object GetData(Address item)
        {
            return new AddressNode(item);
        }

        protected override IQueryExecutor<Address> CreateQueryExecutor()
        {
            return new GetQueryExecutor(Db, MetadataFactory);
        }

        #region Queries

        private class GetQueryExecutor : GenericQueryExecutor<Address, AddressResult>
        {
            public GetQueryExecutor(GraphClient db, IMetadataFactory metadataFactory) : base(db, metadataFactory)
            {
            }

            protected override ICypherFluentQuery OnExecute(ICypherFluentQuery cypher)
            {
                return cypher.ReturnDistinct((accessGroup, assets) => new AddressResult
                {
                    Address = accessGroup.As<AddressNode>()
                });
            }
        }

        #endregion
    }
}