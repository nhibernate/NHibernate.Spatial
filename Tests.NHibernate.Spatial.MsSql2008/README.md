## Running the Tests

In order to run the tests, a MS SQL Server 2008 instance must be available for connection and correctly configured.

### Installation

MS SQL Server 2008 Express with Tools is free and can be downloaded from the [MS website](https://www.microsoft.com/en-us/download/details.aspx?id=22973).

### Configuration

Once MS SQL Server 2008 has been set up, it must be initialized for testing:

1. Ensure that the "Server authentication" is set to "SQL Server and Windows Authentication mode" (see image below).

![Server authentication](http://i.imgur.com/SxkyVpx.png)

2. After changing the authentication mode, the server must be restarted for the changes to take effect (this can be done via the "SQL Server Configuration Manager").

3. Add a new `nhsp_test` user, using SQL Server authentication with `nhsp_test` for the password and making sure to untick "Enforce password policy" (see image below).

![Create new user](http://i.imgur.com/VIOArTc.png)

4. Create a new `nhsp_test` database, owned by the `nhsp_test` user (see image below).

![Create new database](http://i.imgur.com/93mrjwn.png)

5. Finally, the `Server=.` part of the connection string in the [app.config](app.config) file should be updated with the name of your server instance, e.g. `Server=hostname\MSSQLSERVER2008`.
