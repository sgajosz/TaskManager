CREATE TABLE users (
	id INT PRIMARY KEY IDENTITY (1, 1),
	email VARCHAR(50) NOT NULL,
	name VARCHAR(50) NOT NULL,
	surname VARCHAR(50) NOT NULL,
	password VARCHAR(50) NOT NULL
);

SELECT * FROM users;
DELETE FROM users

CREATE PROCEDURE addUser
	@Email varchar(50),
	@Name varchar(50),
	@Surname varchar(50),
	@Password varchar(50)
AS
BEGIN
	INSERT INTO users (email, name, surname, password) VALUES (@Email, @Name, @Surname, @Password)
END

CREATE PROCEDURE getUser
	@Email varchar(50),
	@Password varchar(50)
AS
BEGIN
	SELECT * FROM users WHERE email=@Email and password = @Password
END