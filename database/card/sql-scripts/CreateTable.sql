use cardsdb
create table `Card`(
`Id` int,
`Version` int,
`ShortUrl` varchar(10),
`Title` varchar(50),
`Description` varchar(150),
`ImageContent` blob,
`Favicon` varchar(200),
`IsLinked` boolean
);

create table `LinkedCard`(
`Id` int,
`Version` int,
`CatalogId` int
);