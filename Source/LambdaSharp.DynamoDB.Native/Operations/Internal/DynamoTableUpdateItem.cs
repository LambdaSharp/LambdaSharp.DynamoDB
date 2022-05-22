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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using LambdaSharp.DynamoDB.Native.Internal;
using LambdaSharp.DynamoDB.Serialization.Utility;

namespace LambdaSharp.DynamoDB.Native.Operations.Internal {

    internal sealed class DynamoTableUpdateItem<TItem> : IDynamoTableUpdateItem<TItem>
        where TItem : class
    {

        //--- Fields ---
        private readonly DynamoTable _table;
        private readonly UpdateItemRequest _request;
        private readonly DynamoRequestConverter _converter;
        private readonly List<string> _setOperations = new List<string>();
        private readonly List<string> _removeOperations = new List<string>();
        private readonly List<string> _addOperations = new List<string>();
        private readonly List<string> _deleteOperations = new List<string>();

        //--- Constructors ---
        public DynamoTableUpdateItem(DynamoTable table, UpdateItemRequest request) {
            _table = table ?? throw new ArgumentNullException(nameof(table));
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _converter = new DynamoRequestConverter(_request.ExpressionAttributeNames, _request.ExpressionAttributeValues, _table.SerializerOptions);
        }

        //--- Methods ---
        public IDynamoTableUpdateItem<TItem> WithCondition(Expression<Func<TItem, bool>> condition) {
            _converter.AddCondition(condition.Body);
            return this;
        }

        #region *** SET Actions ***
        public IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, T>> attribute, T value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.GetExpressionValueName(value));

        public IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, ISet<T>>> attribute, ISet<T> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.GetExpressionValueName(value));

        public IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, IDictionary<string, T>>> attribute, IDictionary<string, T> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.GetExpressionValueName(value));

        public IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, IList<T>>> attribute, IList<T> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.GetExpressionValueName(value));

        public IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, T>> attribute, Expression<Func<TItem, T>> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.ParseValue(value.Body));

        public IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, ISet<T>>> attribute, Expression<Func<TItem, ISet<T>>> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.ParseValue(value.Body));

        public IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, IDictionary<string, T>>> attribute, Expression<Func<TItem, IDictionary<string, T>>> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.ParseValue(value.Body));

        public IDynamoTableUpdateItem<TItem> Set<T>(Expression<Func<TItem, IList<T>>> attribute, Expression<Func<TItem, IList<T>>> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.ParseValue(value.Body));

        public IDynamoTableUpdateItem<TItem> Set(string attribute, AttributeValue value)
            => SetAttributePathExpression(_converter.GetAttributeName(attribute), _converter.GetExpressionValueName(value));

        private  IDynamoTableUpdateItem<TItem> SetAttributePathExpression(string attributePath, string attributeValueExpression) {
            _setOperations.Add($"{attributePath} = {attributeValueExpression}");
            return this;
        }
        #endregion

        #region *** REMOVE Actions ***
        public IDynamoTableUpdateItem<TItem> Remove<T>(Expression<Func<TItem, T>> attribute) {
            _removeOperations.Add(_converter.ParseAttributePath(attribute.Body));
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Remove(string key) {
            _removeOperations.Add(_converter.GetAttributeName(key));
            return this;
        }
        #endregion

        #region *** ADD Actions ***
        public IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, int>> attribute, int value) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(value);
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, long>> attribute, long value) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(value);
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, double>> attribute, double value) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(value);
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, decimal>> attribute, decimal value) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(value);
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, ISet<string>>> attribute, IEnumerable<string> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, ISet<byte[]>>> attribute, IEnumerable<byte[]> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet(ByteArrayEqualityComparer.Instance));
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, ISet<int>>> attribute, IEnumerable<int> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, ISet<long>>> attribute, IEnumerable<long> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, ISet<double>>> attribute, IEnumerable<double> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Add(Expression<Func<TItem, ISet<decimal>>> attribute, IEnumerable<decimal> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }
        #endregion

        #region *** DELETE Actions ***
        public IDynamoTableUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<string>>> attribute, IEnumerable<string> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _deleteOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<byte[]>>> attribute, IEnumerable<byte[]> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _deleteOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<int>>> attribute, IEnumerable<int> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _deleteOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<long>>> attribute, IEnumerable<long> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _deleteOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<double>>> attribute, IEnumerable<double> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _deleteOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<decimal>>> attribute, IEnumerable<decimal> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _deleteOperations.Add($"{path} {operand}");
            return this;
        }
        #endregion

        public async Task<bool> ExecuteAsync(CancellationToken cancellationToken) {
            PrepareRequest();
            try {
                await _table.DynamoClient.UpdateItemAsync(_request);
                return true;
            } catch(ConditionalCheckFailedException) {
                return false;
            }
        }

        public async Task<TItem?> ExecuteReturnNewItemAsync(CancellationToken cancellationToken) {
            PrepareRequest();
            _request.ReturnValues = ReturnValue.ALL_NEW;
            try {
                var response = await _table.DynamoClient.UpdateItemAsync(_request);
                return _table.DeserializeItem<TItem>(response.Attributes);
            } catch(ConditionalCheckFailedException) {
                return default(TItem);
            }
        }

        public async Task<TItem?> ExecuteReturnOldItemAsync(CancellationToken cancellationToken) {
            PrepareRequest();
            _request.ReturnValues = ReturnValue.ALL_OLD;
            try {
                var response = await _table.DynamoClient.UpdateItemAsync(_request);
                return _table.DeserializeItem<TItem>(response.Attributes);
            } catch(ConditionalCheckFailedException) {
                return default(TItem);
            }
        }

        private void PrepareRequest() {

            // combine update actions
            var result = new List<string>();
            var modifiedAttributeName = _converter.GetAttributeName("_m");
            var modifiedAttributeValue = _converter.GetExpressionValueName(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            result.Add($"SET {string.Join(", ", _setOperations.Append($"{modifiedAttributeName} = {modifiedAttributeValue}"))}");
            if(_removeOperations.Any()) {
                result.Add($"REMOVE {string.Join(", ", _removeOperations)}");
            }
            if(_addOperations.Any()) {
                result.Add($"ADD {string.Join(", ", _addOperations)}");
            }
            if(_deleteOperations.Any()) {
                result.Add($"DELETE {string.Join(", ", _deleteOperations)}");
            }

            // update request
            _request.ConditionExpression = _converter.ConvertConditions(_table.Options);
            _request.UpdateExpression = string.Join(" ", result);
        }
    }
}
