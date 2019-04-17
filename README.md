# Backend for RTUITLab managing system


Status | master | develop
--- | --- | ---
Build |  [![Build Status][build-master-image]][build-master-link] | [![Build Status][build-dev-image]][build-dev-link]
Public API | [![RTUITLab.ITLab.Models.PublicAPI](https://img.shields.io/nuget/v/RTUITLab.ITLab.Models.PublicAPI.svg)](https://www.nuget.org/packages/RTUITLab.ITLab.Models.PublicAPI/) | ---


[build-dev-image]: https://dev.azure.com/rtuitlab/ITLab/_apis/build/status/itlab/ITLab-Back-develop?branchName=develop
[build-dev-link]: https://dev.azure.com/rtuitlab/ITLab/_build/latest?definitionId=39&branchName=develop
[build-master-image]: https://dev.azure.com/rtuitlab/ITLab/_apis/build/status/itlab/ITLab-Back-master?branchName=master
[build-master-link]: https://dev.azure.com/rtuitlab/ITLab/_build/latest?definitionId=38&branchName=master

## Prerequriments

.Net Core 2.1

## Configuration

```appsettings.Secret.json``` must be placed to the BackEnd folder

```json
{
    "DB_TYPE": "IN_MEMORY",
    "ConnectionStrings" : {
        "SQL_SERVER_LOCAL": "ms sql server connection string if DB_TYPE == SQL_SERVER_LOCAL",
        "SQL_SERVER_REMOTE": "ms sql server connection string if DB_TYPE == SQL_SERVER_LOCAL",
        "POSTGRES_LOCAL": "postgres connection string if DB_TYPE == POSTGRES_LOCAL"
    },
    "JwtIssuerOptions": {
        "SecretKey": "some random key for JWT"
    },
    "RegisterTokenPair": [
        {
            "Email": "test1@test.com",
            "Token": "ABCDEFG"
        }
    ],
	"UseDebugEmailSender": true,
    "EmailSenderSettings": {
        "Email": "sender email",
        "Password": "password for sender email",
        "InvitationTemplateUrl": "direct link to invitation email template",
        "ResetPasswordTemplateUrl": "direct link to reset password template",
        "SmtpHost": "smtp host for sender email",
        "SmtpPort": "smtp port for sender email"
    },
    "DB_INIT": true,
    "DBInitializeSettings": {
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
    "UseConsoleLogger": true,
    "NotifierSettings": {
        "Host": "link to notifier service",
        "AccessToken": "token for access that service from notifier service",
        "NotifySecret": "token for access notifier service from that service"
    }
}
```

**DB_TYPE** - type of the database which will be used

**UseDebugEmailSender** - if true app will use a debug email service

**DB_INIT** - if true - database will be initialized from DBInitializeSettings section

**UseConsoleLogger** - if true - all notifications will be print in logger. If false - notifications will be send via notifier service

## Run
```bash
cd ./Backend
dotnet run
```
API will be available on [localhost:5000](http://localhost:5000)
