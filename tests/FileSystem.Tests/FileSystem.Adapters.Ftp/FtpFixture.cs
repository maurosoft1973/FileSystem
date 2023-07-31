using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Xunit;
using System;
using dotenv.net;
using Docker.DotNet.Models;
using System.Text.RegularExpressions;

namespace FileSystem.Tests.FileSystem.Adapters.Ftp
{
    public class FtpFixture : IAsyncLifetime
    {
        public IContainer FtpContainer { get; }

        private static readonly Faker faker = new();

        private readonly string userName;
        private readonly string password;

        public string UserName => userName;

        public string Password => password;

        private const int MaxUsersCount = 1;

        private const int PassivePortStart = 31_000;

        private const string Image = "maurosoft1973/alpine-proftpd-server";
        public string GetHostname() => FtpContainer.Hostname;
        public int GetPort() => FtpContainer.GetMappedPublicPort(21);

        public FtpFixture()
        {
            DotEnv.Load();

            if (Environment.GetEnvironmentVariable("DOCKER_HOST") == null)
                Environment.SetEnvironmentVariable("DOCKER_HOST", "unix:///var/run/docker.sock");

            userName = faker.Internet.UserName();
            password = faker.Internet.Password();

            userName = Regex.Replace(userName, @"[^a-zA-Z0-9_]+", "");

            var passivePortEnd = PassivePortStart + (MaxUsersCount);

            var containerBuilder = new ContainerBuilder()
                .WithName(Guid.NewGuid().ToString("D"))
                .WithAutoRemove(autoRemove: false)
                .WithImage(Image)
                .WithCleanUp(cleanUp: true)
                .WithEnvironment(new Dictionary<string, string>
                {
                    ["FTP_USER"] = UserName,
                    ["FTP_USER_PASSWORD"] = Password,
                    ["PASV_MIN_PORT"] = $"{PassivePortStart}",
                    ["PASV_MAX_PORT"] = $"{passivePortEnd}",
                    ["TIMEZONE"] = "Europe/Rome"
                })
                .WithPortBinding(20, assignRandomHostPort: true)
                .WithPortBinding(21, assignRandomHostPort: true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(21));

            for (var port = PassivePortStart; port < passivePortEnd; port++)
                containerBuilder = containerBuilder.WithPortBinding(port);

            FtpContainer = containerBuilder.Build();
        }

        ///<inheritdoc/>
        public async Task InitializeAsync()
        {
            await FtpContainer.StartAsync().ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task DisposeAsync() => await FtpContainer.StopAsync().ConfigureAwait(false);
    }
}
