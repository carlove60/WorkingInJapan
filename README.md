## Welcome!
Thank you for taking the time and effort to read, understand and review my code!
This is a coding assignment and for that reason there is no protection nor will this be maintained. 
Also, the commit messages will probably be very nonsensical since I am the only one working on this repo and have no intention on working on this further.

## **Pre-Requisites**
Make sure you have the following installed:
[Docker](https://www.docker.com/get-started) (with Docker Compose)
[Git](https://git-scm.com/)

## **How to Run**
To get started quickly with Docker, follow these steps:
### **1. Clone the Repository**
First, clone the project to your local machine:

### **. Build and Start the Containers**
Use the following command to build and start the Docker containers for the API and MySQL database:
``` bash
docker-compose up --build
```
- The `--build` flag ensures the Docker images are rebuilt if there are changes in the code.
- Docker Compose will spin up:
    - `waitinglist-api`: The API service running on port `5240`.
    - `db`: The MySQL database instance on port `3306`.
#### API Logs:
You can view the running API logs to ensure everything is working:
``` bash
docker-compose logs waitinglist-api
```
#### MySQL:
The database is accessible from inside the `waitinglist-network` with the following credentials:
- **Host:** `db`
- **Port:** `3306`
- **User:** `root`
- **Password:** `qh734hsr05`
- **Database:** `WaitingList`

#### Troubleshooting:
3306 has to be available for the database to run. Ensure yours is free before you build the app

## UPDATE
SSE support has been added under the open branch.
Can be ignored for this code review but I wasn't satisfied with the current implementation.

## About this web-app
Layered in 6 projects.
- One for the API calls (entry point, Program.cs)
- One for the backend (used by the API layer)
- One for the background services (initiated by the API layer)
- One for the contracts (DTOs, used by the API and backend layers)
- One for the database (used by the backend layer)
- One for for tests (using the backend, contract and contract layers)

Besides these projects there are also middleware classes that are used by the session, I decided to keep them in the API layer.

Starting the app will ensure the database is empty and ready for an empty waiting list.
Since we only have one, it is retrieved by name.

Session cookies are being used to keep track of spots on the waiting list, though JWT would have been a more secure way, for this project it suffices.
After service a background service will put the end time of service and the user can sign up again. Just in case users are inactive, a background service will remove users from the queue after 10 minutes.

I went with a coding style where throwing exceptions is a last-ditch effort. The client also shows system errors as a snackbar error and user / validation errors right above where the user is looking on the screen.
Using interfaces was a bit of an overkill for the repositories and services but it was to show that I usually use these when more generic methods are introduced for more repositories; like Save and Get.

The Database uses a code-first approach to get generated, giving a single point to check for all relations. Between the Backend and Database Entities get passed between them. The API layer only gets DTOs, to ensure to take away complexity and only showing relevant data to the receiver/user. The services are in place for data manipulation and the repositories' only job is CRUD.

# Flow
The user immediately lands on the WaitingList page, which is retrieved by name, if there are more restaurants, a new table would be created to add these and link the waiting lists. A domain driven approach.
The page first checks if there is a party already registered with the current setting. If so, it shows the Party component, else it will show the waiting list. 

The app supports multiple sessions, then can be tested with using multiple private tabs / browsers. If a party of 10 puts themselves on the waiting list the other session will not be allowed to sign-up anymore. 
Only when the first party checks in, will there be space again on the waiting list. While being on the waiting list the user has the option to remove themselves off the list. Polling in the background ensures the user will see the check-in screen when they're up to enter the restaurant. After their service is done, they'll be able to go back to the waiting list page.









