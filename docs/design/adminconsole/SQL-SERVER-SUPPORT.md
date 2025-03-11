# Adding MS SQL Support to Instance Management

## Summary

The Instance Management worker allows system administrators to manage instances of their ODS. This worker is currently configured to support PostgreSQL and needs support added for SQL Server.

## Background

The Instance Management worker was designed to execute ODS administration tasks without tying up a handler's main process. This asynchronous nature requires the Instance management worker to manage other aspects of its environment, including persistence and compatibility with other services. The data layer for the Instance Management worker was implemented using PostgreSQL, with the idea of using this dialect of SQL to start, verifying its implementation, then including support for SQL Server after.

The Postgres implementation of the Instance Manager includes support for Creating, Renaming, Deleting, and Restoring ODS Instances.

There is a Postgres DB dump of sample student data that serves as a starting point for minimal and populated templates. No such sample exists for SQL Server so this will need to be solved in order to remain in parity with the Postgres provisioning features. Additionally, SQL Server utilizes a slightly different dialect from server so these actions must be converted. Lastly, SQL Server licensing requires that images / containers including the Azure SQL Server base image not be distributed. This does not prohibit providing instructions on how to build these images.

Azure SQL Server support is an additional consideration but will remain out of the scope of this implementation.

## Design Overview

Adding support for SQL Server has been divided into the following four distinct areas of focus below.

### 1. Configuring and connecting to the database

The first step involves adding a connection and corresponding configuration to the application. This step is to ensure the application is communicating with the desired SQL Server instance (Platform hosted, docker hosted). There is a `CreateConnection()` method in the [`SqlServerSandboxProvisioner.cs`](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS/blob/main/Application/EdFi.Ods.Sandbox/Provisioners/SqlServerSandboxProvisioner.cs) example from ODS Sandbox, which demonstrates how to connect properly.

### 2. Translating the actions to MS SQL dialect for each of the admin functions

The current Postgres implementation of the Instance Management Worker borrows from the provisioner patterns seen in the `EdFi.Ods.Sandbox` project.

The `PostgresSandboxProvisioner.cs` class contains methods for creating connections, renaming, deleting,  managing file paths, retrieving DB Status among others. These actions can be used to inform the implementation details for the Instance Management worker and the corresponding SQL Server actions. These actions will need to be translated to MS SQL and added to a new`SqlServerInstanceProvisioner.cs` file located in the `Provisioners/` directory.

Other supporting configuration files, e.g. `Provisioners/DatabaseEngineProvider.cs` will also need to be updated to reflect added support for SQL Server, resulting in a seamless experience between selecting the PostgreSQL engine and SQL Server.

### 3. Low-friction SQL Server connection setup

The next task is providing a low-friction environment for users to spin up and connect to the desired SQL Server instance. Historically, Ed-Fi has provided guidance to users on how to provision SQL Server configurations for various environments. This is done to avoid hosting images containing the distribution itself, which would create conflict with the Apache 2.0 license that accompanies the Ed-Fi Alliance source code and tools.

SQL Server Options that Ed-Fi provides guidance:

1. Installation included with official binaries
2. Experimental, bare MSSQL install scripts
3. Docker compose with local sample data (Users SQL Server Express Edition)

A quick reference for setting up SQL Server runtime can be found at the following [tech docs guide](https://docs.ed-fi.org/reference/docker/#step-2-setup-runtime-environment). Because the Instance Management worker already takes advantage of Docker compose, option three appears to provide the most benefit for little effort. This will still allow users to get up and running quickly without spending much time provisioning a host machine.

### 4. Seeding data for restoration

Lastly, the Instance Management worker should be able to restore an ODS instance. This by extension means that the worker must support exporting the data, creating the instance, and importing the data to a created instance.

The SQL Server database will need to be populated in order to provide the necessary data for export. Two viable options to consider for migrating the data include:

1. Creating the Sample data and tables directly using synthesized data.
2. Transforming Sample data from PostgreSQL backups

It appears that the MSSQL Sandbox compose files include a populated template that connects to a predefined volume. An approach can be to connect to this instance then export the to a BACPAC file, which can be used for restoration.

Once this is done successfully, the next step is to implement the restoration of this BACPAC template data. An action can be added to the interface that runs the necessary steps within the transaction that results in creating an instance and subsequently reading the BACPAC.

## Design Details

Will add additional implementation details based on feedback from above.

## Test Strategy

The following user journeys represent areas critical to instance management using SQL Server.

An admin can connect to a SQL Server.

* Provision the server
* Create connection string
* Ensure that the application connect to server successfully

An admin can add a new SQL Server ODS instance

* Connect to the SQL Server Instance
* Execute command to create a new DB Instance and tables
* Demonstrate new instance and tables are available

An admin can rename an existing SQL Server ODS instance

* Connect to the SQL Server Instance
* Execute command to rename ODS instance
* Ensure DB and corresponding tables and connection strings are updated
* Add additional path checking duplicate name for rename does not exist

An admin delete a new SQL Server ODS instance

* Connect to the SQL Server instance
* Execute command to delete instance
* Ensure instance and corresponding tables are properly marked for deletion.

An admin can restore a new SQL Server ODS instance

* Connect to the SQL Server instance
* Execute the create command providing a name and source of the restoration
* Ensure a new instance exists with provided restoration data (dbs, tables, and rows)
