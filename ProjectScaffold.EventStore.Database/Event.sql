CREATE TABLE [dbo].[Event]
(
	[StreamId] NVARCHAR(255) NOT NULL,
	[EventNumber] int NOT NULL,
	[TypeName] NVARCHAR(255) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[EventData] NVARCHAR(MAX) NOT NULL,
	CONSTRAINT Pk_Event PRIMARY KEY NONCLUSTERED ([StreamId], [EventNumber])
)
