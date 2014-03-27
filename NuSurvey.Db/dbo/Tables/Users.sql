CREATE TABLE [dbo].[Users]
(
	[Id] VARCHAR(256) NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(200) NOT NULL, 
    [Title] VARCHAR(200) NOT NULL, 
    [Agency] VARCHAR(250) NOT NULL,
	[Street] VARCHAR(100) NOT NULL,  
    [City] VARCHAR(100) NOT NULL, 
    [State] VARCHAR(50) NOT NULL,     
    [Zip] VARCHAR(11) NOT NULL, 
    [TargetPopulationWic] BIT NOT NULL DEFAULT 0,
	[TargetPopulationSnap] BIT NOT NULL DEFAULT 0,
	[TargetPopulationHeadStart] BIT NOT NULL DEFAULT 0,
	[TargetPopulationEfnep] BIT NOT NULL DEFAULT 0,
	[TargetPopulationLowIncome] BIT NOT NULL DEFAULT 0,
	[TargetPopulationOther] BIT NOT NULL DEFAULT 0
)
