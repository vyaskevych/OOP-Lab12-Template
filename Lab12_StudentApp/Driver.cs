namespace Lab12_StudentApp
{
    public class Driver
    {
        public string Name { get; set; }
        public int Experience { get; set; }

        public Driver(string name, int experience)
        {
            Name = name;
            Experience = experience;
        }

        // Уже реалізовано. Використовується у Завданні 8.
        public string Introduce()
        {
            return $"Я {Name}, стаж {Experience} р.";
        }
    }
}
