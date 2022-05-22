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
using Amazon.DynamoDBv2.Model;
using LambdaSharp.DynamoDB.Native.Internal;

namespace LambdaSharp.DynamoDB.Native.Operations.Internal {

    internal sealed class DynamoTableTransactWriteItems : IDynamoTableTransactWriteItems {

        //--- Fields ---
        private readonly DynamoTable _table;
        private readonly TransactWriteItemsRequest _request;

        //--- Constructors ---
        public DynamoTableTransactWriteItems(DynamoTable table, TransactWriteItemsRequest request) {
            _table = table ?? throw new ArgumentNullException(nameof(table));
            _request = request ?? throw new ArgumentNullException(nameof(request));
        }

        //--- Properties ---
        public DynamoTable Table => _table;

        //--- Methods ---
        public IDynamoTableTransactWriteItemsConditionCheck<TItem> BeginConditionCheck<TItem>(DynamoPrimaryKey<TItem> primaryKey) where TItem : class {
            var transactWriteItem = new TransactWriteItem {
                ConditionCheck = new ConditionCheck {
                    TableName = _table.TableName,
                    Key = new Dictionary<string, AttributeValue> {
                        [primaryKey.PKName] = new AttributeValue(primaryKey.PKValue),
                        [primaryKey.SKName] = new AttributeValue(primaryKey.SKValue)
                    }
                }
            };
            _request.TransactItems.Add(transactWriteItem);
            var converter = new DynamoRequestConverter(transactWriteItem.ConditionCheck.ExpressionAttributeNames, transactWriteItem.ConditionCheck.ExpressionAttributeValues, _table.SerializerOptions);
            return new DynamoTableTransactWriteItemsConditionCheck<TItem>(this, transactWriteItem.ConditionCheck, converter);
        }

        public IDynamoTableTransactWriteItemsDeleteItem<TItem> BeginDeleteItem<TItem>(DynamoPrimaryKey<TItem> primaryKey) where TItem : class {
            var transactWriteItem = new TransactWriteItem {
                Delete = new Delete {
                    TableName = _table.TableName,
                    Key = new Dictionary<string, AttributeValue> {
                        [primaryKey.PKName] = new AttributeValue(primaryKey.PKValue),
                        [primaryKey.SKName] = new AttributeValue(primaryKey.SKValue)
                    }
                }
            };
            _request.TransactItems.Add(transactWriteItem);
            var converter = new DynamoRequestConverter(transactWriteItem.Delete.ExpressionAttributeNames, transactWriteItem.Delete.ExpressionAttributeValues, _table.SerializerOptions);
            return new DynamoTableTransactWriteItemsDeleteItem<TItem>(this, transactWriteItem.Delete, converter);
        }

        public IDynamoTableTransactWriteItemsPutItem<TItem> BeginPutItem<TItem>(DynamoPrimaryKey<TItem> primaryKey, TItem item) where TItem : class {
            var transactWriteItem = new TransactWriteItem {
                Put = new Put {
                    TableName = _table.TableName,
                    Item = _table.SerializeItem(item, primaryKey),
                }
            };
            _request.TransactItems.Add(transactWriteItem);
            var converter = new DynamoRequestConverter(transactWriteItem.Put.ExpressionAttributeNames, transactWriteItem.Put.ExpressionAttributeValues, _table.SerializerOptions);
            return new DynamoTableTransactWriteItemsPutItem<TItem>(this, transactWriteItem.Put, converter);
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> BeginUpdateItem<TItem>(DynamoPrimaryKey<TItem> primaryKey) where TItem : class {
            var transactWriteItem = new TransactWriteItem {
                Update = new Update {
                    TableName = _table.TableName,
                    Key = new Dictionary<string, AttributeValue> {
                        [primaryKey.PKName] = new AttributeValue(primaryKey.PKValue),
                        [primaryKey.SKName] = new AttributeValue(primaryKey.SKValue)
                    }
                }
            };
            _request.TransactItems.Add(transactWriteItem);
            var converter = new DynamoRequestConverter(transactWriteItem.Update.ExpressionAttributeNames, transactWriteItem.Update.ExpressionAttributeValues, _table.SerializerOptions);
            return new DynamoTableTransactWriteItemsUpdateItem<TItem>(this, transactWriteItem.Update, converter);
        }

        public async Task<bool> TryExecuteAsync(CancellationToken cancellationToken = default) {
            if(!_request.TransactItems.Any()) {
                throw new InvalidOperationException("BatchWriteItems cannot be empty");
            }
            if(_request.TransactItems.Count > 25) {
                throw new InvalidOperationException("BatchWriteItems too many operations");
            }

            // perform transaction
            try {
                await Table.DynamoClient.TransactWriteItemsAsync(_request, cancellationToken);
                return true;
            } catch(TransactionCanceledException) {

                // transaction failed
                return false;
            }
        }
    }

