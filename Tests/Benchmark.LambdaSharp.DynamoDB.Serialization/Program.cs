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
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LambdaSharp.DynamoDB.Serialization;

var summary = BenchmarkRunner.Run<DynamoSerialization>();

public class DynamoSerialization {

    private object? value;

    [GlobalSetup]
    public void GlobalSetup() {
        value = new {
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
    }

    [Benchmark(OperationsPerInvoke = 10_000)]
    public void Serialize_anonymous_class() {
        for(var i = 0; i < 10_000; ++i) {
            DynamoSerializer.Serialize(value);
        }
    }
}