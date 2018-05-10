USE master; 
ALTER DATABASE LProject SET SINGLE_USER WITH ROLLBACK IMMEDIATE; 
DROP DATABASE LProject 
CREATE DATABASE LProject 

USE LProject 

CREATE TABLE dbo.People 
( 
ID_People int identity (1, 1) not null Primary Key, 
Pass nvarchar(50) not null, 
PersonName varchar(25) not null UNIQUE, 
IsAdmin bit null DEFAULT(0), 

) 

CREATE TABLE dbo.Session 
( 
ID_Session int identity (1, 1) not null Primary Key, 
Session_Name varchar(25) not null, 
IsArchived bit null DEFAULT(0), 
ArchTime date null, 
CreationDate date not null, 
ID_People int FOREIGN KEY REFERENCES dbo.People (ID_People) ON DELETE CASCADE
)

CREATE TABLE dbo.Route 
( 
ID_Route int identity (1, 1) not null Primary Key, 
RouteNumber int not null, 
Color varchar(50) not null, 
ID_Session int FOREIGN KEY REFERENCES dbo.Session (ID_Session) ON DELETE CASCADE
) 


CREATE TABLE dbo.Point 
( 
ID_Point int identity (1, 1) not null Primary Key, 
OrderNumber varchar(50) not null,
City varchar(50) not null, 
Street varchar(70) not null, 
House varchar(50) not null, 
Korp varchar(50) not null, 
Interval varchar(30), 
Comment varchar(500), 
PointNumber int, 
PointType int, 
ID_Route int FOREIGN KEY REFERENCES dbo.Route (ID_Route) ON DELETE CASCADE, 
ID_Session int FOREIGN KEY REFERENCES dbo.Session (ID_Session)
) 

