# AspNetCoreSimpleTemplate
This repository serves as a good starting point for a startup ASP.NET Core application.

All you need to start the app locally is to install docker compose, and run the below command at the root directory.

```
docker compose --env-file docker-compose-local.env -f docker-compose-local.yaml up -d
```
You can make changes in the docker-compose-local.yaml in case you want to use different ports for the database/SMTP server.

Afterwards you can run the MyApp.DbDeploy project, and the server is ready to be started and used.
For emails, you can access the MailHog SMTP UI locally at localhost:8025.