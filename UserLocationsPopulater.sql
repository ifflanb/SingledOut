INSERT INTO [Users] ([FirstName],[Surname],[Sex],[CreatedDate],[UpdateDate],[Email])
VALUES ('User1','sur1','male',getdate(),getdate(),'test@test.com')
GO
INSERT INTO [UserLocations] ([UserID],[Latitude],[Longitude],[CreatedDate],[UpdateDate])
     VALUES (@@IDENTITY, -43.548339, 172.567666, getdate(), getdate())
GO

INSERT INTO [Users] ([FirstName],[Surname],[Sex],[CreatedDate],[UpdateDate],[Email])
VALUES ('User2','sur2','male',getdate(),getdate(),'test2@test.com')
GO
INSERT INTO [UserLocations] ([UserID],[Latitude],[Longitude],[CreatedDate],[UpdateDate])
     VALUES (@@IDENTITY, -43.579643, 172.566739, getdate(), getdate())
GO

INSERT INTO [Users] ([FirstName],[Surname],[Sex],[CreatedDate],[UpdateDate],[Email])
VALUES ('User3','sur3','male',getdate(),getdate(),'test3@test.com')
GO
INSERT INTO [UserLocations] ([UserID],[Latitude],[Longitude],[CreatedDate],[UpdateDate])
     VALUES (@@IDENTITY, -43.575407, 172.560251, getdate(), getdate())
GO

INSERT INTO [Users] ([FirstName],[Surname],[Sex],[CreatedDate],[UpdateDate],[Email])
VALUES ('User4','sur4','male',getdate(),getdate(),'test4@test.com')
GO
INSERT INTO [UserLocations] ([UserID],[Latitude],[Longitude],[CreatedDate],[UpdateDate])
     VALUES (@@IDENTITY, -43.572981, 172.556767, getdate(), getdate())
GO

INSERT INTO [Users] ([FirstName],[Surname],[Sex],[CreatedDate],[UpdateDate],[Email])
VALUES ('User5','sur5','male',getdate(),getdate(),'test5@test.com')
GO
INSERT INTO [UserLocations] ([UserID],[Latitude],[Longitude],[CreatedDate],[UpdateDate])
     VALUES (@@IDENTITY, -43.544839, 172.567498, getdate(), getdate())
GO

INSERT INTO [Users] ([FirstName],[Surname],[Sex],[CreatedDate],[UpdateDate],[Email])
VALUES ('User6','sur6','male',getdate(),getdate(),'test6@test.com')
GO
INSERT INTO [UserLocations] ([UserID],[Latitude],[Longitude],[CreatedDate],[UpdateDate])
     VALUES (@@IDENTITY, -43.550962, 172.598623, getdate(), getdate())
GO
 