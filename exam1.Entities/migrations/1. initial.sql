create database acceloka
go

USE acceloka
go

create table CategoryTicket(
	categoryID int identity not null primary key,
	categoryName varchar(25) not null
)

create table AvailableTicket(
	ticketCode varchar(7) not null primary key,
	categoryID int not null,
	ticketName varchar(255) not null,
	eventDate datetime not null,
	quota int not null,
	price int not null,

	foreign key (categoryID) references Categoryticket(categoryID)
)

create table BookedTicket(
	bookedTicketID int identity not null primary key,
	createdAt datetimeoffset not null default sysdatetimeoffset()
)

CREATE TABLE BookedTicketDetail(
    bookedTicketDetailId INT NOT NULL,
    ticketCode VARCHAR(7) NOT NULL,
    quantity INT NOT NULL,

    CONSTRAINT PK_BookedTicketDetail PRIMARY KEY (bookedTicketDetailId, ticketCode),
    CONSTRAINT FK_BookedTicketDetail_Ticket FOREIGN KEY (ticketCode) REFERENCES AvailableTicket(ticketCode),
    CONSTRAINT FK_BookedTicketDetail_BookedTicket FOREIGN KEY (bookedTicketDetailId) REFERENCES BookedTicket(bookedTicketId)
);
