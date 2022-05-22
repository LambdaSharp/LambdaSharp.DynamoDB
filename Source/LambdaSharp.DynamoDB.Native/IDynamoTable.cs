/*
 * LambdaSharp (λ#)
 * Copyright (C) 2018-2022
 * lambdasharp.net
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LambdaSharp.DynamoDB.Native.Operations;

namespace LambdaSharp.DynamoDB.Native {

    /// <summary>
    /// Interface exposing DynamoDB operations in a type-safe mannter using LINQ expressions.
    /// </summary>
    public interface IDynamoTable {

        // TODO (2021-07-05, bjorg): add 'Scan()` API

        //--- Methods ---

        /// <summary>
        /// Specify the GetItem operation for the given primary key.
        /// </summary>
        /// <param name="primaryKey">Primary key of the item to retrieve.</param>
        /// <param name="consistentRead">Boolean indicating if the read operation should be performed against the main partition (2x cost compared to eventual consistent read).</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        IDynamoTableGetItem<TItem> GetItem<TItem>(DynamoPrimaryKey<TItem> primaryKey, bool consistentRead = false)
            where TItem : class;

        /// <summary>
        /// Specify the PutItem operation for the given primary key and item. When successful, this operation creates a new row or replaces all attributes of the matching row.
        /// </summary>
        /// <param name="primaryKey">Primary key of the item to write.</param>
        /// <param name="item">The item to write</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        IDynamoTablePutItem<TItem> PutItem<TItem>(DynamoPrimaryKey<TItem> primaryKey, TItem item)
            where TItem : class;

        /// <summary>
        /// Specify the UpdateItem operation for the given primary key.
        /// </summary>
        /// <param name="primaryKey">Primary key of the item to update.</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        IDynamoTableUpdateItem<TItem> UpdateItem<TItem>(DynamoPrimaryKey<TItem> primaryKey)
            where TItem : class;

        /// <summary>
        /// Specify the DeleteItem operation for the given primary key.
        /// </summary>
        /// <param name="primaryKey">Primary key of the item to delete.</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        IDynamoTableDeleteItem<TItem> DeleteItem<TItem>(DynamoPrimaryKey<TItem> primaryKey)
            where TItem : class;

        /// <summary>
        /// Specify the BatchGetItems operation for a list of primary keys that all share the same item type.
        /// </summary>
        /// <param name="primaryKeys">List of primary keys to retrieve.</param>
        /// <param name="consistentRead">Boolean indicating if the read operations should be performed against the main partition (2x cost compared to eventual consistent read).</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        IDynamoTableBatchGetItems<TItem> BatchGetItems<TItem>(IEnumerable<DynamoPrimaryKey<TItem>> primaryKeys, bool consistentRead = false)
            where TItem : class;

        /// <summary>
        /// Specify the BatchGetItems operation with mixed item types.
        /// </summary>
        /// <param name="consistentRead">Boolean indicating if the read operations should be performed against the main partition (2x cost compared to eventual consistent read).</param>
        IDynamoTableBatchGetItems BatchGetItems(bool consistentRead = false);

        /// <summary>
        /// Specify the BatchWriteItems operation to write or delete rows.
        /// </summary>
        IDynamoTableBatchWriteItems BatchWriteItems();

        /// <summary>
        /// Specify the TransactGetItems operation for a list of primary keys that all share the same item type.
        /// </summary>
        /// <param name="primaryKeys">List of primary keys to retrieve.</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        IDynamoTableTransactGetItems<TItem> TransactGetItems<TItem>(IEnumerable<DynamoPrimaryKey<TItem>> primaryKeys)
            where TItem : class;

        /// <summary>
        /// Specify the TransactGetItems operation with mixed item types.
        /// </summary>
        IDynamoTableTransactGetItems TransactGetItems();

        /// <summary>
        /// Specify the TransactWriteItems to created, put, update, or delete items in a transaction.
        /// </summary>
        IDynamoTableTransactWriteItems TransactWriteItems();

        /// <summary>
        /// Specify the Query operation to fetch a list of items of the same type.
        /// </summary>
        /// <param name="queryClause">The query clause that specifies the index and sort-key constraints.</param>
        /// <param name="limit">The maximum number of items to read.</param>
        /// <param name="scanIndexForward">The direction of index scan.</param>
        /// <param name="consistentRead">Boolean indicating if the read operations should be performed against the main partition (2x cost compared to eventual consistent read).</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        IDynamoTableQuery<TItem> Query<TItem>(IDynamoQueryClause<TItem> queryClause, int limit = int.MaxValue, bool scanIndexForward = true, bool consistentRead = false)
            where TItem : class;

        /// <summary>
        /// Specify the Query operation to fetch a list of mixed items type.
        /// </summary>
        /// <param name="queryClause">The query clause that specifies the index and sort-key constraints.</param>
        /// <param name="limit">The maximum number of items to read.</param>
        /// <param name="scanIndexForward">The direction of index scan.</param>
        /// <param name="consistentRead">Boolean indicating if the read operations should be performed against the main partition (2x cost compared to eventual consistent read).</param>
        IDynamoTableQuery Query(IDynamoQueryClause queryClause, int limit = int.MaxValue, bool scanIndexForward = true, bool consistentRead = false);

        //--- Default Methods ---

        /// <summary>
        /// Perform a GetItem operation that retrieves all attributes for the given primary key.
        ///
        /// This method is the same: <c>GetItem(primaryKey, consistentRead).ExecuteAsync(cancellationToken)</c>.
        /// </summary>
        /// <param name="primaryKey">Primary key of the item to retrieve.</param>
        /// <param name="consistentRead">Boolean indicating if the read operation should be performed against the main partition (2x cost compared to eventual consistent read).</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        Task<TItem?> GetItemAsync<TItem>(DynamoPrimaryKey<TItem> primaryKey, bool consistentRead = false, CancellationToken cancellationToken = default)
            where TItem : class
            => GetItem(primaryKey, consistentRead).ExecuteAsync(cancellationToken);

        /// <summary>
        /// Performs a PutItem operation that creates/replaces the row matched by the given primary key with the given item.
        ///
        /// This method is the same: <c>PuItem(primaryKey, item).ExecuteAsync(cancellationToken)</c>.
        /// </summary>
        /// <param name="item">The item to write</param>
        /// <param name="primaryKey">Primary key of the item to write.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        Task<bool> PutItemAsync<TItem>(DynamoPrimaryKey<TItem> primaryKey, TItem item, CancellationToken cancellationToken = default)
            where TItem : class
            => PutItem(primaryKey, item).ExecuteAsync(cancellationToken);

        /// <summary>
        /// Performs a DeleteItem operation to delete the given primary key.
        ///
        /// This method is the same: <c>DeleteItem(primaryKey).ExecuteAsync(cancellationToken)</c>.
        /// </summary>
        /// <param name="primaryKey">Primary key of the item to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        Task<bool> DeleteItemAsync<TItem>(DynamoPrimaryKey<TItem> primaryKey, CancellationToken cancellationToken = default)
            where TItem : class
            => DeleteItem(primaryKey).ExecuteAsync(cancellationToken);

        /// <summary>
        /// Performs a Query operation to retrieve a single.
        ///
        /// This method is the same: <c>Query(queryClause, limit: 1, consistentRead).ExecuteFetchAllAttributesAsync(cancellationToken)</c>.
        /// </summary>
        /// <param name="queryClause">The query clause that specifies the index and sort-key constraints.</param>
        /// <param name="consistentRead">Boolean indicating if the read operations should be performed against the main partition (2x cost compared to eventual consistent read).</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        async Task<TItem?> QuerySingleAsync<TItem>(IDynamoQueryClause<TItem> queryClause, bool consistentRead = false, CancellationToken cancellationToken = default)
            where TItem : class
            => (await Query(queryClause, limit: 1, consistentRead).ExecuteFetchAllAttributesAsync(cancellationToken)).FirstOrDefault();
    }
}
