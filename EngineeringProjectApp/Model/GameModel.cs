namespace EngineeringProjectApp
{
    public class GameModel
    {
        public int GameId { get; set; }
        public int UserId { get; set; }
        public string Date { get; set; }
        public string Level { get; set; }
        public int Returning { get; set; }
        public int AmountOfButterflies { get; set; }
        public int AmountOfBirds { get; set; }
        public int Time { get; set; }
        public int Velocity { get; set; }

        public override string ToString()
        {
            return GameId + " " + UserId + " " + Date + " " + Level + " " + Returning + " " + AmountOfButterflies + " " + AmountOfBirds + " " + Time + " " + Velocity;
        }
    }
}
