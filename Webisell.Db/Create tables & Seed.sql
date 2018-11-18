
DROP PROCEDURE IF EXISTS Find_or_Create_FilterValue
DROP PROCEDURE IF EXISTS Find_or_Create_FilterValue_byID
DROP PROCEDURE IF EXISTS Insert_Product_category_id
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
CREATE TABLE dbo.Products_category_id(
 Id int IDENTITY(1,1) PRIMARY KEY, 
 ProductId int FOREIGN KEY  REFERENCES Products(ProductId), 
 Name nvarchar(250) NOT NULL,
 Price int,
 Available bit DEFAULT 1,
 Rating int DEFAULT 0,
 --for Product detail page and comparison page
 Data nvarchar(max) NULL, --json
 --Filters:
 --mandatory
 Manufacturer int FOREIGN KEY  REFERENCES FilterValues(FilterValueId),
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

CREATE PROCEDURE Insert_Product_category_id (
 @categoryId int,
 @name nvarchar(50),
 @data nvarchar(max) --json   
)  AS  
BEGIN  	
	--Product
	INSERT INTO dbo.Products(CategoryId) 
	VALUES (@categoryId)
	DECLARE @ProductID int = @@IDENTITY
	DECLARE @price int = CONVERT(int, JSON_VALUE(@data, '$.price')) 
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

	--Product_category_id
	INSERT INTO Products_category_id (ProductId,Name,Price,Data,Manufacturer,color,operating_system)
	VALUES(
		@ProductID,
		@name,
		@price,
		@data,
		@manufacturerId,
		@colorId,
		@operating_systemId
	)
	RETURN
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
	INSERT INTO Categories(Name)
	VALUES ('category_seed_sql')
	DECLARE @categoryId int = @@IDENTITY

	INSERT INTO FilterTypes(Name)
	VALUES ('FilterValue OR')
	DECLARE @filterTypeId int = @@IDENTITY
	INSERT INTO FilterTypes(Name)
	VALUES ('FilterValue AND')
	INSERT INTO FilterTypes(Name)
	VALUES ('Multiple Columns OR')
	INSERT INTO FilterTypes(Name)
	VALUES ('Integer OR')

	INSERT INTO Filters(CategoryId,Name,FilterTypeId)
	VALUES (@categoryId,'manufacturer',@filterTypeId)

	INSERT INTO Filters(CategoryId,Name,FilterTypeId)
	VALUES (@categoryId,'color',@filterTypeId)

	INSERT INTO Filters(CategoryId,Name,FilterTypeId)
	VALUES (@categoryId,'operating_system',@filterTypeId)

	exec Insert_Product_category_id @categoryId,'Samsung S8 seed sql',@data
	PRINT IDENT_CURRENT('Products_category_id')
	PRINT SCOPE_IDENTITY()
	PRINT @@IDENTITY
	exec Insert_Product_category_id @categoryId,'Apple IPhone 5s',@data2	
	PRINT @@IDENTITY
END

GO

SELECT * FROM Products_category_id
SELECT * FROM Filters
SELECT * FROM FilterValues
SELECT * FROM Categories