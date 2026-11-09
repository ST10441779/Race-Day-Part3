# RaceDay – Part 3

## ASP.NET Core MVC Web Application

RaceDay is a web-based event management application developed using ASP.NET Core MVC. The application provides a platform for organisers to manage sporting events and for participants to explore events, enrol, and view their results.

The Part 3 application uses the REST API developed in Part 2 to retrieve and manage application data.

## Features

### Organiser
- Secure login using the organiser account.
- View the organiser dashboard.
- View and manage sporting events.
- Create, edit and delete events.
- Manage event categories.
- View participant enrolments.
- Manage and record event results.
- Update profile information.

### Participant
- Register and log in.
- Browse available sporting events.
- View event details and categories.
- Enrol in events.
- View personal results.
- Update profile information.

### General
- Session-based authentication.
- Role-specific navigation for organisers and participants.
- REST API integration.
- Responsive user interface.
- Consistent sporty design and styling.
- Success and error notifications.

## Technologies Used

- C#
- ASP.NET Core MVC
- ASP.NET Core Web API
- HTML5
- CSS3
- Bootstrap
- JavaScript
- REST API
- Git and GitHub


## Project Structure

The solution is organised into the following main components:

- **RaceDay.Mvc** – The ASP.NET Core MVC front end.
- **src/RaceDay.Api** – The REST API responsible for application data and business operations.


## Requirements

To run the project locally, you will need:

- Visual Studio 2022 or later.
- .NET SDK compatible with the project.
- A web browser.
- Git (optional, for version control).

## Installation and Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/ST10441779/RaceDay-Part-3.git
   ```

2. Open the downloaded project folder.

3. Open the solution file in Visual Studio.

4. Restore the NuGet packages.

5. Ensure the API base address configured in the MVC application points to the running RaceDay API.

6. Start the API project.

7. Start the MVC project.

8. Open the local application URL displayed by Visual Studio.

## Running the Application

The API must be running for the MVC application to retrieve and manage data.

The recommended startup order is:

1. Start the RaceDay API.
2. Start the RaceDay MVC application.
3. Register or log in using the appropriate account.
4. Navigate through the organiser or participant interface.


## Project Purpose

The purpose of RaceDay is to simplify the management of sporting events by providing separate interfaces for event organisers and participants.

The application demonstrates the integration of an ASP.NET Core MVC front end with a REST API, session-based authentication, role-specific functionality and responsive web design.

