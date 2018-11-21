
--Delete all Product_{X} tables
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += '
DROP TABLE IF EXISTS ' 
    + QUOTENAME(s.name)
    + '.' + QUOTENAME(t.name) + ';'
    FROM sys.tables AS t
    INNER JOIN sys.schemas AS s
    ON t.[schema_id] = s.[schema_id] 
    WHERE t.name LIKE 'Products[_]%';

EXEC sp_executesql @sql;

DROP PROCEDURE IF EXISTS Find_or_Create_FilterValue
DROP PROCEDURE IF EXISTS Find_or_Create_FilterValue_byID
DROP PROCEDURE IF EXISTS Insert_Product_category_id
DROP PROCEDURE IF EXISTS Insert_Product_0
DROP TABLE IF EXISTS dbo.Products_category_id
DROP TABLE IF EXISTS dbo.Products
DROP TABLE IF EXISTS dbo.FilterValues
DROP TABLE IF EXISTS dbo.Filters
DROP TABLE IF EXISTS dbo.FilterTypes
DROP TABLE IF EXISTS dbo.Categories
GO

CREATE TABLE dbo.Categories(
 CategoryId int IDENTITY(1,1) PRIMARY KEY ,
 Name nvarchar(250) NOT NULL,
 ) 
CREATE TABLE dbo.FilterTypes(
 FilterTypeId int IDENTITY(1,1) PRIMARY KEY ,
 Name nvarchar(250) NOT NULL
 ) 

CREATE TABLE dbo.Filters(
 FilterId int IDENTITY(1,1) PRIMARY KEY ,
 CategoryId int NOT NULL  FOREIGN KEY  REFERENCES Categories(CategoryId), 
 FilterTypeId int NOT NULL FOREIGN KEY  REFERENCES FilterTypes(FilterTypeId), 
 Name nvarchar(250) NOT NULL,
 IsSystem bit NOT NULL DEFAULT 0,
 ) 

CREATE TABLE dbo.FilterValues(
 FilterValueId int IDENTITY(1,1) PRIMARY KEY ,
 FilterId int NOT NULL FOREIGN KEY  REFERENCES Filters(FilterId), 
 Value nvarchar(250) NOT NULL,
 ) 



CREATE TABLE dbo.Products(
 ProductId int IDENTITY(1,1) PRIMARY KEY, 
 CategoryId int FOREIGN KEY  REFERENCES Categories(CategoryId), 
) 
CREATE TABLE dbo.Products_0(
 Id int IDENTITY(1,1) PRIMARY KEY, 
 ProductId int FOREIGN KEY  REFERENCES Products(ProductId), 
 Name nvarchar(250) NOT NULL,
 Available bit DEFAULT 1,
 Rating int DEFAULT 0,
 --for Product detail page and comparison page
 Data nvarchar(max) NULL, --json
 --Filters:
 --mandatory
 Manufacturer int FOREIGN KEY  REFERENCES FilterValues(FilterValueId),
 price as CONVERT(int, JSON_VALUE(Data, '$.price')),
 --depends on category
 operating_system int FOREIGN KEY  REFERENCES FilterValues(FilterValueId), 
 color int FOREIGN KEY  REFERENCES FilterValues(FilterValueId), 
 --ComputedOS AS Find_or_Create_FilterValue('operating_system',JSON_VALUE(Data, '$.operating_system')) Persisted ,  
 --connection_standart_2g AS (CONTAINS(JSON_VALUE(Data, '$.operating_system'),'2g')) Persisted
 connection_standart_2g AS CASE WHEN   (JSON_QUERY(Data, '$.connection_standart') like '%2g%') THEN 1 ELSE 0 END  Persisted,
 connection_standart_3g AS CASE WHEN   (JSON_QUERY(Data, '$.connection_standart') like '%3g%') THEN 1 ELSE 0 END  Persisted,
 connection_standart_4g AS CASE WHEN   (JSON_QUERY(Data, '$.connection_standart') like '%4g%') THEN 1 ELSE 0 END  Persisted,
 connection_standart_cdma AS CASE WHEN (JSON_QUERY(Data, '$.connection_standart') like '%cdma%') THEN 1 ELSE 0 END  Persisted
) 

GO

CREATE PROCEDURE Find_or_Create_FilterValue (
	@categoryId int, @name nvarchar(250), @value nvarchar(250), 
	@FilterValueId int output)  AS  
BEGIN  
	DECLARE @FilterId int = NULL  

	SELECT @FilterId = FilterId
	FROM  Filters f  
	WHERE f.Name=@name and f.CategoryId = @categoryId

	SELECT @FilterValueId = FilterValueId
	FROM FilterValues fv
	WHERE fv.FilterId = @FilterId and fv.Value = @value
	
	IF(@FilterValueId IS NULL)
	BEGIN
		INSERT INTO FilterValues( FilterId , Value ) 
		VALUES (@FilterId, @value)
		SET @FilterValueId = @@IDENTITY 
	END
	RETURN   
END  
GO

