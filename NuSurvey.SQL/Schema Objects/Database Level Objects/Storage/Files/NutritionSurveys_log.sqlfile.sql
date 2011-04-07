ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [NutritionSurveys_log], FILENAME = 'E:\DB\NutritionSurveys_log.ldf', SIZE = 11264 KB, MAXSIZE = 2097152 MB, FILEGROWTH = 10 %);

