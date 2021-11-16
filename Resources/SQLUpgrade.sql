----更新数据库版本号
if exists (select 1 from sys.extended_properties WHERE NAME = 'Version' and class_desc='DATABASE')
EXEC sys.sp_updateextendedproperty @name=N'Version', @value=N'1.0.1.2020010314' ;
else
EXEC sys.sp_addextendedproperty @name=N'Version', @value=N'1.0.3.20200103'
GO
