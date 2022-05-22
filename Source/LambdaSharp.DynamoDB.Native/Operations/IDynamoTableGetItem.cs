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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LambdaSharp.DynamoDB.Native.Operations {

    /// <summary>
    /// Interface to specify the GetItem operation.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    public interface IDynamoTableGetItem<TItem> where TItem : class {

        //--- Methods ---

        /// <summary>
        /// Selects a item property to fetch.
        /// </summary>
        /// <param name="attribute">A lambda expression that returns the item property.</param>
        /// <typeparam name="T">The property type.</typeparam>
        IDynamoTableGetItem<TItem> Get<T>(Expression<Func<TItem, T>> attribute);

        /// <summary>
        /// Execute the GetItem operation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The item when found and condition is met. <c>null</c>, otherwise.</returns>
        Task<TItem?> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
