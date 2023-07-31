# Maurosoft FileSystem [![NuGet](https://img.shields.io/nuget/v/Maurosoft.FileSystem)](https://www.nuget.org/packages/Maurosoft.FileSystem)

## Builds
[![FileSystem [Build]](https://github.com/maurosoft1973/FileSystem/actions/workflows/build.yml/badge.svg)](https://github.com/maurosoft1973/FileSystem/actions/workflows/build.yml)
[![FileSystem [Sonarqube]](https://github.com/maurosoft1973/FileSystem/actions/workflows/sonarqube.yml/badge.svg)](https://github.com/maurosoft1973/FileSystem/actions/workflows/sonarqube.yml)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Maurosoft_FileSystem&metric=alert_status)](https://sonarcloud.io/summary/overall?id=Maurosoft_FileSystem) \
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Maurosoft_FileSystem&metric=sqale_rating)](https://sonarcloud.io/summary/overall?id=Maurosoft_FileSystem) \
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Maurosoft_FileSystem&metric=reliability_rating)](https://sonarcloud.io/summary/overall?id=Maurosoft_FileSystem) \
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Maurosoft_FileSystem&metric=security_rating)](https://sonarcloud.io/summary/overall?id=Maurosoft_FileSystem) \
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Maurosoft_FileSystem&metric=coverage)](https://sonarcloud.io/summary/overall?id=Maurosoft_FileSystem)

## Introduction
Maurosoft FileSystem is a file system abstraction supporting multiple adapters.

## Installation
Reference NuGet package `Maurosoft.FileSystem` (https://www.nuget.org/packages/Maurosoft.FileSystem).

For adapters other than the local file system (included in the `Maurosoft.FileSystem` package) please see the [Supported adapters](#supported-adapters) section.

## Supported adapters
| Adapter                                         | Package                                           | NuGet                                                                                                                                                                      |
|:------------------------------------------------|:--------------------------------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [Local adapter](#local-adapter)                 | `Maurosoft.FileSystem`                            | [![NuGet](https://img.shields.io/nuget/v/Maurosoft.FileSystem)](https://www.nuget.org/packages/Maurosoft.FileSystem)                                                       |
| [FTP](#ftp-adapter)                             | `Maurosoft.FileSystem.Adapters.Ftp`               | [![NuGet](https://img.shields.io/nuget/v/Maurosoft.FileSystem.Adapters.Ftp)](https://www.nuget.org/packages/Maurosoft.FileSystem.Adapters.Ftp)                             |
| [Memory](#memory-adapter)                       | `Maurosoft.FileSystem.Adapters.Memory`            | [![NuGet](https://img.shields.io/nuget/v/Maurosoft.FileSystem.Adapters.Memory)](https://www.nuget.org/packages/Maurosoft.FileSystem.Adapters.Memory)                       |
| [SFTP](#sftp-adapter)                           | `Maurosoft.FileSystem.Adapters.Sftp`              | [![NuGet](https://img.shields.io/nuget/v/Maurosoft.FileSystem.Adapters.Sftp)](https://www.nuget.org/packages/Maurosoft.FileSystem.Adapters.Sftp)                           |

## Supported operations
For a full list of the supported operations please see the [IFileSystem](../master/FileSystem/src/IFileSystem.cs) interface.

## Usage

### Instantiation
```
var adapters = new List<IAdapter>
{
    new LocalAdapter("adapterPrefix", "adapterRootPath")
};

// Instantiation option 1.
var fileSystem = new FileSystem(adapters);

// Instantiation option 2.
var fileSystem = new FileSystem();
fileSystem.Adapters = adapters;
```

### Local adapter
```
var adapters = new List<IAdapter>
{
    new LocalAdapter("local1", "/var/files"),
    new LocalAdapter("local2", "D:\\Files")
};

var fileSystem = new FileSystem(adapters);
```

### Memory adapter
```
var adapters = new List<IAdapter>
{
    new MemoryAdapter("memory1", "/"),
    new MemoryAdapter("memory2", "/")
};

var fileSystem = new FileSystem(adapters);
```

### SFTP adapter
```
// SFTP connection.
var privateKeyFile = new PrivateKeyFile("/home/userName/.ssh/id_rsa");
var privateKeyAuthenticationMethod = new PrivateKeyAuthenticationMethod("userName", privateKeyFile);
var sftpConnectionInfo = new ConnectionInfo("hostName", "userName", privateKeyAuthenticationMethod);
var sftpClient = new SftpClient(sftpConnectionInfo);

var adapters = new List<IAdapter>
{
    new LocalAdapter("local", "/var/files"),
    new SftpAdapter("sftp", "/var/files", sftpClient)
};

var fileSystem = new FileSystem(adapters);
```

### Example operations
```
// Sftp connection.
var privateKeyFile = new PrivateKeyFile("/home/userName/.ssh/id_rsa");
var privateKeyAuthenticationMethod = new PrivateKeyAuthenticationMethod("userName", privateKeyFile);
var sftpConnectionInfo = new ConnectionInfo("hostName", "userName", privateKeyAuthenticationMethod);
var sftpClient = new SftpClient(sftpConnectionInfo);

var adapters = new List<IAdapter>
{
    new LocalAdapter("local", "/var/files"),
    new SftpAdapter("sftp", "/var/files", sftpClient)
};

// Copies a file from the `local` adapter to the `sftp` adapter.
await fileSystem.CopyFileAsync("local://foo/bar.txt", "sftp://bar/foo.txt");

// Moves a file from the `sftp` adapter to the `local` adapter.
await fileSystem.MoveFileAsync("sftp://Foo/Bar.txt", "local://Bar/Foo.txt");

// Writes string contents to the `local` adapter.
await fileSystem.WriteFileAsync("local://foo/bar.txt", "Bar!");

```