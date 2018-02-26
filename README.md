# crypto-pricewatch

[![serverless](https://dl.dropboxusercontent.com/s/d6opqwym91k0roz/serverless_badge_v3.svg)](http://www.serverless.com)

Crypto pricewatch slackbot, running on AWS Lambda using the Serverless framework.

## Credits
Inspired by Serverless aws-csharp 1.0 template

Inspired by [PageUpPeopleOrg](https://github.com/PageUpPeopleOrg/serverless-microservice-bootstrap) Bootstrap Template
HealthCheck Endpoint / Build Scripts / ReadMe

BuiltWith: [DotNetServerlessBootstrap](https://github.com/jdonohoo/serverless-aws-aspnetcore2) -- written by me.

Need help with the cloud? Check us out over at [Observian](https://www.observian.com).



## Intro
Sick of hitting refresh on CoinMarketCap.com? Yeah, me too.
This code will post something like this : `prices` to your slack channel every hour.
```
=== Crypto-PriceWatch ===
BTC : 10207.15
ETH : 859.7
LTC : 218.57
XRP : 0.9181
VTC : 4
XVG : 0.06059
CRC : 2.48
=========================
```

## Getting Started

### Windows
Install [Chocolatey](https://chocolatey.org/install)

Install Node
```
choco install nodejs.install
```

Install curl
```
choco install curl
```

Install [Serverless Framework](http://www.serverless.com)
```
npm install serverless -g
```

Configure the aws-cli if you haven't already. [aws-cli](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-getting-started.html)

Install dotnet core on your machine. Instructions can be found at [dotnet website](https://www.microsoft.com/net/download)

## Build

Windows via powershell
```
build.ps1
```

Linux / Mac via bash
```
./build.sh
```

## Testing CommandLine
```
dotnet test .\Tests
```

#### Output:
```
Build started, please wait...
Build completed.

Test run for C:\projects\crypto-pricewatch\Tests\bin\Debug\netcoreapp2.0\Tests.dll(.NETCoreApp,Version=v2.0)
Microsoft (R) Test Execution Command Line Tool Version 15.5.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
[xUnit.net 00:00:00.4202457]   Discovering: Tests
[xUnit.net 00:00:00.4809961]   Discovered:  Tests
[xUnit.net 00:00:00.4867238]   Starting:    Tests
[xUnit.net 00:00:00.6334110]   Finished:    Tests

Total tests: 2. Passed: 2. Failed: 0. Skipped: 0.
Test Run Successful.
Test execution time: 1.3748 Seconds
```

## Deploying CommandLine
```
serverless deploy
```

#### Output:
```
Serverless: Packaging service...
Serverless: Uploading CloudFormation file to S3...
Serverless: Uploading artifacts...
Serverless: Validating template...
Serverless: Creating Stack...
Serverless: Checking Stack create progress...
.........................................
Serverless: Stack create finished...
Service Information
service: crypto-pricewatch
stage: dev
region: us-east-1
stack: crypto-pricewatch-dev
api keys:
  None
endpoints:
  GET - https://xxxxxxxxxx.execute-api.us-east-1.amazonaws.com/dev/healthcheck
functions:
  hello: crypto-pricewatch-dev-hello
  healthcheck: crypto-pricewatch-dev-healthcheck
```

### Testing via HealthCheck Endpoint

```
curl https://xxxxxxxxxx.execute-api.us-east-1.amazonaws.com/dev/healthcheck
```

#### Output:
```
StatusCode        : 200
StatusDescription : OK
Content           : OK
RawContent        : HTTP/1.1 200 OK
                    Connection: keep-alive
                    x-amzn-RequestId: 5d9f56d7-142c-11e8-8aba-3d2ffd5e6280
                    Context-Type: text/html
                    X-Amzn-Trace-Id: sampled=0;root=1-5a88a34d-1315491996eff1c9a983409f
                    X-Cache: ...
Forms             : {}
Headers           : {[Connection, keep-alive], [x-amzn-RequestId, 5d9f56d7-142c-11e8-8aba-3d2ffd5e6280], [Context-Type, text/html], [X-Amzn-Trace-Id, sampled=0;root=1-5a88a34d-1315491996eff1c9a983409f]...}
Images            : {}
InputFields       : {}
Links             : {}
ParsedHtml        : mshtml.HTMLDocumentClass
RawContentLength  : 2
```



## Configuration SSM
If you aren't familar with AWS SSM Parameter Store start [here](https://aws.amazon.com/blogs/mt/organize-parameters-by-hierarchy-tags-or-amazon-cloudwatch-events-with-amazon-ec2-systems-manager-parameter-store/)

### How to get there:
```
AWSConsole > EC2 > Parameter Store (Bottom left corner scroll down)
```
All functions are deployed with the environment variable: parameterPath
Because of this block in Serverless.yml:
```
  environment:
	parameterPath: /${self:provider.stage}/${self:service}/settings
```
### Lambda Role
I make a service role for Lambda called `micro-service` with the following policy: (SSM ReadOnly)
```
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "ssm:Describe*",
                "ssm:Get*",
                "ssm:List*"
            ],
            "Resource": "*"
        }
    ]
}
```
As well as (CloudWatchLogsFullAccess)

```
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Action": [
                "logs:*"
            ],
            "Effect": "Allow",
            "Resource": "*"
        }
    ]
}
```

Paste the full ARN into the serverless.yml, I have it called out under provider:


`role: arn:aws:iam::723027765751:role/service-role/micro-service`
### Settings hierarchy
```
/stage/servicename/settings
```
### Accessing SSM Parameters via Code
```
AppConfig.Instance.Parameters["CoinPriceUrl"];
AppConfig.Instance.Parameters["CoinsToWatch"]; //Comma separated list of crypto Symbols to pull prices for.
AppConfig.Instance.Parameters["SlackChannel"]; 
AppConfig.Instance.Parameters["SlackUser"]; 
AppConfig.Instance.Parameters["SlackWebHook"]; 
```
Secure strings will automatically be decrypted. In the AppConfig helper.


### Retrieving parameters via aws-cli
```
aws ssm get-parameters-by-path --path /dev/crypto-pricewatch/settings --recursive
```
#### Sample Output:
```
{
    "Parameters": [
        {
            "Version": 1,
            "Type": "String",
            "Name": "/dev/crypto-pricewatch/settings/CoinPriceUrl",
            "Value": "https://min-api.cryptocompare.com/data/"
        },
        {
            "Version": 2,
            "Type": "StringList",
            "Name": "/dev/crypto-pricewatch/settings/CoinsToWatch",
            "Value": "BTC,ETH,LTC,XRP,VTC,XVG,CRC"
        },
        {
            "Version": 1,
            "Type": "String",
            "Name": "/dev/crypto-pricewatch/settings/SlackChannel",
            "Value": "general"
        },
        {
            "Version": 1,
            "Type": "String",
            "Name": "/dev/crypto-pricewatch/settings/SlackUser",
            "Value": "LambdaBot"
        },
        {
            "Version": 1,
            "Type": "SecureString",
            "Name": "/dev/crypto-pricewatch/settings/SlackWebHook",
            "Value": "AQICAHj7GTUMLLb+voz+gUUoBAz/KGeLrbKNq+UgF9HcIvhrEAHs/5wNWqNBskFhaaW4pKPmAAAArzCBrAYJKoZIhvcNAQcGoIGeMIGbAgEAMIGVBgkqhkiG9w0BBwEwHgYJYIZIAWUDBAEuMBEEDDCGg+WhxzkwVRZ44AIBEIBoGic6cpL3+fFcJtJ8vSH2nIf3s/poX59hmvUnokQIRmseij+9D0YnxOH0VFufHWKKfHfyA2VoAPQMsl3BFACtSYisljdSRS0dC2Y+4e4aiXurzlQajyWVX/madn5pDxi2YtcN1yCQ/Kw="
        }
    ]
}
```

### Retrieve secured values via aws-cli
```
aws ssm get-parameter --name /dev/crypto-pricewatch/settings/TestSecure --with-decryption
```
#### Sample Output:
```
{
    "Parameter": {
        "Version": 1,
        "Type": "SecureString",
        "Name": "/dev/crypto-pricewatch/settings/TestSecure",
        "Value": "Secure string test value"
    }
}
```
