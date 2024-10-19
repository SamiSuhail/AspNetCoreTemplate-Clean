# AspNetCoreTemplate-Clean
This repository serves as a good starting point for a startup ASP.NET Core application.

All you need to start the app locally is to install docker compose, and run the below command at the root solution directory (MyApp).

```
docker-compose -f .\infra\docker-compose.yml --env-file .\infra\docker-compose.env up -d
```
You can make changes in the docker-compose-local.yaml in case you want to use different ports for the database/SMTP server.

Afterwards you can run the MyApp.DbDeploy project, and the server is ready to be started and used.
For emails, you can access the MailHog SMTP UI locally at localhost:8025.
For messaging, you can access the RabbitMQ Management UI locally at localhost:15672
For database administration, you can access the PgAdmin UI locally at localhost:8888

All credentials can be found in the docker-compose-local.env file.

If you want to rename the solution there is a script called renameSolution.ps1 in the repository root.
You can run the script from the root folder and it will rename all occurences of the solution name to the new one.
There are still a few references you might want to manually fix:
- the database name in docker-compose-local.env, and appsettings.json for both projects
- the sender email in appsettings.json