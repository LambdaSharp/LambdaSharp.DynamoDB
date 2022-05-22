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

namespace Sample.DynamoDBNative.DataAccess;

using Amazon.DynamoDBv2;
using LambdaSharp.DynamoDB.Native;
using Sample.DynamoDBNative.DataAccess.Models;

public class ThriftBooksDataAccessClient : IThriftBooksDataAccess {

    //--- Class Fields ---
    public static readonly DynamoTableOptions TableOptions = new DynamoTableOptions {
        ExpectedTypeNamespace = "Sample.DynamoDBNative.DataAccess.Models"
    };

    //--- Constructors ---
    public ThriftBooksDataAccessClient(string tableName, IAmazonDynamoDB? dynamoClient = null)
        => Table = new DynamoTable(tableName, dynamoClient, TableOptions);

    //--- Properties ---
    protected IDynamoTable Table { get; }

    //--- Methods ---
    public async Task CreateCustomerAsync(CustomerRecord customer, CancellationToken cancellationToken) {
        if(customer.Addresses is null) {

            // initialize an empty dictionary for addresses so we can add to it easily later
            customer.Addresses = new Dictionary<string, AddressRecord>();
        }
        var customerEmail = new CustomerEmailRecord {
            Username = customer.Username,
            EmailAddress = customer.EmailAddress
        };

        // check if both items can be written
        var success = await Table.TransactWriteItems()
            .BeginPutItem(customer.GetPrimaryKey(), customer)
                .WithConditionItemDoesNotExist()
            .End()
            .BeginPutItem(customerEmail.GetPrimaryKey(), customerEmail)
                .WithConditionItemDoesNotExist()
            .End()
        .TryExecuteAsync();
        if(!success) {
            throw new Exception("operation failed");
        }
    }

    public Task AddOrUpdateAddressAsync(string customerUsername, AddressRecord address, CancellationToken cancellationToken) {
        return Table.UpdateItem(DataModel.CustomerRecordPrimaryKey(customerUsername))
            .Set(item => item.Addresses![address.Label!], address)
            .ExecuteAsync(cancellationToken);
    }

    public async Task<(CustomerRecord Customer, IEnumerable<OrderRecord> Orders)> GetCustomerWithMostRecentOrdersAsync(string customerUsername, int limit, CancellationToken cancellationToken) {

        // query all items under the customer name, which include the order items as well
        var items = await Table.Query(DataModel.SelectCustomerAndOrders(customerUsername), limit: 11, scanIndexForward: false)
            .ExecuteAsync(cancellationToken: cancellationToken);

        // split returned items by type
        return (Customer: items.OfType<CustomerRecord>().Single(), Orders: items.OfType<OrderRecord>().ToList());
    }

    public async Task SaveOrderAsync(OrderRecord order, IEnumerable<OrderItemRecord> orderItems, CancellationToken cancellationToken) {

        // store all order items using batch operations
        while(orderItems.Any()) {
            var batch = Table.BatchWriteItems();

            // BatchWriteItem can take up to 25 operations
            foreach(var orderItem in orderItems.Take(25)) {
                batch.BeginPutItem(orderItem.GetPrimaryKey(), orderItem)
                    .Set("GSI1PK", string.Format(DataModel.ORDER_ITEM_GSI1_PK_PATTERN, orderItem.OrderId))
                    .Set("GSI1SK", string.Format(DataModel.ORDER_ITEM_GSI1_SK_PATTERN, orderItem.OrderId, orderItem.ItemId))
                .End();
            }
            await batch.ExecuteAsync();

            // skip the order items that we stored
            orderItems = orderItems.Skip(25);
        }

        // store order
        var success = await Table.PutItem(order.GetPrimaryKey(), order)
            .WithConditionItemDoesNotExist()
            .Set("GSI1PK", string.Format(DataModel.ORDER_GSI1_PK_PATTERN, order.OrderId))
            .Set("GSI1SK", string.Format(DataModel.ORDER_GSI1_SK_PATTERN, order.OrderId))
            .Set("LSI1SK", string.Format(DataModel.ORDER_LSI1_SK_PATTERN, order.OrderId, order.Status))
            .ExecuteAsync(cancellationToken);
        if(!success) {
            throw new Exception("unable to store order");
        }
    }

    public async Task UpdateOrderAsync(string orderId, OrderStatus orderStatus, CancellationToken cancellationToken) {

        // resolve order ID to primary key by querying the global secondary index
        var order = (await Table.Query(DataModel.SelectOrders(orderId))
            .Get(item => item.CustomerUsername)
            .ExecuteAsync(cancellationToken)
        ).FirstOrDefault();
        ArgumentAssertException.Assert(order is not null);
        ArgumentAssertException.Assert(order.CustomerUsername is not null);
        ArgumentAssertException.Assert(order.OrderId is not null);

        // update order with state
        await Table.UpdateItem(DataModel.OrderRecordPrimaryKey(order.CustomerUsername, order.OrderId))
            .Set(item => item.Status, orderStatus)
            .ExecuteAsync(cancellationToken);
    }

    public async Task<(OrderRecord Order, IEnumerable<OrderItemRecord> Items)> GetOrderWithOrderItemsAsync(string orderId, CancellationToken cancellationToken) {

        // query all items under the order ID, which include the order item items as well
        var items = await Table.Query(DataModel.SelectOrderAndOrderItems(orderId))
            .ExecuteAsync(cancellationToken);

        // split returned items by type
        return (Order: items.OfType<OrderRecord>().Single(), Items: items.OfType<OrderItemRecord>().ToList());
    }
}
