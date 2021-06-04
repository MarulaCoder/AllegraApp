Create Database MosheTestDB
Go

Use MosheTestDB
Go

CREATE TABLE Users (
    UserId INT IDENTITY (1, 1),
    Username VARCHAR (20) NOT NULL,
    CONSTRAINT PK_Users_UserId PRIMARY KEY (UserId)
);
Go

CREATE TABLE Exercises (
    ExerciseId INT IDENTITY (1, 1),
	UserId INT NOT NULL,
    Description VARCHAR (250) NOT NULL,
	Duration INT NOT NULL,
	Date DATETIME,
    CONSTRAINT PK_Exercises_ExerciseId PRIMARY KEY (ExerciseId),
	CONSTRAINT FK_Exercises_UserId FOREIGN KEY (UserId) REFERENCES Users (UserId)
);
Go
