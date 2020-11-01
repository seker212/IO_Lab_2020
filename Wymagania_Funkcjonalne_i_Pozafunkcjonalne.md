# Wymagania Funkcjonalne I Pozafunkcjonalne

## Wymagania Funkcjonalne

Serwer odczytuje wiadomość przesłaną przez klienta.
Przyjmuje ciąg znaków i zwraca do klienta wiadomość z kodem ASCII reprezentującym otrzymaną wcześniej wiadomość. 
Serwer posiada modyfikowalną wielkość bufforu, adres IP na którym zostanie uruchomiony oraz port.
Serwer nie przyjmije więcej niż jednego połączenia, oraz przy jego zamknięciu kończy swoje działanie.
Znaki powrotu karetki oraz nowej linii nie są odtwarzane w formie kodów ASCII.

## Wymagania Pozafunkcjonalne

Aplikacja jest zbudowana w oparciu o .NET Framework 4.7.2, który jest wymagany do uruchomienia. Serwer pracuje synchronicznie.