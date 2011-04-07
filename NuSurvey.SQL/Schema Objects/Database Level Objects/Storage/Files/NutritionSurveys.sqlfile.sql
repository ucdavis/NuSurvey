ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [NutritionSurveys], FILENAME = 'E:\DB\NutritionSurveys.mdf', SIZE = 2048 KB, FILEGROWTH = 1024 KB) TO FILEGROUP [PRIMARY];

