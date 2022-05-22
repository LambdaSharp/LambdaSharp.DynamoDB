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

using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using LambdaSharp.DynamoDB.Native.Exceptions;

namespace LambdaSharp.DynamoDB.Native.Operations {

    /// <summary>
    /// Interface to specify the BatchWriteItems operation.
    /// </summary>
    public interface IDynamoTableBatchWriteItems {

        //--- Methods ---

        /// <summary>
        /// Begin specification of a PutItem operation for BatchWriteItems.
        /// </summary>
        /// <param name="primaryKey">Primary key of the item to write.</param>
        /// <param name="item">The item to write</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        IDynamoTableBatchWriteItemsPutItem<TItem> BeginPutItem<TItem>(DynamoPrimaryKey<TItem> primaryKey, TItem item)
            where TItem : class;

        /// <summary>
        /// Add a PutItem operation to BatchWriteItems for the given primary key and item. When successful, this operation creates a new row or replaces all attributes of the matching row.
        /// </summary>
        /// <param name="primaryKey">Primary key of the item to write.</param>
        /// <param name="item">The item to write</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        IDynamoTableBatchWriteItems PutItem<TItem>(TItem item, DynamoPrimaryKey<TItem> primaryKey)
            where TItem : class;

        /// <summary>
        /// Add a DeleteItem operation to BatchWriteItems for the given primary key.
        /// </summary>
        /// <param name="primaryKey">Primary key of the item to delete.</param>
        /// <typeparam name="TItem">The item type.</typeparam>
        IDynamoTableBatchWriteItems DeleteItem<TItem>(DynamoPrimaryKey<TItem> primaryKey)
            where TItem : class;

        /// <summary>
        /// Execute the BatchWriteItems operation.
        /// </summary>
        /// <param name="maxAttempts">Maximum number of attempts with exponential back when encountering provisioned throughput is exceeded.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="DynamoTableBatchWriteItemsMaxAttemptsExceededException">Thrown when the maximum number of attempts is exceeded.</exception>
        Task ExecuteAsync(int maxAttempts = 5, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Interface to specify a PutItem operation for BatchWriteItems.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    public interface IDynamoTableBatchWriteItemsPutItem<TItem> where TItem : class {

        //--- Methods ---

        /// <summary>
        /// Set the value of a DynamoDB item attribute. Used for storing attributes used by local/global secondary indices.
        /// </summary>
        /// <param name="key">Name of attribute.</param>
        /// <param name="value">Value of attribute.</param>
        IDynamoTableBatchWriteItemsPutItem<TItem> Set(string key, AttributeValue value);

        /// <summary>
        /// End specification of the GetItem operation for BatchGetItems.
        /// </summary>
        IDynamoTableBatchWriteItems End();

        //--- Default Methods ---

        /// <summary>
        /// Set the value of a DynamoDB item attribute. Used for storing attributes used by local/global secondary indices.
        /// </summary>
        /// <param name="key">Name of attribute.</param>
        /// <param name="value">Value of attribute.</param>
        IDynamoTableBatchWriteItemsPutItem<TItem> Set(string key, string value)
            => Set(key, new AttributeValue(value));
    }
}
