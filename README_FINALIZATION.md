# Dotnet Service for Content Management

This is a dotnet service that provides CRUD operations for managing content, with additional functionality for managing genres.

## Tasks Completed

1. **Implement Missing Endpoints:**
   - `POST /api/v1/Content/{id}/genre`: Adds new genres to a content.
   - `DELETE /api/v1/Content/{id}/genre`: Removes genres from a content.

2. **Logging:**
   - Added logging to provide insights into the service operations without overwhelming production logs.

3. **Database Technology:**
   - Adapted the project to use [your chosen database technology].

4. **Performance Improvement:**
   - Implemented caching to speed up responses while still using `SlowDatabase` class.

5. **Unit Tests:**
   - Added comprehensive unit tests to ensure the functionality works as expected.

6. **Deprecate and Replace Endpoint:**
   - Deprecated `GET /api/v1/Content`.
   - Introduced new endpoint with filtering by Title and Genre.

## Installation

1. **Clone the repository:**
   ```sh
   git clone [your-repo-url]
   cd [repo-directory]
    ```
2. **Install dependencies:**
    ```sh
    dotnet restore
    ```
    
3. **Set up the new database (if applicable):**

    *If you're using MongoDB:*
   - Install MongoDB on your system.
   - Create a new database for the application.
   - Update the connection string in the `appsettings.json` file with the MongoDB URI.

   *If you're using Redis:*
   - Install Redis on your system.
   - Configure Redis settings in the `appsettings.json` file.

## Usage

**Run the application:**
    ```sh
    dotnet run
    ```
    
**Access the endpoints via:**

   - GET /api/v1/Content
   - POST /api/v1/Content
   - PUT /api/v1/Content/{id}
   - DELETE /api/v1/Content/{id}
   - POST /api/v1/Content/{id}/genre
   - DELETE /api/v1/Content/{id}/genre
   - GET /api/v1/Content/Search
  
##Testing

1. **Run the tests:**

    ```sh
    dotnet test
    ```

##Technologies Used

   - ASP.NET Core
   - Redis (for caching)
   - MongoDB (as the chosen database technology)
   - Moq (for mocking in unit tests)
   - xUnit (for testing)
   - Serilog (for logging)

## License

   - This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

    
##Contributing

   - Feel free to submit issues or pull requests. For major changes, please open an issue first to discuss what you would like to change.