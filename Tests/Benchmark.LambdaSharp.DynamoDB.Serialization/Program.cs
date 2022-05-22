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

using System.Text;
using Benchmark.LambdaSharp.DynamoDB.Serialization.Internal;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LambdaSharp.DynamoDB.Serialization;

var summary = BenchmarkRunner.Run<DynamoSerialization>();

public class DynamoSerialization {

    //--- Types ---
    public class MyCustomType {

        //--- Properties ---

        [DynamoPropertyIgnore]
        public string? IgnoreText { get; set; }

        [DynamoPropertyName("OtherName")]
        public string? CustomName { get; set; }
    }

    //--- Methods ---

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void Serialize_anonymous_class() {
        var value = new {
            Active = true,
            Binary = Encoding.UTF8.GetBytes("Bye"),
            Name = "John Doe",
            Age = 42,
            List = new object[] {
                new {
                    Message = "Hello"
                },
                "World!"
            },
            Dictionary = new Dictionary<string, object> {
                ["Key"] = "Value"
            },
            StringSet = new[] { "Red", "Blue" }.ToHashSet(),
            NumberSet = new[] { 123, 456 }.ToHashSet(),
            BinarySet = new[] { Encoding.UTF8.GetBytes("Good"), Encoding.UTF8.GetBytes("Day") }.ToHashSet()
        };
        for(var i = 0; i < 10_000; ++i) {
            DynamoSerializer.Serialize(value);
        }
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void Serialize_with_custom_converter() {

        // arrange
        var value = new {
            TimeSpan = TimeSpan.FromSeconds(789)
        };
        var options = new DynamoSerializerOptions {
            Converters = {
                new DynamoTimeSpanConverter()
            }
        };
        for(var i = 0; i < 10_000; ++i) {
            DynamoSerializer.Serialize(value, options);
        }
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void Serialize_empty_string() {
        var value = new {
            Text = ""
        };
        for(var i = 0; i < 10_000; ++i) {
            DynamoSerializer.Serialize(value);
        }
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void Serialize_empty_string_set() {
        var value = new {
            StringSet = new HashSet<string>()
        };
        for(var i = 0; i < 10_000; ++i) {
            DynamoSerializer.Serialize(value);
        }
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void Serialize_empty_int_set() {
        var value = new {
            IntSet = new HashSet<int>()
        };
        for(var i = 0; i < 10_000; ++i) {
            DynamoSerializer.Serialize(value);
        }
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void Serialize_empty_long_set() {
        var value = new {
            LongSet = new HashSet<long>()
        };
        for(var i = 0; i < 10_000; ++i) {
            DynamoSerializer.Serialize(value);
        }
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void Serialize_empty_double_set() {
        var value = new {
            DoubleSet = new HashSet<double>()
        };
        for(var i = 0; i < 10_000; ++i) {
            DynamoSerializer.Serialize(value);
        }
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void Serialize_empty_decimal_set() {
        var value = new {
            DecimalSet = new HashSet<decimal>()
        };
        for(var i = 0; i < 10_000; ++i) {
            DynamoSerializer.Serialize(value);
        }
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void Serialize_empty_binary_set() {
        var value = new {
            BinarySet = new HashSet<byte[]>()
        };
        for(var i = 0; i < 10_000; ++i) {
            DynamoSerializer.Serialize(value);
        }
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void Serialize_custom_name_property() {
        var value = new MyCustomType {
            CustomName = "Hello"
        };
        for(var i = 0; i < 10_000; ++i) {
            DynamoSerializer.Serialize(value);
        }
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void Serialize_ignore_property() {
        var value = new MyCustomType {
            IgnoreText = "World"
        };
        for(var i = 0; i < 10_000; ++i) {
            DynamoSerializer.Serialize(value);
        }
    }
}