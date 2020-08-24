# Introduction 
TODO: Give a short introduction of your project. Let this section explain the objectives or the motivation behind this project. 

#Prerequisites

## ESFA.DC.Web.Operations ##
1. Install Latest LTS Version of nodejs from https://nodejs.org/en/download/ 
2. Install NPM Task Runner extension into Visual Studio 2019 https://github.com/madskristensen/NpmTaskRunner 
3. Install clientside dependancies.  Please run *npm install* from command prompt in web project folder (src/ESFA.DC.Web.Operations), this folder should contain a package.json file detailing what to install.  You may need to run the command prompt as admin and upon completion you should see the node_modules folder on disk.  Please note this is excluded from the project and should not be committed.
4. Build the project.  This will copy the relevant files from the node_modules folder into the following folder: \ESFA.DC.Web.Operations\wwwroot\lib.  Again files here should not be committed to source control.

The currently installed version of all node packages can be found in src/ESFA.DC.Web.Operations/package.json

# Build and Test
## ESFA.DC.Web.Operations ##
Javascript unit tests are provided by Jest (https://jestjs.io/docs/en/getting-started) and are run by running *npm test* via a command prompt in the web project folder (src/ESFA.DC.Web.Operations).  If the NpmTaskRunner runner has been installed then tests can also be run via its GUI in Visual studio.

Some examples of javascript unit tests can be found here: Web.Operations\wwwroot\assets\js\util.test.js

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)