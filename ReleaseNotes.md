# Release Notes


## 1.0.0 (2022-05-22)

Migrated assembly from [LambdaSharpTool](https://github.com/LambdaSharp/LambdaSharpTool) repository to [LambdaSharp.DynamoDB](https://github.com/LambdaSharp/LambdaSharp.DynamoDB/) repository.

### BREAKING CHANGES

* Renamed method parameters from `record` to `item` to align better with established DynamoDB terminology.
* Renamed generic parameters from `TRecord` to `TItem` to align better with established DynamoDB terminology.

### Features

* Added `WithConditionItemExists()` as shortcut for `WithCondition(item => DynamoCondition.Exists(item))`.
* Added `WithConditionItemDoesNotExist()` as shortcut for `WithCondition(item => DynamoCondition.DoesNotExist(item))`.