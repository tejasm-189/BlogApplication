# BlogApplication

BlogApplication is a modern web application built with Blazor Server, .NET, and MongoDB. It provides a platform for users to create, share, and interact with blog posts, incorporating features like user authentication, real-time notifications, and AI-powered content generation.

## Overview

This application serves as a feature-rich blogging platform where users can:
*   Register and sign in to their accounts.
*   Create, read, update, and delete blog posts.
*   Enhance posts with images and categorize them.
*   Utilize AI to generate blog post content and find relevant images.
*   Comment on posts and like posts.
*   Follow other users.
*   Receive real-time notifications for interactions like new followers, likes, and comments.

## Core Features

### 1. User Authentication & Management
*   **User Registration & Sign-in:** Secure user registration and login functionality, managed via Supabase.
*   **Session Management:** User sessions are maintained, and user-specific data (like ID, username, email, role) is accessible through browser local storage.
*   **Role-Based Access (Admin):** The system can identify admin users (though specific admin functionalities beyond comment/post deletion by an admin are not explicitly detailed in the reviewed code).

### 2. Blog Post Management (CRUD)
*   **Create Posts:** Users can create new blog posts with a title, content, category, and an option to publish immediately.
    *   **AI Content Generation:** Integrated with Google's Gemini API to automatically generate blog post content based on the title and category.
    *   **AI Image Suggestion:** Integrated with Google Custom Search API to fetch relevant images for the blog post based on title and category.
*   **Read Posts:**
    *   View a list of all blog posts (`ViewPost.razor` - inferred).
    *   View detailed information for a single post (`PostDetail.razor`), including content, author, category, creation date, likes, and comments.
*   **Update Posts:** Users can edit their existing blog posts (`EditPost.razor` - inferred, `PostDetail.razor` links to it).
*   **Delete Posts:** Users can delete their own posts. Admin users can also delete any post.
*   **Image Uploads:** Users can upload images for their blog posts. Images are stored on the server in the `wwwroot/uploads` directory.

### 3. User Interactions
*   **Comments:**
    *   Users can add comments to blog posts.
    *   Users can delete their own comments. Admin users can delete any comment.
*   **Likes:**
    *   Users can like and unlike blog posts.
    *   The number of likes is displayed for each post.
*   **Follows:**
    *   Users can follow and unfollow other blog authors.
    *   Follower and following counts are displayed on the post detail page for the author.

### 4. Real-time Notifications
*   **Notification System:** Users receive notifications for various events:
    *   New follower
    *   New like on their post
    *   New comment on their post
*   **Notification Storage:** Notifications are stored in MongoDB.
*   **Notification UI:**
    *   `Shared/NotificationBadge.razor` and `Shared/NotificationsMenu.razor` (inferred UI components) likely display unread notification counts and a list of notifications.
*   **Real-time Updates:** The system uses `NotificationUpdateService` to signal UI updates for new notifications, with an intended integration with SignalR (`NotificationHub.cs` is present but placeholder).
*   **Notification Management:** Users can mark notifications as read, and mark all notifications as read.

### 5. Database & Services
*   **MongoDB Integration:** MongoDB is the primary database, storing blog posts, comments, likes, follows, and notifications. `MongoDBService` encapsulates all database interactions.
*   **Supabase Integration:** Used for user authentication and management via `SupabaseService`.

## Tech Stack

*   **Framework:** ASP.NET Core / Blazor Server
*   **Language:** C#
*   **Database:** MongoDB
*   **Authentication:** Supabase
*   **UI Library:** MudBlazor
*   **Client-side Storage:** Blazored LocalStorage
*   **Real-time Communication:** SignalR (infrastructure in place, `NotificationHub` is a placeholder)
*   **External APIs:**
    *   Google Gemini API (for AI content generation)
    *   Google Custom Search API (for image searching)
*   **Containerization:** Docker (`Dockerfile`, `docker-compose.yml`)
*   **Deployment:** Configuration for Render.com (`render.yaml`) present.

## Project Structure

*   **`Data/`**: Contains database context (`MongoDBContext.cs`).
*   **`Models/`**: Defines data structures (e.g., `BlogPost.cs`, `Comment.cs`, `Like.cs`, `Notification.cs`).
*   **`Pages/`**: Contains Blazor components representing application pages (e.g., `CreatePost.razor`, `PostDetail.razor`, `Signin.razor`).
*   **`Services/`**: Contains service classes for business logic and data access (e.g., `MongoDBService.cs`, `AuthService.cs`, `NotificationService.cs`, `SupabaseService.cs`).
*   **`Shared/`**: Contains shared Blazor components (e.g., `MainLayout.razor`, `NotificationBadge.razor`).
*   **`Hubs/`**: Contains SignalR hubs (`NotificationHub.cs`).
*   **`wwwroot/`**: Contains static assets like CSS, images (including user-uploaded images in `wwwroot/uploads`).
*   **`Program.cs`**: Application entry point and service configuration.
*   **`appsettings.json`**: Configuration file for database connection strings, API keys, etc.

## Setup and Running the Application

(This section would typically contain instructions on how to set up the development environment, configure `appsettings.json` with necessary API keys and connection strings, and run the application. Based on the file structure, this would involve):

1.  **Prerequisites:**
    *   .NET SDK (version specified in global.json or project file, typically .NET 6 or later for this structure)
    *   MongoDB instance (local or cloud)
    *   Supabase account and project setup
    *   Google Cloud Platform project with Gemini API and Custom Search API enabled.
2.  **Configuration (`appsettings.json`):**
    *   `ConnectionStrings:MongoDB`: Connection string for your MongoDB instance.
    *   `ConnectionStrings:DatabaseName`: Name of the database in MongoDB (e.g., "BlogAppDB").
    *   `Supabase:Url`: Your Supabase project URL.
    *   `Supabase:ApiKey`: Your Supabase public API key.
    *   `Gemini:ApiKey`: Your API key for the Google Gemini API.
    *   `Google:ApiKey`: Your API key for Google Custom Search API.
    *   `Google:SearchEngineId`: Your Google Custom Search Engine ID.
    *   `ApiBaseUrl` (if different from default, used by HttpClient).
3.  **Restore Dependencies:**
    ```bash
    dotnet restore
    ```
4.  **Run the Application:**
    ```bash
    dotnet run
    ```
    Or use your IDE (Visual Studio, Rider) to run the project.

5.  **Docker (Optional):**
    *   Build the Docker image: `docker build -t blogapplication .`
    *   Run using Docker Compose: `docker-compose up` (ensure `docker-compose.yml` is correctly configured).
