## **Pre-Requisites**
Make sure you have the following installed:
1. [Docker](https://www.docker.com/get-started) (with Docker Compose)
2. [Git](https://git-scm.com/)

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


## About this web-app
Layered in 3 projects.
One for the API calls
One for the contracts (DTOs)
One for the backend with EntityFramework MySQL (basically two layers in one project)

Besides these projects there are also middleware classes and background services, these would be good to be placed in their own projects as well: WaitingList.BackgroundServices, WaitingList.Middleware and also WaitingList.Database
But I kept it simplified since it is a simple project while still hoping to showcase know-of.

Starting the app will ensure the database is empty and ready for an empty waiting list.
Since we only have one, it is retrieved by name.

Session cookies are being used to keep track of spots on the waiting list, though JWT would have been a more secure way, for this project it suffices.
After service a background service will put the end time of service and the user can sign up again.

I went with a coding style where throwing exceptions is a last-ditch effort. The client also shows system errors as a snackbar error and user / validation errors right above where the user is looking on the screen.
Using interfaces was a bit of an overkill for the repositories and services but it was to show that I usually use these when more generic methods are introduced for more repositories; like Save and Get.
