# Seed code - Boilerplate for News-App Step6 Assignment

## Assignment Step Description

In this Assignment News-App Step6, we will implement JWT (JSON Web Token) on top of News-App Step6 Assignment. JSON Web Token (JWT) is an open standard (RFC 7519) that defines a compact and self-contained way for securely transmitting information between parties as a JSON object. This information can be verified and trusted because it is digitally signed.

In this step, we will create this application as collection of 4 microservices using REST API.

1. UserService
2. NoteService
3. ReminderService
4. AuthenticationService

### Problem Statement

In this assignment, we will integrate JWT Authentication into our microservices. To acheive this, we'll create a separate microservice to register and authenticate the user. Check the correctness of the operations with the help of Postman API.

1. We want to secure all the endpoints of our microservices. We want to implement Token based authentication.
2. AuthenticationService should use SQL Server as data store.
3. AuthenticationService should work as Security token service(STS) to provide JWT after authentication.
4. All other microservices should validate the client using JWT provided by AuthenticationService.

<b> Note: For detailed clarity on the class files, kindly go thru the Project Structure </b>

### Expected Solution

REST API must expose the endpoints for the following operations:

- User should be able to register and login with Authentication service.
- AuthenticationService should generate and return a JWT after successfull authentication.
- User should be asked for JWT token to access any endpoints of other microservices(News,Reminder,User).
- Unauthorized user should not be allowed to access any endpoints of other microservices   (News,Reminder,User).
- UserId should be extracted from JWT token to identify the user.

### Steps to be followed

    Step 1: Fork and Clone the boilerplate in a specific folder on your local machine.
    Step 2: Implement Authentication Microservice.
    Step 3: Test all layers of AuthenticationService.
    Step 4: Refactor User,News and Reminder microservices to integrate JWT authentication.
    Step 5: Test each and every controller with appropriate test cases.
    Step 6: Check all the functionalities using URI's mentioned in the controllers with the help of Postman for final output.

### Project structure

The folders and files you see in this repositories, is how it is expected to be in projects, which are submitted for automated evaluation by Hobbes

