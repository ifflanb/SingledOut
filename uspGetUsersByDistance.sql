USE [SingledOut]
GO
/****** Object:  StoredProcedure [dbo].[uspGetUsersByDistance]    Script Date: 18/07/2014 10:40:30 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[uspGetUsersByDistance] 
    @Latitude float,
	@Longitude float,
	@Distance float
AS 

DECLARE @EarthsRadius int
SET @EarthsRadius = 6371;  -- earth's mean radius in km

SELECT u.ID, FirstName, Surname,  Sex, Latitude, Longitude
FROM UserLocations ul
JOIN Users u ON ul.UserID = u.ID
WHERE ACOS(COS(RADIANS(90-@Latitude))*COS(RADIANS(90-latitude)) +SIN(RADIANS(90-@Latitude)) *SIN(RADIANS(90-latitude))*COS(RADIANS(@Longitude-longitude)))*@EarthsRadius <= @Distance

