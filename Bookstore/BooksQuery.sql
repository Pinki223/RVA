Create table Authors
(
	AuthorId int primary key identity,
	ShortDesc text
)
Create table Books
(
	BookId int primary key identity,
	Title text,
	PublishYear int,
	AuthorsId int foreign key references Authors(AuthorId),
	RentedTo varchar(50) foreign key references Members(Username)
)
Create table Members
(
	Username varchar(50) primary key,
	FirstName text,
	LastName text,
)