namespace Lab12_StudentApp
{
    public class Wheel
    {
        // Тиск у шині вимірюється в барах.
        public const double MinPressure = 1.5;   // нижче цього шина вважається спущеною
        public const double MaxPressure = 3.0;   // вище накачати не можна

        public int Radius { get; set; }

        // Тиск у шині. Змінюється лише методами Inflate() і Puncture().
        public double Pressure { get; private set; }

        // Уже реалізовано: шина спущена, якщо тиск нижче мінімального.
        public bool IsFlat => Pressure < MinPressure;

        public Wheel(int radius)
        {
            Radius = radius;
            Pressure = 2.2;                      // нове колесо накачане до норми
        }

        // Уже реалізовано: імітація проколу, тиск падає до нуля.
        public void Puncture()
        {
            Pressure = 0;
        }

        // TODO: Завдання 2. Підкачування шини на bar бар.
        // Тиск зростає на bar, але не може перевищити MaxPressure.
        // Якщо bar <= 0, нічого не змінюйте.
        public void Inflate(double bar)
        {

        }
    }
}
