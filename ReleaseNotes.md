# Release Notes


## 1.0.0 (TBD)

* Miscellaneous
    * Migrated assembly from [LambdaSharpTool](https://github.com/LambdaSharp/LambdaSharpTool) repository to [LambdaSharp.DynamoDB](https://github.com/LambdaSharp/LambdaSharp.DynamoDB/) repository.

* SDK
    * Added `WithConditionExists()` as shortcut for `WithCondition(record => DynamoCondition.Exists(record))`.
    * Added `WithConditionDoesNotExist()` as shortcut for `WithCondition(record => DynamoCondition.DoesNotExist(record))`.