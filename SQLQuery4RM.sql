Create table staff
(
staffID int primary key identity,
sName varchar(50),
sPhone varchar(50),
sRole varchar(50)

)

create table products 
(
pID int primary key identity,
pName varchar(50),
pPrice float,
CategoryID int,
pImage image


)

DROP TABLE products;



select pID,pName,pPrice,CategoryID,c.catName from products p inner join category c on c.catID = p.CategoryID

SELECT DB_NAME();

SELECT TOP 10 *
FROM products
ORDER BY pID ASC;

DELETE FROM products
WHERE pID IN (
    SELECT TOP 10 pID
    FROM products
    ORDER BY pID ASC
);

DELETE FROM staff;