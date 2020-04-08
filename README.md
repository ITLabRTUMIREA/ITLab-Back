# Backend for RTUITLab managing system


Status | master | develop
--- | --- | ---
Build |  [![Build Status][build-master-image]][build-master-link] | [![Build Status][build-dev-image]][build-dev-link]
Public API | [![RTUITLab.ITLab.Models.PublicAPI](https://img.shields.io/nuget/v/RTUITLab.ITLab.Models.PublicAPI.svg)](https://www.nuget.org/packages/RTUITLab.ITLab.Models.PublicAPI/) | ---

[build-dev-image]: https://dev.azure.com/rtuitlab/RTU%20IT%20Lab/_apis/build/status/ITLab-Back?branchName=develop
[build-dev-link]: https://dev.azure.com/rtuitlab/RTU%20IT%20Lab/_build/latest?definitionId=65&branchName=develop
[build-master-image]: https://dev.azure.com/rtuitlab/RTU%20IT%20Lab/_apis/build/status/ITLab-Back?branchName=master
[build-master-link]: https://dev.azure.com/rtuitlab/RTU%20IT%20Lab/_build/latest?definitionId=65&branchName=master

## Requriments

.Net Core 3.0

## Configuration

```appsettings.json```

```appsettings.Secret.json``` can be placed to the BackEnd folder

```js
{
    "Authority": "autority host of identity server 4",
    "DB_TYPE": "IN_MEMORY" | "POSTGRES",
    "ConnectionStrings" : {
        "POSTGRES": "postgres connection string if DB_TYPE == POSTGRES"
    },
    "RegisterTokenPair": [ // Use for register first user
        {
            "Email": "test1@test.com",
            "Token": "ABCDEFG"
        }
    ],
    "UseDebugEmailSender": true | false, // true for using console email sender
    "EmailSenderOptions": { // used when UseDebugEmailSender == false
        "BaseAddress": "base address of email sender service",
        "Key": "access token for sendind emails"
    },
    "EmailTemplateSettings": {
        "InvitationTemplateUrl": "direct link to invitation email template",
        "ResetPasswordTemplateUrl": "direct link to reset password template",
    },
    "DB_INIT": true, // true for activate initializer service, cread default roles and users from DBInitializeSettings section
    "DBInitializeSettings": { // section for db initializer service
        "Users" : [
            {
                "UserName": "user name for one of default users",
                "FirstName": "firstname for one of default users",
                "LastName": "lastname for one of default users",
                "Email": "email for one of default users",
                "PhoneNumber": "phone number for one of default users",
                "Password": "password for one of default users"
            }
        ],
        "WantedRoles": [
            {
                "Email": "email of target user",
                "RoleName": "name of role (Models.People.Roles.RoleNames)"
            }
        ]
    },
    "NotifyType" : "http" | "redis" | "console" // type of used notify service
    "HttpNotifierSettings": { // used for NotifyType == "http"
        "Host": "host of notifier service",
        "NeedChangeUrl": true | false, // change notify service host in runtime capability activator
        "NotifySecret": "token for access notifier service from that service"
    },
    "RedisNotifierSettings": { // used for NotifyType == "redis"
        "ConnectionString": "connection string to redis instance",
        "CnhannelName": "channel name for publishing messages"
    },
    "UseRandomEventsGenerator": true | false, // true for spawn random events, use only for debug

}
```


## Run
```bash
cd ./Backend
dotnet run
```

Swagger API will be available on [localhost:5500](http://localhost:5500)

## Work with database

To use ```dotnet ef``` commands go to Database project, and with all commands use ```--startup-project ../Backend``` parameter

For fast work with migrations use PowerShell scripts in folder Database/Scripts. For example, add migration:
```bash
cd Database
./Scripts/AddMigration.ps1
```