```
📦News-Step-6
 ┣ 📂AuthenticationService //Microservice to handle user authentication and generate JWT
 ┃ ┣ 📂Controllers
 ┃ ┃ ┗ 📜AuthController.cs //REST API controller to define endpoints for Register and Login
 ┃ ┣ 📂Exceptions //custom exception classes
 ┃ ┃ ┗ 📜UserAlreadyExistsException.cs
 ┃ ┣ 📂Models
 ┃ ┃ ┣ 📜AuthDbContext.cs //DbContext class to speak to SQL Server
 ┃ ┃ ┗ 📜User.cs
 ┃ ┣ 📂Properties
 ┃ ┃ ┗ 📜launchSettings.json
 ┃ ┣ 📂Repository
 ┃ ┃ ┣ 📜AuthRepository.cs  //Implementation of IAuthRepository
 ┃ ┃ ┗ 📜IAuthRepository.cs //Interface to define contract for User(database operations)
 ┃ ┣ 📂Service
 ┃ ┃ ┣ 📜AuthService.cs   //Implementation of IAuthService
 ┃ ┃ ┗ 📜IAuthService.cs  //Interface to define Business Rules
 ┃ ┣ 📜appsettings.Development.json
 ┃ ┣ 📜appsettings.json
 ┃ ┣ 📜AuthenticationService.csproj
 ┃ ┣ 📜Program.cs
 ┃ ┗ 📜Startup.cs
 ┣ 📂NewsService //Microservice to handle news data
 ┃ ┣ 📂Controllers
 ┃ ┃ ┗ 📜NewsController.cs //REST API controller to define endpoints for News
 ┃ ┣ 📂Exceptions //custom exception classes
 ┃ ┃ ┣ 📜NewsAlreadyExistsException.cs
 ┃ ┃ ┣ 📜NoNewsFoundException.cs
 ┃ ┃ ┣ 📜NoReminderFoundException.cs
 ┃ ┃ ┗ 📜ReminderAlreadyExistsException.cs
 ┃ ┣ 📂Models
 ┃ ┃ ┣ 📜News.cs
 ┃ ┃ ┣ 📜NewsContext.cs //class to define Mongo Collection and configuring MongoClient
 ┃ ┃ ┣ 📜Reminder.cs
 ┃ ┃ ┗ 📜UserNews.cs
 ┃ ┣ 📂Properties
 ┃ ┃ ┗ 📜launchSettings.json
 ┃ ┣ 📂Repository
 ┃ ┃ ┣ 📜INewsRepository.cs //Interface to define contract for News(database operations)
 ┃ ┃ ┗ 📜NewsRepository.cs //Implementation of INewsRepository
 ┃ ┣ 📂Services
 ┃ ┃ ┣ 📜INewsService.cs //Interface to define Business Rules
 ┃ ┃ ┗ 📜NewsService.cs //Implementation of INewsService
 ┃ ┣ 📜appsettings.Development.json
 ┃ ┣ 📜appsettings.json
 ┃ ┣ 📜NewsService.csproj
 ┃ ┣ 📜Program.cs
 ┃ ┗ 📜Startup.cs
 ┣ 📂ReminderService //Microservice to handle reminder data
 ┃ ┣ 📂Controllers
 ┃ ┃ ┗ 📜ReminderController.cs //REST API controller to define endpoints for Reminder
 ┃ ┣ 📂Exceptions //custom exception classes
 ┃ ┃ ┣ 📜NoReminderFoundException.cs
 ┃ ┃ ┗ 📜ReminderAlreadyExistsException.cs
 ┃ ┣ 📂Models
 ┃ ┃ ┣ 📜Reminder.cs
 ┃ ┃ ┣ 📜ReminderContext.cs //class to define Mongo Collection and configuring MongoClient
 ┃ ┃ ┗ 📜UserReminder.cs
 ┃ ┣ 📂Properties
 ┃ ┃ ┗ 📜launchSettings.json
 ┃ ┣ 📂Repository
 ┃ ┃ ┣ 📜IReminderRepository.cs //Interface to define contract for Reminder(database operations)
 ┃ ┃ ┗ 📜ReminderRepository.cs //Implementation of IReminderRepository
 ┃ ┣ 📂Services
 ┃ ┃ ┣ 📜IReminderService.cs //Interface to define Business Rules
 ┃ ┃ ┗ 📜ReminderService.cs //Implementation of IReminderService
 ┃ ┣ 📜appsettings.Development.json
 ┃ ┣ 📜appsettings.json
 ┃ ┣ 📜Program.cs
 ┃ ┣ 📜ReminderService.csproj
 ┃ ┗ 📜Startup.cs
 ┣ 📂UserService //Microservice to handle User Profile
 ┃ ┣ 📂Controllers
 ┃ ┃ ┗ 📜UserController.cs //REST API controller to define endpoints for User
 ┃ ┣ 📂Exceptions //custom exception classes
 ┃ ┃ ┣ 📜UserAlreadyExistsException.cs
 ┃ ┃ ┗ 📜UserNotFoundException.cs
 ┃ ┣ 📂Models
 ┃ ┃ ┣ 📜UserContext.cs //class to define Mongo Collection and configuring MongoClient
 ┃ ┃ ┗ 📜UserProfile.cs
 ┃ ┣ 📂Properties
 ┃ ┃ ┗ 📜launchSettings.json
 ┃ ┣ 📂Repository
 ┃ ┃ ┣ 📜IUserRepository.cs //Interface to define contract for User(database operations)
 ┃ ┃ ┗ 📜UserRepository.cs //Implementation of IUserRepository
 ┃ ┣ 📂Services
 ┃ ┃ ┣ 📜IUserService.cs //Interface to define Business Rules
 ┃ ┃ ┗ 📜UserService.cs //Implementation of IUserService
 ┃ ┣ 📜appsettings.Development.json
 ┃ ┣ 📜appsettings.json
 ┃ ┣ 📜Program.cs
 ┃ ┣ 📜Startup.cs
 ┃ ┗ 📜UserService.csproj
 ┣ 📂test
 ┃ ┣ 📂ControllerTests
 ┃ ┃ ┣ 📂IntegrationTest
 ┃ ┃ ┃ ┣ 📜AuthControllerTest.cs
 ┃ ┃ ┃ ┣ 📜CustomWebApplicationFactory.cs
 ┃ ┃ ┃ ┣ 📜NewsControllerTest.cs
 ┃ ┃ ┃ ┣ 📜ReminderControllerTest.cs
 ┃ ┃ ┃ ┗ 📜UserControllerTest.cs
 ┃ ┃ ┗ 📂UnitTest
 ┃ ┃ ┃ ┣ 📜AuthControllerTest.cs
 ┃ ┃ ┃ ┣ 📜NewsControllerTest.cs
 ┃ ┃ ┃ ┣ 📜ReminderControllerTest.cs
 ┃ ┃ ┃ ┗ 📜UserControllerTest.cs
 ┃ ┣ 📂InfraSetup
 ┃ ┃ ┣ 📜NewsDbFixture.cs
 ┃ ┃ ┣ 📜ReminderDbFixture.cs
 ┃ ┃ ┗ 📜UserDbFixture.cs
 ┃ ┣ 📂RepositoryTests
 ┃ ┃ ┣ 📜NewsRepositoryTest.cs
 ┃ ┃ ┣ 📜ReminderRepositoryTest.cs
 ┃ ┃ ┗ 📜UserRepositoryTest.cs
 ┃ ┣ 📂ServiceTests
 ┃ ┃ ┣ 📜NewsServiceTest.cs
 ┃ ┃ ┣ 📜ReminderServiceTest.cs
 ┃ ┃ ┗ 📜UserServiceTest.cs
 ┃ ┣ 📜appsettings-integration.json
 ┃ ┣ 📜appsettings.json
 ┃ ┣ 📜PriorityOrderer.cs
 ┃ ┣ 📜test.csproj
 ┃ ┣ 📜test.csproj.user
 ┃ ┗ 📜TokenModel.cs
 ┣ 📜News-Step-6.sln
 ┗ 📜README.md
```
