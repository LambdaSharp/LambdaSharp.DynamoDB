# Release Notes


## 1.0.0 (TBD)

* Miscellaneous
    * Migrated assembly from [LambdaSharpTool](https://github.com/LambdaSharp/LambdaSharpTool) repository to [LambdaSharp.DynamoDB](https://github.com/LambdaSharp/LambdaSharp.DynamoDB/) repository.

* SDK
    * Added `WithConditionItemExists()` as shortcut for `WithCondition(item => DynamoCondition.Exists(item))`.
    * Added `WithConditionItemDoesNotExist()` as shortcut for `WithCondition(item => DynamoCondition.DoesNotExist(item))`.