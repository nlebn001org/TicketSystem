select * from Roles
select * from Tickets
select * from Users 

DBCC CHECKIDENT ('[Roles]', RESEED, 0);

delete from Roles
delete from Tickets
delete from Users 

insert into Roles (roleName)
values ('admin'),('support'),('user')

insert into Users (Username, [Password], RoleId)
values ('admin', 'admin',1),
		('support', 'support',2),
		('customer', 'customer',3)

insert into Tickets (Title, CreatorId)
values('Test', 3)