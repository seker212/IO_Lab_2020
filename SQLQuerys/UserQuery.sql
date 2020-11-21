CREATE TABLE Users(
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	Login varchar(255),
	Password varchar(255),
	isAdmin BOOLEAN,
)

INSERT INTO Users (Login, Password, isAdmin)
VALUES ('admin', 'admin', TRUE);
