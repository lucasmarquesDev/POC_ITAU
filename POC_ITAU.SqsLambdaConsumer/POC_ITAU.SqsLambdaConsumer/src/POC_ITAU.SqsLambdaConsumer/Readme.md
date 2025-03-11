# AWS Lambda Simple SQS Function Project

This starter project consists of:
* Function.cs - class file containing a class with a single function handler method
* aws-lambda-tools-defaults.json - default argument settings for use with Visual Studio and command line deployment tools for AWS

You may also have a test project depending on the options selected.

The generated function handler responds to events on an Amazon SQS queue.

After deploying your function you must configure an Amazon SQS queue as an event source to trigger your Lambda function.

## Json de teste

{
  "Records": [
    {
      "messageId": "1",
      "receiptHandle": "MessageReceiptHandle",
      "body": "{\"destination\":\"123\", \"subject\":\"123\", \"body\":\"123\"}",
      "attributes": {
        "ApproximateReceiveCount": "1",
        "SentTimestamp": "1573251510774",
        "SenderId": "123456789012",
        "ApproximateFirstReceiveTimestamp": "1573251510774"
      },
      "messageAttributes": {},
      "md5OfBody": "e99a18c428cb38d5f260853678922e03",
      "eventSource": "aws:sqs",
      "eventSourceARN": "arn:aws:sqs:us-east-1:123456789012:MyQueue",
      "awsRegion": "us-east-1"
    }
  ]
}


Install Amazon.Lambda.Tools Global Tools if not already installed.
```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.
```
    dotnet tool update -g Amazon.Lambda.Tools
```

Deploy function to AWS Lambda
```
    cd "POC_ITAU.SqsLambdaConsumer/src/POC_ITAU.SqsLambdaConsumer"
    dotnet lambda deploy-function
```
