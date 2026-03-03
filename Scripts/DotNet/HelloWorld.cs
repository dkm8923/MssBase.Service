// DiceThrow.cs
var rng = new Random();
int roll = rng.Next(1, 7);
Console.WriteLine($"You rolled a {roll}!");