    internal sealed class DynamoTableTransactWriteItemsConditionCheck<TItem> : IDynamoTableTransactWriteItemsConditionCheck<TItem>
        where TItem : class
    {

        //--- Fields ---
        private readonly DynamoTableTransactWriteItems _parent;
        private readonly ConditionCheck _conditionCheck;
        private readonly DynamoRequestConverter _converter;

        //--- Constructors ---
        public DynamoTableTransactWriteItemsConditionCheck(DynamoTableTransactWriteItems parent, ConditionCheck conditionCheck, DynamoRequestConverter converter) {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _conditionCheck = conditionCheck ?? throw new ArgumentNullException(nameof(conditionCheck));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        //--- Methods ---
        public IDynamoTableTransactWriteItemsConditionCheck<TItem> WithCondition(Expression<Func<TItem, bool>> condition) {
            _converter.AddCondition(condition.Body);
            return this;
        }

        public IDynamoTableTransactWriteItems End() {
            _conditionCheck.ConditionExpression = _converter.ConvertConditions(_parent.Table.Options);
            return _parent;
        }
    }

    internal sealed class DynamoTableTransactWriteItemsDeleteItem<TItem> : IDynamoTableTransactWriteItemsDeleteItem<TItem>
        where TItem : class
    {

        //--- Fields ---
        private readonly DynamoTableTransactWriteItems _parent;
        private readonly Delete _delete;
        private readonly DynamoRequestConverter _converter;

        //--- Constructors ---
        public DynamoTableTransactWriteItemsDeleteItem(DynamoTableTransactWriteItems parent, Delete delete, DynamoRequestConverter converter) {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _delete = delete ?? throw new ArgumentNullException(nameof(delete));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        //--- Methods ---
        public IDynamoTableTransactWriteItemsDeleteItem<TItem> WithCondition(Expression<Func<TItem, bool>> condition) {
            _converter.AddCondition(condition.Body);
            return this;
        }

        public IDynamoTableTransactWriteItems End() {
            _delete.ConditionExpression = _converter.ConvertConditions(_parent.Table.Options);
            return _parent;
        }
    }

    internal sealed class DynamoTableTransactWriteItemsPutItem<TItem> : IDynamoTableTransactWriteItemsPutItem<TItem>
        where TItem : class
    {

        //--- Fields ---
        private readonly DynamoTableTransactWriteItems _parent;
        private readonly Put _put;
        private readonly DynamoRequestConverter _converter;

        //--- Constructors ---
        public DynamoTableTransactWriteItemsPutItem(DynamoTableTransactWriteItems parent, Put put, DynamoRequestConverter converter) {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _put = put ?? throw new ArgumentNullException(nameof(put));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        //--- Methods ---
        public IDynamoTableTransactWriteItemsPutItem<TItem> WithCondition(Expression<Func<TItem, bool>> condition) {
            _converter.AddCondition(condition.Body);
            return this;
        }

        public IDynamoTableTransactWriteItemsPutItem<TItem> Set(string key, AttributeValue value) {
            _put.Item[key] = value;
            return this;
        }

        public IDynamoTableTransactWriteItems End() {
            _put.ConditionExpression = _converter.ConvertConditions(_parent.Table.Options);
            return _parent;
        }
    }

    internal sealed class DynamoTableTransactWriteItemsUpdateItem<TItem> : IDynamoTableTransactWriteItemsUpdateItem<TItem>
        where TItem : class
    {

        //--- Fields ---
        private readonly DynamoTableTransactWriteItems _parent;
        private readonly Update _update;
        private readonly DynamoRequestConverter _converter;
        private readonly List<string> _setOperations = new List<string>();
        private readonly List<string> _removeOperations = new List<string>();
        private readonly List<string> _addOperations = new List<string>();
        private readonly List<string> _deleteOperations = new List<string>();

        //--- Constructors ---
        public DynamoTableTransactWriteItemsUpdateItem(DynamoTableTransactWriteItems parent, Update update, DynamoRequestConverter converter) {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _update = update ?? throw new ArgumentNullException(nameof(update));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        //--- Methods ---

        #region *** SET Actions ***
        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Set<T>(Expression<Func<TItem, T>> attribute, T value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.GetExpressionValueName(value));

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Set<T>(Expression<Func<TItem, ISet<T>>> attribute, ISet<T> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.GetExpressionValueName(value));

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Set<T>(Expression<Func<TItem, IDictionary<string, T>>> attribute, IDictionary<string, T> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.GetExpressionValueName(value));

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Set<T>(Expression<Func<TItem, IList<T>>> attribute, IList<T> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.GetExpressionValueName(value));

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Set<T>(Expression<Func<TItem, T>> attribute, Expression<Func<TItem, T>> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.ParseValue(value.Body));

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Set<T>(Expression<Func<TItem, ISet<T>>> attribute, Expression<Func<TItem, ISet<T>>> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.ParseValue(value.Body));

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Set<T>(Expression<Func<TItem, IDictionary<string, T>>> attribute, Expression<Func<TItem, IDictionary<string, T>>> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.ParseValue(value.Body));

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Set<T>(Expression<Func<TItem, IList<T>>> attribute, Expression<Func<TItem, IList<T>>> value)
            => SetAttributePathExpression(_converter.ParseAttributePath(attribute.Body), _converter.ParseValue(value.Body));

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Set(string attribute, AttributeValue value)
            => SetAttributePathExpression(_converter.GetAttributeName(attribute), _converter.GetExpressionValueName(value));

        private  IDynamoTableTransactWriteItemsUpdateItem<TItem> SetAttributePathExpression(string attributePath, string attributeValueExpression) {
            _setOperations.Add($"{attributePath} = {attributeValueExpression}");
            return this;
        }
        #endregion

        #region *** REMOVE Actions ***
        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Remove<T>(Expression<Func<TItem, T>> attribute) {
            _removeOperations.Add(_converter.ParseAttributePath(attribute.Body));
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Remove(string key) {
            _removeOperations.Add(_converter.GetAttributeName(key));
            return this;
        }
        #endregion

        #region *** ADD Actions ***
        public IDynamoTableTransactWriteItemsUpdateItem<TItem> WithCondition(Expression<Func<TItem, bool>> condition) {
            _converter.AddCondition(condition.Body);
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Add(Expression<Func<TItem, int>> attribute, int value) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(value);
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Add(Expression<Func<TItem, long>> attribute, long value) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(value);
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Add(Expression<Func<TItem, double>> attribute, double value) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(value);
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Add(Expression<Func<TItem, decimal>> attribute, decimal value) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(value);
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Add(Expression<Func<TItem, ISet<string>>> attribute, IEnumerable<string> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Add(Expression<Func<TItem, ISet<byte[]>>> attribute, IEnumerable<byte[]> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Add(Expression<Func<TItem, ISet<int>>> attribute, IEnumerable<int> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Add(Expression<Func<TItem, ISet<long>>> attribute, IEnumerable<long> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Add(Expression<Func<TItem, ISet<double>>> attribute, IEnumerable<double> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Add(Expression<Func<TItem, ISet<decimal>>> attribute, IEnumerable<decimal> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }
        #endregion

        #region *** DELETE Actions ***
        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<string>>> attribute, IEnumerable<string> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<byte[]>>> attribute, IEnumerable<byte[]> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<int>>> attribute, IEnumerable<int> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<long>>> attribute, IEnumerable<long> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<double>>> attribute, IEnumerable<double> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }

        public IDynamoTableTransactWriteItemsUpdateItem<TItem> Delete(Expression<Func<TItem, ISet<decimal>>> attribute, IEnumerable<decimal> values) {
            var path = _converter.ParseAttributePath(attribute.Body);
            var operand = _converter.GetExpressionValueName(values.ToHashSet());
            _addOperations.Add($"{path} {operand}");
            return this;
        }
        #endregion

        public IDynamoTableTransactWriteItems End() {

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
            _update.ConditionExpression = _converter.ConvertConditions(_parent.Table.Options);
            _update.UpdateExpression = string.Join(" ", result);
            return _parent;
        }
    }
}