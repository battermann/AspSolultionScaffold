CREATE TABLE [dbo].[ItemEvent]
(
	[StreamId] NVARCHAR(255) NOT NULL,
	[EventNumber] int NOT NULL,
	[TypeName] NVARCHAR(255) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[EventData] NVARCHAR(MAX) NOT NULL,
	CONSTRAINT Pk_ItemEvent PRIMARY KEY NONCLUSTERED ([StreamId], [EventNumber])
)
