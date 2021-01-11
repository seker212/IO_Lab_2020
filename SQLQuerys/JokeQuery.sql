CREATE TABLE Joke(
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	CreatorID integer,
	Content varchar(255)
);

INSERT INTO Joke (CreatorID, Content)
VALUES (1, 'Gdzie podpisano traktat wersalski? \n- Na samym dole, pod tekstem.\r\n'),
(1, 'Co sie po jednej stronie glaszcze a po drugiej lize?? - Nie wiem. -... znaczek pocztowy!\r\n'),
(1, 'Co sie stanie jak walec drogowy przejedzie czlowieka? - Konwersja obrazu z 3D na 2D\r\n'),
(1, 'Jak sie nazywa mnich odpowiedzialny za podatki? - Brat PIT.\r\n'),
(1, 'Jaka jest ulubiona zabawa dzieci grabarza?  - W chowanego.\r\n'),
(1, 'Co robi elektryk na scenie? - Buduje napiecie.\r\n'),
(1, 'Przychodzi baba do lekarza, a lekarz tez baba.\r\n'),
(1, 'Jaki jest ulubiony owoc zolnierza? - Granat.\r\n');
