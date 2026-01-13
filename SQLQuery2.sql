Select * from tblMain m
inner join tblDetails d on m.MainID = d.MainID
inner join products p on p.pID = d.proID
inner join category c on c.catID = p.CategoryID
where m.aData between '2026-01-01' and '2026-01-30' 


ALTER TABLE tblMain
ADD PaidDate DATETIME NULL;

Select * from tblMain;

Select * from tblDetails;



