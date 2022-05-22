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
using LambdaSharp.DynamoDB.Native.Options;
using LambdaSharp.DynamoDB.Serialization;

namespace LambdaSharp.DynamoDB.Native {

    /// <summary>
    /// Defines the access options for <see cref="DynamoTable"/>.
    /// </summary>
    public class DynamoTableOptions {

        //--- TYpes ---

        //--- Properties ---

        /// <summary>
        /// Get/set serialization options for items.
        /// </summary>
        public DynamoSerializerOptions SerializerOptions { get; set; } = new DynamoSerializerOptions();

        /// <summary>
        /// Get/set expected type namespace for stored item type names.
        /// </summary>
        public string? ExpectedTypeNamespace { get; set; }

        // TODO (2021-07-11, bjorg): not ready to be public
        internal List<ItemType> ItemTypes { get; set; } = new List<ItemType>();

        //--- Methods ---
        internal string GetShortTypeName(Type type) {
            if(type is null) {
                throw new ArgumentNullException(nameof(type));
            }
            var result = type.FullName ?? throw new ArgumentException("missing type name", nameof(type));

            // check if a custom short type name is defined
            var itemType = ItemTypes.FirstOrDefault(itemType => itemType.Type == type);
            if(!(itemType is null) && !(itemType.ShortTypeName is null)) {
                return itemType.ShortTypeName;
            }

            // check if the type name has an expected prefix
            if(
                !string.IsNullOrEmpty(ExpectedTypeNamespace)
                && result.StartsWith(ExpectedTypeNamespace, StringComparison.InvariantCulture)
            ) {
                result = result.Substring(ExpectedTypeNamespace.Length);
            }
            return result;
        }

        internal string GetFullTypeName(string typeName) {

            // check if a type name corresponds to a custom short type name
            var itemType = ItemTypes.FirstOrDefault(itemType => itemType.ShortTypeName == typeName);
            if(!(itemType is null) && !(itemType.ShortTypeName is null)) {
                return itemType.FullTypeName;
            }

            // check if the type name shorted an expected prefix
            if(typeName.StartsWith(".")) {
                typeName = $"{ExpectedTypeNamespace}{typeName}";
            }
            return typeName;
        }

        internal Type? GetItemType(string typeName) {
            var fullTypeName = GetFullTypeName(typeName);

            // find type that matches full typename
            return ItemTypes.FirstOrDefault(itemType => itemType.Type.FullName == fullTypeName)?.Type;
        }
    }
}
