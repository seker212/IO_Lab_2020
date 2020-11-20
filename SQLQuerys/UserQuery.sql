CREATE TABLE Users(
	ID int NOT NULL AUTOINCREMENT,
	Login varchar(255),
	Password varchar(255),
	isAdmin BOOLEAN,
	PRIMARY KEY(ID)
)

INSERT INTO Users (Login, Password, isAdmin)
VALUES ('admin', 'admin', TRUE);
