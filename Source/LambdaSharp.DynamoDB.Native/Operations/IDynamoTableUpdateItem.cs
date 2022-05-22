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
using Amazon.DynamoDBv2.Model;

namespace LambdaSharp.DynamoDB.Native.Operations {

    /// <summary>
    /// Interface to specify a UpdateItem operation.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    public interface IDynamoTableUpdateItem<TItem> where TItem : class {

        //--- Methods ---

        /// <summary>
        /// Add condition for UpdateItem operation.
        /// </summary>
        /// <param name="condition">A lambda predicate representing the DynamoDB condition expression.</param>
        IDynamoTableUpdateItem<TItem> WithCondition(Expression<Func<TItem, bool>> condition);

        /// <summary>
        /// Add condition that item exists for UpdateItem operation.
        /// </summary>
        IDynamoTableUpdateItem<TItem> WithConditionItemExists() => WithCondition(item => DynamoCondition.Exists(item));

        /// <summary>
        /// Add condition that item does not exist for UpdateItem operation.
        /// </summary>
        IDynamoTableUpdateItem<TItem> WithConditionItemDoesNotExist() => WithCondition(item => DynamoCondition.DoesNotExist(item));

        // *** `SET Foo.Bar = :value` action ***

        /// <summary>
        /// Set a item property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item property.</param>
        /// <param name="value">The value to set.</param>
        /// <typeparam name="T">The property type.</typeparam>
        IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, T>> attribute, T value);

        /// <summary>
        /// Set a item property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item property.</param>
        /// <param name="value">The value to set.</param>
        /// <typeparam name="T">The property type.</typeparam>
        IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, ISet<T>>> attribute, ISet<T> value);

        /// <summary>
        /// Set a item property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item property.</param>
        /// <param name="value">The value to set.</param>
        /// <typeparam name="T">The property type.</typeparam>
        IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, IDictionary<string, T>>> attribute, IDictionary<string, T> value);

        /// <summary>
        /// Set a item property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item property.</param>
        /// <param name="value">The value to set.</param>
        /// <typeparam name="T">The property type.</typeparam>
        IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, IList<T>>> attribute, IList<T> value);

        /// <summary>
        /// Set a item property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item property.</param>
        /// <param name="value">The value to set.</param>
        /// <typeparam name="T">The property type.</typeparam>
        IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, T>> attribute, Expression<Func<TItem, T>> value);

        /// <summary>
        /// Set a item property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item property.</param>
        /// <param name="value">The value to set.</param>
        /// <typeparam name="T">The property type.</typeparam>
        IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, ISet<T>>> attribute, Expression<Func<TItem, ISet<T>>> value);

        /// <summary>
        /// Set a item property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item property.</param>
        /// <param name="value">The value to set.</param>
        /// <typeparam name="T">The property type.</typeparam>
        IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, IDictionary<string, T>>> attribute, Expression<Func<TItem, IDictionary<string, T>>> value);

        /// <summary>
        /// Set a item property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item property.</param>
        /// <param name="value">The value to set.</param>
        /// <typeparam name="T">The property type.</typeparam>
        IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, IList<T>>> attribute, Expression<Func<TItem, IList<T>>> value);

        /// <summary>
        /// Set the value of a DynamoDB item attribute. Used for storing attributes used by local/global secondary indices.
        /// </summary>
        /// <param name="key">Name of attribute.</param>
        /// <param name="value">Value of attribute.</param>
        IDynamoTableUpdateItem<TItem> Set(string key, AttributeValue value);

        // *** `REMOVE Brand` action ***

        /// <summary>
        /// Remove a item property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item property.</param>
        /// <typeparam name="T">The property type.</typeparam>
        IDynamoTableUpdateItem<TItem> Remove<T>(Expression<Func<TItem, T>> attribute);

        /// <summary>
        /// Remove a DynamoDB item attribute. Used for removing attributes used by local/global secondary indices.
        /// </summary>
        /// <param name="key">Name of attribute.</param>
        IDynamoTableUpdateItem<TItem> Remove(string key);

        // *** `ADD Color :c` action ***

        /// <summary>
        /// Add a value to a item property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item property.</param>
        /// <param name="value">Value to add.</param>
        IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, int>> attribute, int value);

        /// <summary>
        /// Add a value to a item property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item property.</param>
        /// <param name="value">Value to add.</param>
        IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, long>> attribute, long value);

        /// <summary>
        /// Add a value to a item property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item property.</param>
        /// <param name="value">Value to add.</param>
        IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, double>> attribute, double value);

        /// <summary>
        /// Add a value to a item property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item property.</param>
        /// <param name="value">Value to add.</param>
        IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, decimal>> attribute, decimal value);

        /// <summary>
        /// Add one or more values to a item set property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item set property.</param>
        /// <param name="values">Values to add.</param>
        IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, ISet<string>>> attribute, IEnumerable<string> values);

        /// <summary>
        /// Add one or more values to a item set property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item set property.</param>
        /// <param name="values">Values to add.</param>
        IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, ISet<byte[]>>> attribute, IEnumerable<byte[]> values);

        /// <summary>
        /// Add one or more values to a item set property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item set property.</param>
        /// <param name="values">Values to add.</param>
        IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, ISet<int>>> attribute, IEnumerable<int> values);

        /// <summary>
        /// Add one or more values to a item set property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item set property.</param>
        /// <param name="values">Values to add.</param>
        IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, ISet<long>>> attribute, IEnumerable<long> values);

        /// <summary>
        /// Add one or more values to a item set property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item set property.</param>
        /// <param name="values">Values to add.</param>
        IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, ISet<double>>> attribute, IEnumerable<double> values);

        /// <summary>
        /// Add one or more values to a item set property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item set property.</param>
        /// <param name="values">Values to add.</param>
        IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, ISet<decimal>>> attribute, IEnumerable<decimal> values);

        // *** `DELETE Color :p` action ***

        /// <summary>
        /// Delete one or more values from a item set property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item set property.</param>
        /// <param name="values">Values to delete.</param>
        IDynamoTableUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<string>>> attribute, IEnumerable<string> values);

        /// <summary>
        /// Delete one or more values from a item set property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item set property.</param>
        /// <param name="values">Values to delete.</param>
        IDynamoTableUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<byte[]>>> attribute, IEnumerable<byte[]> values);

        /// <summary>
        /// Delete one or more values from a item set property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item set property.</param>
        /// <param name="values">Values to delete.</param>
        IDynamoTableUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<int>>> attribute, IEnumerable<int> values);

        /// <summary>
        /// Delete one or more values from a item set property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item set property.</param>
        /// <param name="values">Values to delete.</param>
        IDynamoTableUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<long>>> attribute, IEnumerable<long> values);

        /// <summary>
        /// Delete one or more values from a item set property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item set property.</param>
        /// <param name="values">Values to delete.</param>
        IDynamoTableUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<double>>> attribute, IEnumerable<double> values);

        /// <summary>
        /// Delete one or more values from a item set property.
        /// </summary>
        /// <param name="attribute">A lambda expression that selects the target item set property.</param>
        /// <param name="values">Values to delete.</param>
        IDynamoTableUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<decimal>>> attribute, IEnumerable<decimal> values);

        // *** Execute UpdateItem ***

        /// <summary>
        /// Execute the UpdateItem operation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>True, when successful. False, when condition is not met.</returns>
        Task<bool> ExecuteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Execute the UpdateItem operation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Updated item when found and condition is met. <c>null</c>, otherwise.</returns>
        Task<TItem?> ExecuteReturnNewItemAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Execute the UpdateItem operation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Old item when found and condition is met. <c>null</c>, otherwise.</returns>
        Task<TItem?> ExecuteReturnOldItemAsync(CancellationToken cancellationToken);

        //--- Default Methods ---

        /// <summary>
        /// Set the value of a DynamoDB item attribute. Used for storing attributes used by local/global secondary indices.
        /// </summary>
        /// <param name="key">Name of attribute.</param>
        /// <param name="value">Value of attribute.</param>
        IDynamoTableUpdateItem<TItem> Set(string key, string value)
            => Set(key, new AttributeValue(value));
    }
}
