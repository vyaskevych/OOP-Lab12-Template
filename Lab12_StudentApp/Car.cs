namespace Lab12_StudentApp
{
    public class Car
    {
        public string Model { get; }

        // Композиція. Властивості лише для читання (get без set): підмінити двигун
        // чи масив коліс ззовні не можна. Усе створюється в конструкторі (як у Лабораторній 11).
        public Engine CarEngine { get; }
        public Wheel[] Wheels { get; }

        // Агрегація: водій призначається ззовні й може бути null.
        public Driver? CarDriver { get; set; }

        // Одометр (км). Змінюється лише методом Drive().
        public double Mileage { get; private set; }

        public Car(string model)
        {
            Model = model;
            CarEngine = new Engine(150, "Бензин");

            Wheels = new Wheel[4];
            for (int i = 0; i < Wheels.Length; i++)
            {
                Wheels[i] = new Wheel(16);
            }
        }

        // Уже реалізовано (з Лабораторної 11).
        public Driver? RemoveDriver()
        {
            Driver? leaving = CarDriver;
            CarDriver = null;
            return leaving;
        }

        // TODO: Завдання 3. Делегування: чи працює двигун цього автомобіля.
        public bool IsEngineRunning
        {
            get { return false; }
        }

        // TODO: Завдання 3. Делегування: заглушіть двигун цього автомобіля.
        public void StopEngine()
        {

        }

        // TODO: Завдання 4. Завести двигун. Без водія двигун не заведеш:
        // якщо водія немає, двигун не запускається й метод повертає false.
        // Інакше двигун запускається й метод повертає true.
        public bool TryStartEngine()
        {
            return false;
        }

        // TODO: Завдання 5. Скільки шин спущено (Wheel.IsFlat). Від 0 до 4.
        public int CountFlatWheels()
        {
            return 0;
        }

        // TODO: Завдання 5. Підкачати КОЖНУ шину на bar бар (як на АЗС).
        public void PumpAllWheels(double bar)
        {

        }

        // TODO: Завдання 6. Замінити колесо на позиції index на newWheel.
        // Повертає зняте (старе) колесо. Якщо index поза межами масиву
        // або newWheel дорівнює null, нічого не змінюйте й поверніть null.
        public Wheel? ReplaceWheel(int index, Wheel newWheel)
        {
            return null;
        }

        // TODO: Завдання 7. Оновіть метод з Лабораторної 11. Тепер авто готове до поїздки,
        // лише якщо одночасно: є водій, двигун працює, жодна шина не спущена.
        public bool IsReadyToDrive()
        {
            return false;
        }

        // TODO: Завдання 7. Поїздка на km кілометрів. Повертає, скільки км проїхали.
        // Якщо km <= 0 або авто не готове до поїздки, поверніть 0 і нічого не змінюйте.
        // Інакше збільште Mileage на km і поверніть km.
        public double Drive(double km)
        {
            return 0;
        }
    }
}
