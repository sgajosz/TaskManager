CREATE TABLE users (
	id INT PRIMARY KEY IDENTITY (1, 1),
	email VARCHAR(50) NOT NULL,
	name VARCHAR(50) NOT NULL,
	surname VARCHAR(50) NOT NULL,
	password VARCHAR(50) NOT NULL
);

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

SELECT * FROM users;
DELETE FROM users

DROP TABLE projects;

CREATE TABLE projects (
	id INT PRIMARY KEY IDENTITY (1, 1),
	userFK INT FOREIGN KEY REFERENCES users(id),
	name VARCHAR(50) NOT NULL,
	creation datetime NOT NULL,
);

DROP PROCEDURE addProject;

CREATE PROCEDURE addProject
	@User int,
	@Name varchar(50),
	@Creation datetime
AS
BEGIN
	INSERT INTO projects(userFK, name, creation) VALUES (@User, @Name, @Creation)
END

DROP PROCEDURE getProjectsForUser;
CREATE PROCEDURE getProjectsForUser
	@User int
AS
BEGIN
	SELECT * FROM projects WHERE userFk = @User
END

DROP TABLE tasks;

CREATE TABLE tasks (
	id INT PRIMARY KEY IDENTITY (1, 1),
	projectFK INT FOREIGN KEY REFERENCES projects(id),
	userFK INT FOREIGN KEY REFERENCES users(id),
	name VARCHAR(50),
	technology VARCHAR(50) FOREIGN KEY REFERENCES technologies(name),
	errand VARCHAR(50),
	type VARCHAR(50),
	hours INT,
	doneHours INT,
	status VARCHAR(50),
	description VARCHAR(255),
);

SELECT * FROM tasks;

DROP PROCEDURE addTask;

CREATE PROCEDURE addTask
	@Project int,
	@User int,
	@Name varchar(50),
	@Technology varchar(50),
	@Errand varchar(50),
	@Type varchar(50),
	@Hours int,
	@DoneHours int,
	@Status varchar(50),
	@Description varchar(255)
AS
BEGIN
	INSERT INTO tasks(projectFK, userFK, name, technology, errand, type, hours, doneHours, status, description) VALUES (@Project, @User, @Name, @Technology, @Errand, @Type, @Hours, @DoneHours, @Status, @Description)
END

CREATE PROCEDURE editTask
	@ID int,
	@User int,
	@Name varchar(50),
	@Technology varchar(50),
	@Errand varchar(50),
	@Type varchar(50),
	@Hours int,
	@DoneHours int,
	@Status varchar(50),
	@Description varchar(255)
AS
BEGIN
	 UPDATE tasks SET userFK = @User, name = @Name, technology = @Technology, errand = @Errand, type = @Type, hours = @Hours, doneHours = @DoneHours, status = @Status, description = @Description WHERE id = @ID
END

DROP PROCEDURE getTaskForProject;

CREATE PROCEDURE getTaskForProject
	@Project int
AS
BEGIN
	SELECT * FROM tasks WHERE projectFK = @Project;
END

CREATE TABLE project_user_join (
	id INT PRIMARY KEY IDENTITY (1, 1),
	projectFK INT FOREIGN KEY REFERENCES projects(id),
	userFK INT FOREIGN KEY REFERENCES users(id)
);

CREATE TABLE technologies (
	name VARCHAR(50) PRIMARY KEY,
	projectFK INT FOREIGN KEY REFERENCES projects(id),
	price FLOAT
);

DROP TABLE technologies;

CREATE PROCEDURE addTechnology
	@Name varchar(50),
	@ProjectFK int,
	@Price float
AS
BEGIN
	INSERT INTO technologies(name, projectFK, price) VALUES (@Name, @ProjectFK, @Price)
END

SELECT * FROM tasks;

CREATE TABLE types (
	id INT PRIMARY KEY IDENTITY (1, 1),
	name VARCHAR(50),
	projectFK INT FOREIGN KEY REFERENCES projects(id)
);

CREATE PROCEDURE addType
	@Name varchar(50),
	@ProjectFK int
AS
BEGIN
	INSERT INTO types(name, projectFK) VALUES (@Name, @ProjectFK)
END

CREATE TABLE errands (
	id INT PRIMARY KEY IDENTITY (1, 1),
	name VARCHAR(50),
	projectFK INT FOREIGN KEY REFERENCES projects(id)
);

CREATE TABLE hours_history (
	id INT PRIMARY KEY IDENTITY (1, 1),
	userFK INT FOREIGN KEY REFERENCES users(id),
	taskFK INT FOREIGN KEY REFERENCES tasks(id),
	edited_at datetime,
	hours INT
);

SELECT * FROM hours_history;

CREATE PROCEDURE addHoursHistory
	@User as int,
	@Task as int,
	@Edited as datetime,
	@Hours as int
AS
BEGIN
	INSERT INTO hours_history(userFK, taskFK, edited_at, hours) VALUES (@User, @Task, @Edited, @Hours)
END

DELETE FROM hours_history WHERE taskFK in (SELECT id FROM tasks WHERE projectFK = 1);
DROP TABLE technologies, errands, types;

CREATE PROCEDURE addErrand
	@Name varchar(50),
	@ProjectFK int
AS
BEGIN
	INSERT INTO errands(name, projectFK) VALUES (@Name, @ProjectFK)
END

SELECT * FROM technologies;
SELECT * FROM errands;
SELECT * FROM types;
SELECT * FROM tasks;
DELETE FROM tasks;
SELECT * FROM users;
