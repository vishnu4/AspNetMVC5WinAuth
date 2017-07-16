# AspNetMVC5WinAuth
Example of windows authorization in Asp.Net MVC 5

This is a basic application that uses OWIN, EF6 and ASP.Net MVC5 to authenticate users via windows authorization, and then create entries for that user inside a local database (so that other database entries can be linked back to that user).  Structuremap is used as a DI, and additionally Web API has been set up as well.

###Installation Steps
In order to get this to work, you'll need to do a few things on your local box:
* create an IIS Application pointing at this application, make sure it uses an application pool with a .Net 4.0 framework 
* In IIS, set up your websites authentication to ONLY Asp.net impersonation and Windows authentication.  You may need to install these if you don't see them as options
* The user that accesses the website via the browser MUST have permissions on the folder where the website exists (Write permissions are not necesary).  Otherwise you will get a 401 error.
  * By default IIS tends to set up this user as pass-through authentication
* the database connection strings assume trusted connection.  You may need to change that, but if you don't then the user that you log into the website with must have data read/write on the database

####Other Notes
* You can actually switch between windows authentication and a regular login page model, by
  * turning off Windows Authentication in IIS, and turning on anonymous authentication
  * changing UseWindowsAuthentication in web.config to false
* Make sure when you run update-database on WindowsAuthCommon to create your local version of the database that you change your app.config to point to your local database
* there is a section in accountcontroller.windows that grabs domain information about the user (other than the username) and adds it's details to the database user, but it is not really tested and i can't confirm it works.
