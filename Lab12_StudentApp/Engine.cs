namespace Lab12_StudentApp
{
    public class Engine
    {
        public int Power { get; set; }
        public string FuelType { get; set; }

        // Стан двигуна. Змінити його ззовні не можна (private set):
        // це роблять лише методи Start() і Stop().
        public bool IsRunning { get; private set; }

        public Engine(int power, string fuelType)
        {
            Power = power;
            FuelType = fuelType;
        }

        // TODO: Завдання 1. Запуск двигуна: після виклику IsRunning дорівнює true.
        public void Start()
        {

        }

        // TODO: Завдання 1. Зупинка двигуна: після виклику IsRunning дорівнює false.
        public void Stop()
        {

        }
    }
}
