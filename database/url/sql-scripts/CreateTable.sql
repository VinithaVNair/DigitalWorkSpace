use urldb
create table `TinyUrl`(
`ShortUrl` varchar(10) NOT NULL,
`OriginalUrl` varchar(5000) NOT NULL,
`Expiry` datetime NOT NULL,
`IsLinked` boolean
);