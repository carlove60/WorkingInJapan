CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;
IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE TABLE `AddressModel` (
        `Id` char(36) NOT NULL,
        `Street` longtext NOT NULL,
        `City` longtext NOT NULL,
        `State` longtext NOT NULL,
        `ZipCode` longtext NOT NULL,
        `Country` longtext NOT NULL,
        PRIMARY KEY (`Id`)
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE TABLE `AspNetRoles` (
        `Id` varchar(255) NOT NULL,
        `Name` varchar(256) NULL,
        `NormalizedName` varchar(256) NULL,
        `ConcurrencyStamp` longtext NULL,
        PRIMARY KEY (`Id`)
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE TABLE `AspNetUsers` (
        `Id` varchar(255) NOT NULL,
        `UserName` varchar(256) NULL,
        `NormalizedUserName` varchar(256) NULL,
        `Email` varchar(256) NULL,
        `NormalizedEmail` varchar(256) NULL,
        `EmailConfirmed` tinyint(1) NOT NULL,
        `PasswordHash` longtext NULL,
        `SecurityStamp` longtext NULL,
        `ConcurrencyStamp` longtext NULL,
        `PhoneNumber` longtext NULL,
        `PhoneNumberConfirmed` tinyint(1) NOT NULL,
        `TwoFactorEnabled` tinyint(1) NOT NULL,
        `LockoutEnd` datetime NULL,
        `LockoutEnabled` tinyint(1) NOT NULL,
        `AccessFailedCount` int NOT NULL,
        PRIMARY KEY (`Id`)
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE TABLE `AspNetRoleClaims` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `RoleId` varchar(255) NOT NULL,
        `ClaimType` longtext NULL,
        `ClaimValue` longtext NULL,
        PRIMARY KEY (`Id`),
        CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE TABLE `AspNetUserClaims` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `UserId` varchar(255) NOT NULL,
        `ClaimType` longtext NULL,
        `ClaimValue` longtext NULL,
        PRIMARY KEY (`Id`),
        CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE TABLE `AspNetUserLogins` (
        `LoginProvider` varchar(255) NOT NULL,
        `ProviderKey` varchar(255) NOT NULL,
        `ProviderDisplayName` longtext NULL,
        `UserId` varchar(255) NOT NULL,
        PRIMARY KEY (`LoginProvider`, `ProviderKey`),
        CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE TABLE `AspNetUserRoles` (
        `UserId` varchar(255) NOT NULL,
        `RoleId` varchar(255) NOT NULL,
        PRIMARY KEY (`UserId`, `RoleId`),
        CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE TABLE `AspNetUserTokens` (
        `UserId` varchar(255) NOT NULL,
        `LoginProvider` varchar(255) NOT NULL,
        `Name` varchar(255) NOT NULL,
        `Value` longtext NULL,
        PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
        CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE TABLE `Users` (
        `Id` char(36) NOT NULL,
        `FirstName` longtext NOT NULL,
        `LastName` longtext NOT NULL,
        `PhoneNumber` longtext NOT NULL,
        `DateOfbirth` datetime(6) NOT NULL,
        `AddressId` char(36) NOT NULL,
        `Language` int NOT NULL,
        `Role` longtext NOT NULL,
        `IsLoggedIn` tinyint(1) NOT NULL,
        `IdentityUserId` varchar(255) NULL,
        PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Users_AddressModel_AddressId` FOREIGN KEY (`AddressId`) REFERENCES `AddressModel` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_Users_AspNetUsers_IdentityUserId` FOREIGN KEY (`IdentityUserId`) REFERENCES `AspNetUsers` (`Id`)
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE TABLE `Resumes` (
        `Id` char(36) NOT NULL,
        `Title` longtext NOT NULL,
        `Content` longtext NOT NULL,
        `UserId` char(36) NOT NULL,
        PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Resumes_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX `IX_Resumes_UserId` ON `Resumes` (`UserId`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE INDEX `IX_Users_AddressId` ON `Users` (`AddressId`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    CREATE INDEX `IX_Users_IdentityUserId` ON `Users` (`IdentityUserId`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250313174813_InitialCreate')
BEGIN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250313174813_InitialCreate', '9.0.2');
END;

COMMIT;

