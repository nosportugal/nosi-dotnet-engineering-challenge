# Engineering challenge

### Service
This is a dotnet service that will do all the CRUD methods to a database.

The implementation is made for the database to be slow and very simple.

There are 2 endpoints that are not implemented `POST /api/v1/Content/{id}/genre` and `DELETE /api/v1/Content/{id}/genre`.

## The Challenge
We want you to fork this repository and try solve the following tasks.
(Remember that you cannot change the `SlowDatabase.cs`)
## Task 1 ✅ Completed

Implement the missing endpoints:
 * `POST /api/v1/Content/{id}/genre`
    * This endpoint is supposed to add new genres. This should not allow repeated genres to be listed.
 * `DELETE /api/v1/Content/{id}/genre`
    * This endpoint should delete genres.


## Task 2 ✅ Completed

When the service is runnning in production we have no way of knowing what's happening in the service.
Add logging to the service that you think will give us enough information to know what's happening, but not overload the amount of logging when in Production.

### Solution:

To address this challenge, we have implemented logging in the service using the built-in Logger provided by ASP.NET Core. The Logger has been configured to capture relevant information about the service's operations and activities. Log levels have been set appropriately to ensure that logging does not overload the system, especially in production environments. 

Logs can be accessed through various channels, such as console output, log files, or external logging services integrated into the application.

## Task 3 ✅ Completed

You team has the freedom to choose a new database. Make the necessary changes to adapt to this new connection. (You can choose any tecnology, e.g., Mongodb, Redis, Cassandra, CouchDB, MySQL, etc...)

### Solution:

For this task, we have chosen to utilize MongoDB as the new database technology. We have made the necessary changes in the application to connect and interact with MongoDB. The connection string in the `appsettings.json` file has been updated with the MongoDB URI.


## Task 4 ✅ Completed

While your new tecnology isn't Production Ready the  SlowDatabase is really slow (and you cannot change the class).
Find a way to speed up the endpoints without modifying the `SlowDatabase.cs`.

### Solution:

To address the performance issue caused by the slow database, we have implemented Redis caching. By caching frequently accessed data in Redis, we have significantly improved the response time of the endpoints without making any modifications to the `SlowDatabase.cs`. This approach ensures that the application performs efficiently even when the new technology (MongoDB) is not yet production-ready.

## Task 5 ✅ Completed

The Project does not have any unit testing, add tests that guarantee that everything works as expected.

### Solution:

To address the lack of unit testing in the project, we have implemented unit tests using Xunit and Mock. These tests ensure that all components of the service function correctly and as expected. By covering critical functionalities with unit tests, we can verify the correctness of the codebase and catch any regressions during development or refactoring.

## Task 6 ✅ Completed

We want to deprecate the `GET /api/v1/Content` which always returns all the contents.
Create a new endpoint that deprecates this and implements a way to filter the contents by Title and/or Genre.
(The filter condition can be something as simple as substring and a contains).

## Installation

1. **Clone the repository:**
   ```sh
   git clone https://github.com/BruneiMS/nosi-dotnet-engineering-challenge.git
   ```
2. **Navigate to the cloned repository directory:**
   ```sh
    cd nosi-dotnet-engineering-challenge
   ```
3. **Install dependencies:**
    ```sh
    dotnet restore
    ```
    
4. **Set up the new database (if applicable):**

    *If you're using MongoDB:*
   - Install MongoDB on your system.
   - Create a new database for the application.
   - Update the connection string in the `appsettings.json` file with the MongoDB URI.

   *If you're using Redis:*
   - Install Redis on your system.
   - Configure Redis settings in the `appsettings.json` file.

## Usage

**Run the application:**
    
    dotnet run
    
**Access the endpoints via:**

   - GET /api/v1/Content
   - POST /api/v1/Content
   - PUT /api/v1/Content/{id}
   - DELETE /api/v1/Content/{id}
   - POST /api/v1/Content/{id}/genre
   - DELETE /api/v1/Content/{id}/genre
   - GET /api/v1/Content/Search
  
## Testing

1. **Run the tests:**

    ```sh
    dotnet test
    ```

## Technologies Used

   - ASP.NET Core
   - Redis (for caching)
   - MongoDB (as the chosen database technology)
   - Moq (for mocking in unit tests)
   - xUnit (for testing)
   - Serilog (for logging)

## License

   - This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

    
## Contributing

   - Feel free to submit issues or pull requests. For major changes, please open an issue first to discuss what you would like to change.