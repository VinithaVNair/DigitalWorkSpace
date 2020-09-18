use catalogdb
create table `Card`(
`Id` int,
`Version` int,
`CatalogId` int
);

create table `Admin`(
`Id` int,
`CatalogId` int
);

create table `PendingCard`(
`Id` int,
`Version` int,
`CatalogId` int
);

create table `Catalog`(
`Id` int NOT NULL AUTO_INCREMENT,
`Name` varchar(50),
PRIMARY KEY (`Id`)
);