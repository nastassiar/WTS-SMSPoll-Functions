To Deploy these functions to Azure Portal

- Go to the portal and login
- Click add and select function app
- Fill in all necessary fields and click create
- After deployment has finished go to Platform Features
- Click on Deployment options 
- Add you github credentials and link to this repository
- If the functions don't automatically create go to Deployment Options and click sync
- The following functions should be created:
  - FBDataWebhook : Webhook for recieving all pages feed update data
  - FBDataProcess : Function for formatting FB data into correct types. Also passes user data from posts to separate process for updating users.
  - FBDataStore : Function that stores the formatted FB data into documentDB.
  - FBUserStore : Function that updates users based on the data recieved from the facebook page activity.
  - SMSDataStore : Function that stores the sms data.
  - SMSSUserProcess: Function that parses out the user data from the SMS userlogs messages.
  - SMSUserStore : Function that updates or creates users based on the SMS data.
  - GetCurrentDateTimeUnix : a utility function for converting from Unix to datetime. (used by the logic apps) 
- After the functions have been created go to Platform Features
- Select Application Setting
- Add an app settings for:
  - 'AzureWebJobsAzureWebJobsServiceBus' with the connection string to the service bus queue
  - 'CosmosDB_Connection' with the connection string to CosmosDB
  - 'FB_Verify_Token' with the verify token set in the Facebook developer portal


These steps will deploy the functions but it will still be necessary to 
1. Go to Facebook developer portal and setup the webhook for the pages and to push to the URL for FBDataWebhook function.
2. Set up the SMS Logic Apps for polling the SMS API. 
3. Set up all of the following service bus queues
- fb_data_toprocess
- fb_data_tostore
- fb_user_tostore
- sms_data_tostore
- sms_user_toprocess
- sms_user_tostore
4. Create the CosmosDB account and the a database with the name WTSSocialMediaDB and 3 collections, SMSData, FBData and Users


