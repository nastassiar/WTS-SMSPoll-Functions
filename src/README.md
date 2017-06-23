To Deploy these functions to Azure

- Go to the portal and login
- Click add and select function app
- Fill in all necessary fields and click create
- After deployment has finished go to Platform Features
- Click on Deployment options 
- Add you github credentials and link to this repository
- If the functions don't automatically create go to Deployment Options and click sync
- After the functions have been created go to Platform Features
- Select Application Setting
- Add an app setting for'AzureWebJobsAzureWebJobsServiceBus' with the connect string to the service bus queue

