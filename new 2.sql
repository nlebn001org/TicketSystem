select * from Roles
select * from Tickets
select * from Users 

DBCC CHECKIDENT ('[Roles]', RESEED, 0);
DBCC CHECKIDENT ('[Tickets]', RESEED, 0);
DBCC CHECKIDENT ('[Users]', RESEED, 0);

delete from Roles
delete from Tickets
delete from Users 

insert into Roles (roleName)
values ('admin'),('support'),('customer')

insert into Users (Username, [Password], RoleId)
values ('admin', 'admin',1),
		('support', 'support',2),
		('customer', 'customer',3)

insert into Tickets (Title, CreatorId)
values('Test', 3)


if we have 
SqlException: Cannot insert duplicate key row in object 'dbo.Tickets' with unique index 'IX_Tickets_SolverId'. 
The duplicate key value is (2).
The statement has been terminated. ==> Manually delete nonclastered index IX_Tickets_SolverId from database

