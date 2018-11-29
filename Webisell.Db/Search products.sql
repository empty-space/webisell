------------------------------------
--Example of search
------------------------------------
select * 
from Products_1
WHERE price BETWEEN 1 and 200000
AND Manufacturer in (1,2,4,5,6,11)
AND color in (11,12,3)
and internal_storage_size in (1,2,3,13)
and (connection_standart_2G = 1
	OR connection_standart_3G = 1)
ORDER BY price	
----
select *,
       Id as SpecificTableProductId,
       14 as CategoryId
from Products_14
 WHERE 
(Manufacturer IN (11,12))
AND 
(price BETWEEN 0 and 2000)
AND 
(color IN (11,12))
AND 
((connection_standart_2G=1) AND (connection_standart_3G=1))
ORDER BY price
