# FITBOT Monitor
[![Build and deploy dotnet core app to Azure Function App - func-fitbot-prod-westeu-001](https://github.com/benjaminkech/fit-bot-monitor/actions/workflows/main_func-fitbot-prod-westeu-001.yml/badge.svg)](https://github.com/benjaminkech/fit-bot-monitor/actions/workflows/main_func-fitbot-prod-westeu-001.yml)

Azure Function for monitoring the gym API. As soon as a place is available, the function will send a notification with the CallMeeBot. The function is using Azure Durable Function with a Monitor orchestrator.

# Run locally
Create a file local.settings.json with the content below.
- The interval is set in minutes
- Phone number with country code (+41)
````
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "Storage",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "GYM_API": "API",
    "GYM_USERID": "USERID",
    "INTERVAL": "5",
    "USERS": "[{\"name\":\"name\",\"callMeBotSettings\":{\"phone\":\"phone\",\"apikey\":key,\"messenger\":\"Signal\"}}]"
  },
  "Host": {
    "LocalHttpPort": 7072,
    "CORS": "*"
  }
}
```
