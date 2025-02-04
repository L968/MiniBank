IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> mysqlPassword = builder.AddParameter("mysqlPassword", "root", secret: true);

IResourceBuilder<MySqlServerResource> mysql = builder.AddMySql("minibank-mysql", password: mysqlPassword)
    .WithImageTag("9.2.0")
    .WithPhpMyAdmin()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<MySqlDatabaseResource> mysqldb = mysql.AddDatabase("minibank-mysqldb", "minibank");

IResourceBuilder<ParameterResource> rabbitMQUsername = builder.AddParameter("rabbitMQUsername", "guest", secret: true);
IResourceBuilder<ParameterResource> RabbitMQPassword = builder.AddParameter("RabbitMQPassword", "guest", secret: true);

IResourceBuilder<RabbitMQServerResource> rabbitMQ = builder.AddRabbitMQ("minibank-rabbitmq", rabbitMQUsername, RabbitMQPassword)
    .WithImageTag("4.0.5")
    .WithManagementPlugin()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.MiniBank_Api>("minibank-api")
    .WithReference(mysqldb)
    .WaitFor(mysqldb)
    .WithReference(rabbitMQ)
    .WaitFor(rabbitMQ);

builder.AddProject<Projects.MiniBank_MigrationService>("minibank-migrationservice")
    .WithReference(mysqldb)
    .WaitFor(mysqldb);

await builder.Build().RunAsync();
