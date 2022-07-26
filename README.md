- install nodejs
- install npm
- install dotnet sdk 5.0
- npm run build , then copy build folder contents to api/wwwroot folder. replace all files


**to start client(react app)**
- npm install
- npm start

**to start donet app**
- dotnet watch run


**General Rules**
- coding Rules
1. pascalCase functionsName
2. camelCase variableName
3. always give meaningful variableName of all functions and variables


HangfireUrl:
- key is stored in appsettings
- {baseURLAPI}/hangfire?key=super%20secret%20key

AuditLog is implemented for every tables by default
- make sure you use transaction scope in every Create, Update and Delete quries even if the you are modifying a singe table
- Never ever create explicitly assign primary key in the code itself.Let this be handled by the database itself.If you do, adding datas won't be audited.
- However you can  set primary for seeding data. seed datas won't be audited.



