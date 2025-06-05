CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;

CREATE TABLE `WaitingLists` (
    `Id` char(36) NOT NULL,
    `Name` varchar(255) NOT NULL,
    `TotalSeats` int NOT NULL,
    `TimeForService` int NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Parties` (
    `Id` char(36) NOT NULL,
    `Name` longtext NOT NULL,
    `Size` int NOT NULL,
    `ServiceEndedAt` datetime(6) NULL,
    `ServiceStartedAt` datetime(6) NULL,
    `CreatedOn` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `CheckedIn` tinyint(1) NOT NULL DEFAULT FALSE,
    `SessionId` longtext NOT NULL,
    `WaitingListId` char(36) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Parties_WaitingLists_WaitingListId` FOREIGN KEY (`WaitingListId`) REFERENCES `WaitingLists` (`Id`) ON DELETE CASCADE
);

CREATE INDEX `IX_Parties_WaitingListId` ON `Parties` (`WaitingListId`);

CREATE UNIQUE INDEX `IX_WaitingLists_Name` ON `WaitingLists` (`Name`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250605151602_InitialCreate', '8.0.11');

COMMIT;

