
this code uses Entity Framework

to run on the CLI:

* Install global tool  :   dotnet tool install --global dotnet-ef
* Check connection string ins present in appsetting.json


* Create migrations : dotnet ef migrations add helloNewMigration
* Update Databse : dotnet ef database update



Misc:

By default, "Migrations" folder is placed in root of project, if you manually move it (e.g. to "Data/Migrations", EF tooling will automatically continue to use the new location)