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

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LambdaSharp.DynamoDB.Native.Exceptions;

namespace LambdaSharp.DynamoDB.Native.Operations {

    /// <summary>
    /// Interface to specify the BatchGetItems operation with mixed item types.
    /// </summary>
    public interface IDynamoTableBatchGetItems {

        //--- Methods ---

        /// <summary>
        /// Begin specification of a GetItem operation for BatchGetItems.
        /// </summary>
        /// <param name="primaryKey">Primary key of the item to retrieve.</param>
        /// <param name="consistentRead">Boolean indicating if the read operation should be performed against the main partition (2x cost compared to eventual consistent read).</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        IDynamoTableBatchGetItemsGetItem<TItem> BeginGetItem<TItem>(DynamoPrimaryKey<TItem> primaryKey, bool consistentRead = false)
            where TItem : class;

        /// <summary>
        /// Execute the BatchGetItems operation.
        /// </summary>
        /// <param name="maxAttempts">Maximum number of attempts with exponential back when encountering provisioned throughput is exceeded.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>List of found items.</returns>
        /// <exception cref="DynamoTableBatchGetItemsMaxAttemptsExceededException">Thrown when the maximum number of attempts is exceeded.</exception>
        Task<IEnumerable<object>> ExecuteAsync(int maxAttempts = 5, CancellationToken cancellationToken = default);

        //--- Default Methods ---

        /// <summary>
        /// Add a GetItem operation to BatchGetItems that retrieves all attributes for the given primary key.
        ///
        /// This method is the same: <c>BeginGetItem(primaryKey, consistentRead).End()</c>.
        /// </summary>
        /// <param name="primaryKey">Primary key of the item to retrieve.</param>
        /// <param name="consistentRead">Boolean indicating if the read operation should be performed against the main partition (2x cost compared to eventual consistent read).</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        IDynamoTableBatchGetItems GetItem<TItem>(DynamoPrimaryKey<TItem> primaryKey, bool consistentRead = false)
            where TItem : class
            => BeginGetItem(primaryKey, consistentRead).End();
    }

    /// <summary>
    /// Interface to specify a typed GetItem operation for BatchGetItems with mixed item types.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    public interface IDynamoTableBatchGetItemsGetItem<TItem> where TItem : class {

        //--- Methods ---

        /// <summary>
        /// Selects a item property to fetch.
        /// </summary>
        /// <param name="attribute">A lambda expression that returns the item property.</param>
        /// <typeparam name="T">The property type.</typeparam>
        IDynamoTableBatchGetItemsGetItem<TItem> Get<T>(Expression<Func<TItem, T>> attribute);

        /// <summary>
        /// End specification of the GetItem operation for BatchGetItems.
        /// </summary>
        IDynamoTableBatchGetItems End();
    }

    /// <summary>
    /// Interface to specify the BatchGetItems operation for a specific item type.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    public interface IDynamoTableBatchGetItems<TItem> where TItem : class {

        //--- Methods ---

        /// <summary>
        /// Selects a item property to fetch.
        /// </summary>
        /// <param name="attribute">A lambda expression that returns the item property.</param>
        /// <typeparam name="T">The property type.</typeparam>
        IDynamoTableBatchGetItems<TItem> Get<T>(Expression<Func<TItem, T>> attribute);

        /// <summary>
        /// Execute the BatchGetItems operation.
        /// </summary>
        /// <param name="maxAttempts">Maximum number of attempts with exponential back when encountering provisioned throughput is exceeded.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>List of found items.</returns>
        Task<IEnumerable<TItem>> ExecuteAsync(int maxAttempts = 5, CancellationToken cancellationToken = default);
    }
}
