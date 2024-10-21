# AspNetCoreTemplate-Clean
This repository serves as a good starting point for a startup ASP.NET Core application.

All you need to start the app locally is to install docker compose, and run the below command at the root solution directory (MyApp).

```
docker-compose -f .\infra\docker-compose.yml --env-file .\infra\docker-compose.env up -d
```

Web UIs for local development:
Email UI:		localhost:37408
RabbitMQ UI:	localhost:15672
pgAdmin DB UI:	localhost:8888
Swagger UI:		localhost:8889


## Renaming the solution
If you want to rename the solution there is a script called renameSolution.ps1 in the repository root.
You can run the script from the root folder and it will rename all occurrences of the solution name to the new one.

Please verify the diffcheck afterwards, as the script does not (at time of writing) respect casing upon rename.