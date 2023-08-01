using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Xunit;
using System;
using dotenv.net;
using System.Text.RegularExpressions;

namespace FileSystem.Tests.FileSystem.Adapters.Sftp;

public class SftpFixture : IAsyncLifetime
{
    public IContainer SftpContainer { get; }

    private static readonly Faker faker = new();

    private readonly string userName;
    private readonly string password;

    public string UserName => userName;

    public string Password => password;

    private const string Image = "maurosoft1973/alpine-openssh-server";
    public string GetHostname() => SftpContainer.Hostname;
    public int GetPort() => SftpContainer.GetMappedPublicPort(22);

    public SftpFixture()
    {
        DotEnv.Load();

        if (Environment.GetEnvironmentVariable("DOCKER_HOST") == null)
            Environment.SetEnvironmentVariable("DOCKER_HOST", "unix:///var/run/docker.sock");

        userName = faker.Internet.UserName();
        password = faker.Internet.Password();

        userName = Regex.Replace(userName, @"[^a-zA-Z0-9_]+", "");

        var containerBuilder = new ContainerBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .WithAutoRemove(autoRemove: false)
            .WithCleanUp(cleanUp: true)
            .WithImage(Image)
            .WithEnvironment(new Dictionary<string, string>
            {
                ["SSH_USER"] = UserName,
                ["SSH_USER_PASSWORD"] = Password,
                ["TIMEZONE"] = "Europe/Rome"
            })
            .WithPortBinding(22, assignRandomHostPort: true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(22));

        SftpContainer = containerBuilder.Build();
    }

    ///<inheritdoc/>
    public async Task InitializeAsync()
    {
        await SftpContainer.StartAsync().ConfigureAwait(false);
    }

    ///<inheritdoc/>
    public async Task DisposeAsync() => await SftpContainer.StopAsync().ConfigureAwait(false);
}
