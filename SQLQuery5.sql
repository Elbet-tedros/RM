create table tblMain
(
MainID int primary key identity,
aDate date,
aTime varchar(15),
TableName varchar(10),
WaiterName varchar(15),
status varchar(15),
orderType varchar(15),
total float,
recieved float,
change float,
)


create table tblDetails
(
DetailID int primary key identity,
MainID int,
proID int,
qty int,
price float,
amount float,
)

truncate table tblDetails;
truncate table tblMain;

select * from tblMain m 
inner join tblDetails d on m.MainID = d.MainID


SELECT MainID, status FROM tblMain ORDER BY MainID DESC


EXEC sp_help tblDetails;

SELECT proID, qty 
FROM tblDetails 
WHERE MainID = 1;

SELECT MainID, status, orderType 
FROM tblMain
ORDER BY MainID DESC;

SELECT *
FROM tblDetails;



CREATE TABLE ReceiptSequence
(
    ID INT IDENTITY(1,1) PRIMARY KEY
);

INSERT INTO ReceiptSequence DEFAULT VALUES;
SELECT SCOPE_IDENTITY();

INSERT INTO Orders (TableName, WaiterName, TotalAmount)
VALUES (@TableName, @WaiterName, @TotalAmount);

SELECT SCOPE_IDENTITY();


DELETE FROM tblMain;

DBCC CHECKIDENT ('tblMain', RESEED, 0);

DBCC CHECKIDENT ('tblMain');

DELETE FROM tblDetails;  -- first, because it references tblMain
DELETE FROM tblMain;

DBCC CHECKIDENT ('tblMain', RESEED, 0);
DBCC CHECKIDENT ('tblDetails', RESEED, 0);
