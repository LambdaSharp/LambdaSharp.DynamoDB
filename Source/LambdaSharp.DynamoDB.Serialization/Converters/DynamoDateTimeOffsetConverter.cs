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
using System.Globalization;
using Amazon.DynamoDBv2.Model;

namespace LambdaSharp.DynamoDB.Serialization.Converters {

    /// <summary>
    /// The <see cref="DynamoDateTimeOffsetConverter"/> class is used to convert <c>DateTimeOffset</c> and <c>DateTimeOffset?</c> to/from a DynamoDB attribute value.
    /// </summary>
    public class DynamoDateTimeOffsetConverter : ADynamoAttributeConverter {

        //--- Class Fields ---

        /// <summary>
        /// The <see cref="Instance"/> class field exposes a reusable instance of the class.
        /// </summary>
        public static readonly DynamoDateTimeOffsetConverter Instance = new DynamoDateTimeOffsetConverter();

        //--- Methods ---

        /// <summary>
        /// The <see cref="CanConvert(Type)"/> method checks if this converter can handle the presented type.
        /// </summary>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <returns><c>true</c> if the converter can handle the type; otherwise, <c>false</c></returns>
        public override bool CanConvert(Type typeToConvert) => (typeToConvert == typeof(DateTimeOffset)) || (typeToConvert == typeof(DateTimeOffset?));

                /// <summary>
        /// The <see cref="ToAttributeValue(object,Type,DynamoSerializerOptions)"/> method converts an instance to a DynamoDB attribute value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The source value type.</param>
        /// <param name="options">The serialization options.</param>
        /// <returns>A DynamoDB attribute value, or <c>null</c> if the instance state cannot be represented in DynamoDB.</returns>
        public override AttributeValue ToAttributeValue(object value, Type targetType, DynamoSerializerOptions options)
            => new AttributeValue {
                N = ((DateTimeOffset)value).ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)
            };

        /// <summary>
        /// The <see cref="FromNumber(string,Type,DynamoSerializerOptions)"/> method converts a DynamoDB N attribute value to the type of the converter.
        /// </summary>
        /// <param name="value">The DynamoDB attribute value to convert.</param>
        /// <param name="targetType">The expected return type.</param>
        /// <param name="options">The deserialization options.</param>
        /// <returns>An instance of type <paramref name="targetType"/>.</returns>
        public override object FromNumber(string value, Type targetType, DynamoSerializerOptions options)
            => DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(value, CultureInfo.InvariantCulture));
    }
}