CREATE PROCEDURE Find_or_Create_FilterValue_byID (@FilterId int, @value nvarchar(250), @FilterValueId int output)  AS  
BEGIN  	
	SELECT @FilterValueId = FilterValueId
	FROM FilterValues fv
	WHERE fv.FilterId = @FilterId and fv.Value = @value
	
	IF(@FilterValueId IS NULL)
	BEGIN
		INSERT INTO FilterValues( FilterId , Value ) 
		VALUES (@FilterId, @value)
		SET @FilterValueId = @@IDENTITY 
	END
	RETURN  
END  

GO

CREATE PROCEDURE Insert_Product_0 (
 @name nvarchar(50),
 @data nvarchar(max) --json   
)  AS  
BEGIN  	
	DECLARE @categoryId int = 0
	--Product
	INSERT INTO dbo.Products(CategoryId) 
	VALUES (@categoryId)
	DECLARE @ProductID int = @@IDENTITY	
	--Filters
	--GENERATED DYNAMICLY:
	DECLARE @color nvarchar(max) = JSON_VALUE(@data, '$.color') 
	DECLARE @operating_system nvarchar(max) =JSON_VALUE(@data, '$.operating_system') 
	DECLARE @manufacturer nvarchar(max) = JSON_VALUE(@data, '$.manufacturer')	
	DECLARE @colorId int = NULL
	DECLARE @operating_systemId int = NULL
	DECLARE @manufacturerId int = NULL
	--...others filter columns...
	exec Find_or_Create_FilterValue @categoryId,'color', @color,@colorId OUTPUT
	exec Find_or_Create_FilterValue @categoryId,'operating_system', @operating_system, @operating_systemId OUTPUT
	exec Find_or_Create_FilterValue @categoryId,'manufacturer', @manufacturer, @manufacturerId OUTPUT
	-- multiple value Filters 
	--  = are computed persistent columns

	--Product_0
	INSERT INTO Products_0 (ProductId,Name,Data,Manufacturer,color,operating_system)
	VALUES(
		@ProductID,
		@name,
		@data,
		@manufacturerId,
		@colorId,
		@operating_systemId
	)	
END  

GO

DECLARE @data nvarchar(max)= '{	
	"price":"1000",	
	"manufacturer":"Samsung",	
	"operating_system":"Android",
	"camera_resolution":20, 
	"screen_size":5, 
	"color":"black",
	"features":["smartphone"],
	"internal_storage_size": 32, 
	"battery": 1750,
	"connection_standart": ["2G","3G","4G","CDMA"]	
}'
DECLARE @data2 nvarchar(max)= '{	
	"price":"1005",	
	"manufacturer":"Apple",	
	"operating_system":"iOS",
	"camera_resolution":24, 
	"screen_size":5, 
	"color":"white",
	"features":["smartphone","excellent camera"],
	"internal_storage_size": 32, 
	"battery": 1750,
	"connection_standart": ["2G","3G","4G","LTE"]				
}'
IF NOT EXISTS (SELECT * FROM Categories WHERE Name = 'category_seed_sql')
BEGIN
	--SEED
	SET IDENTITY_INSERT Categories ON;

	INSERT INTO Categories(CategoryId,Name)
	VALUES (0,'category_seed_sql')	
	DECLARE @categoryId int = 0

	SET IDENTITY_INSERT Categories OFF;

	SET IDENTITY_INSERT FilterTypes ON;
	INSERT INTO FilterTypes(FilterTypeId,Name)
	VALUES (1,'FilterValue OR')
	INSERT INTO FilterTypes(FilterTypeId,Name)
	VALUES (2,'Multiple Columns OR')
	INSERT INTO FilterTypes(FilterTypeId,Name)
	VALUES (3,'Multiple Columns AND')
	INSERT INTO FilterTypes(FilterTypeId,Name)
	VALUES (4,'Integer OR')
	INSERT INTO FilterTypes(FilterTypeId,Name)
	VALUES (5,'Integer Range')

	SET IDENTITY_INSERT FilterTypes OFF;

	INSERT INTO Filters(CategoryId,Name,FilterTypeId,IsSystem)
	VALUES (@categoryId,'price',5,1)

	INSERT INTO Filters(CategoryId,Name,FilterTypeId)
	VALUES (@categoryId,'manufacturer',1)

	INSERT INTO Filters(CategoryId,Name,FilterTypeId)
	VALUES (@categoryId,'color',1)

	INSERT INTO Filters(CategoryId,Name,FilterTypeId)
	VALUES (@categoryId,'operating_system',1)

	exec Insert_Product_0 'Samsung S8 seed sql',@data
	PRINT IDENT_CURRENT('Products_0')
	PRINT SCOPE_IDENTITY()
	PRINT @@IDENTITY
	exec Insert_Product_0 'Apple IPhone 5s',@data2	
	PRINT @@IDENTITY
END

GO

SELECT * FROM Products_0
SELECT * FROM Filters
SELECT * FROM FilterValues
SELECT * FROM Categories
SELECT * FROM Products_11

SELECT  ProductId,
		Id as Specific_Table_Product_Id, 
		Name,
		price,
		Available,
		Data as JsonData
FROM Products_0


