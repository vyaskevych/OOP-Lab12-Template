using System.Reflection;
using Xunit;
using Lab12_StudentApp;

namespace Lab12_Tests
{
    public class DelegationTests
    {
        private static bool Near(double expected, double actual) => Math.Abs(expected - actual) < 1e-9;

        // Автомобіль, готовий їхати: є водій і двигун працює.
        // Двигун заводимо напряму (Завдання 1), щоб тести Car не залежали від Завдання 4.
        private static Car ReadyCar()
        {
            var car = new Car("Test");
            car.CarDriver = new Driver("Олег", 5);
            car.CarEngine.Start();
            return car;
        }

        // =====================================================================
        // Завдання 1. Engine: Start() / Stop()
        // =====================================================================

        [Fact]
        public void Task1_Engine_IsNotRunningByDefault()
        {
            var engine = new Engine(150, "Бензин");

            Assert.False(engine.IsRunning, "Щойно створений двигун не працює");
        }

        [Fact]
        public void Task1_Engine_Start_SetsRunning()
        {
            var engine = new Engine(150, "Бензин");

            engine.Start();

            Assert.True(engine.IsRunning, "Після Start() властивість IsRunning має бути true");
        }

        [Fact]
        public void Task1_Engine_Stop_ClearsRunning()
        {
            var engine = new Engine(150, "Бензин");
            engine.Start();

            engine.Stop();

            Assert.False(engine.IsRunning, "Після Stop() властивість IsRunning має бути false");
        }

        [Fact]
        public void Task1_Engine_StartTwice_StaysRunning()
        {
            var engine = new Engine(150, "Бензин");

            engine.Start();
            engine.Start();

            Assert.True(engine.IsRunning, "Повторний Start() не вимикає двигун (це не перемикач)");
        }

        [Fact]
        public void Task1_Engine_StopWhenStopped_StaysStopped()
        {
            var engine = new Engine(150, "Бензин");

            engine.Stop();

            Assert.False(engine.IsRunning, "Stop() для зупиненого двигуна лишає його зупиненим (це не перемикач)");
        }

        // =====================================================================
        // Завдання 2. Wheel: Inflate()
        // =====================================================================

        [Fact]
        public void Task2_Wheel_Inflate_IncreasesPressure()
        {
            var wheel = new Wheel(16);
            double before = wheel.Pressure;

            wheel.Inflate(0.5);

            Assert.True(Near(before + 0.5, wheel.Pressure),
                $"Після Inflate(0.5) тиск має зрости на 0.5 (було {before}), а зараз {wheel.Pressure}");
        }

        [Fact]
        public void Task2_Wheel_Inflate_DoesNotExceedMaxPressure()
        {
            var wheel = new Wheel(16);

            wheel.Inflate(5);

            Assert.True(Near(Wheel.MaxPressure, wheel.Pressure),
                $"Тиск не може перевищувати MaxPressure ({Wheel.MaxPressure}), а зараз {wheel.Pressure}");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Task2_Wheel_Inflate_IgnoresNonPositive(double bar)
        {
            var wheel = new Wheel(16);
            double before = wheel.Pressure;

            wheel.Inflate(bar);

            Assert.True(Near(before, wheel.Pressure), $"Inflate({bar}) не змінює тиск (було {before}), а зараз {wheel.Pressure}");
        }

        [Fact]
        public void Task2_Wheel_Inflate_RaisesFlatWheelFromZero()
        {
            var wheel = new Wheel(16);
            wheel.Puncture();
            Assert.True(wheel.IsFlat, "Після проколу шина спущена");

            wheel.Inflate(1.0);
            Assert.True(Near(1.0, wheel.Pressure) && wheel.IsFlat, "Після Inflate(1.0) тиск 1.0: це ще замало (мінімум 1.5), шина лишається спущеною");

            wheel.Inflate(1.0);
            Assert.True(Near(2.0, wheel.Pressure) && !wheel.IsFlat, "Після ще одного Inflate(1.0) тиск 2.0: шина вже не спущена");
        }

        // =====================================================================
        // Захист стану: стартову структуру змінювати не можна
        // =====================================================================

        [Fact]
        public void Structure_InnerObjectsAreReadOnly_AndStateIsPrivate()
        {
            bool NoPublicSetter(Type t, string name)
            {
                MethodInfo? setter = t.GetProperty(name)!.SetMethod;
                return setter == null || !setter.IsPublic;
            }

            Assert.True(NoPublicSetter(typeof(Car), "CarEngine") && NoPublicSetter(typeof(Car), "Wheels"),
                "Не змінюйте стартову структуру Car: CarEngine і Wheels мають бути лише для читання (композиція)");
            Assert.True(NoPublicSetter(typeof(Engine), "IsRunning") && NoPublicSetter(typeof(Wheel), "Pressure") && NoPublicSetter(typeof(Car), "Mileage"),
                "Не змінюйте стартову структуру: IsRunning, Pressure і Mileage мають мати private set");
        }

        // =====================================================================
        // Завдання 3. Car: делегування (IsEngineRunning, StopEngine)
        // =====================================================================

        [Fact]
        public void Task3_Car_IsEngineRunning_ReflectsEngineState()
        {
            var car = new Car("A");
            Assert.False(car.IsEngineRunning, "Новe авто стоїть із вимкненим двигуном");

            car.CarEngine.Start();
            Assert.True(car.IsEngineRunning, "IsEngineRunning має повертати стан двигуна авто (CarEngine.IsRunning)");

            car.CarEngine.Stop();
            Assert.False(car.IsEngineRunning, "Після зупинки двигуна IsEngineRunning має знову бути false");
        }

        [Fact]
        public void Task3_Car_StopEngine_StopsTheEngine()
        {
            var car = new Car("A");
            car.CarEngine.Start();

            car.StopEngine();

            Assert.False(car.CarEngine.IsRunning, "StopEngine() має зупинити двигун авто");
        }

        // =====================================================================
        // Завдання 4. Car.TryStartEngine(): без водія двигун не заведеш
        // =====================================================================

        [Fact]
        public void Task4_TryStartEngine_NoDriver_ReturnsFalse_AndEngineStaysOff()
        {
            var car = new Car("A");

            bool started = car.TryStartEngine();

            Assert.False(started, "Без водія TryStartEngine() має повернути false (і не кидати винятків)");
            Assert.False(car.CarEngine.IsRunning, "Без водія двигун не має запускатися");
        }

        [Fact]
        public void Task4_TryStartEngine_WithDriver_StartsEngine_AndReturnsTrue()
        {
            var car = new Car("A");
            car.CarDriver = new Driver("Олег", 5);

            bool started = car.TryStartEngine();

            Assert.True(started, "З водієм TryStartEngine() має повернути true");
            Assert.True(car.CarEngine.IsRunning, "З водієм двигун має запуститися");
        }

        [Fact]
        public void Task4_TryStartEngine_WithDriver_WhenAlreadyRunning_ReturnsTrue()
        {
            var car = new Car("A");
            car.CarDriver = new Driver("Олег", 5);
            car.CarEngine.Start();

            bool started = car.TryStartEngine();

            Assert.True(started, "Водій є, двигун уже працює: TryStartEngine() має повернути true");
            Assert.True(car.CarEngine.IsRunning, "Двигун має лишитися увімкненим");
        }

        // =====================================================================
        // Завдання 5. Car: робота з усіма колесами (CountFlatWheels, PumpAllWheels)
        // =====================================================================

        [Fact]
        public void Task5_CountFlatWheels_FreshCar_IsZero()
        {
            var car = new Car("A");

            Assert.True(car.CountFlatWheels() == 0, $"У нового авто немає спущених шин, а повернуто {car.CountFlatWheels()}");
        }

        [Fact]
        public void Task5_CountFlatWheels_CountsEachPuncturedWheel()
        {
            var car = new Car("A");
            car.Wheels[1].Puncture();
            car.Wheels[3].Puncture();

            Assert.True(car.CountFlatWheels() == 2, $"Проколоті колеса 1 і 3: має бути 2 спущені шини, а повернуто {car.CountFlatWheels()}");

            car.Wheels[0].Puncture();
            car.Wheels[2].Puncture();

            Assert.True(car.CountFlatWheels() == 4, $"Усі 4 шини спущені: має бути 4, а повернуто {car.CountFlatWheels()}");
        }

        [Fact]
        public void Task5_CountFlatWheels_UsesWheelIsFlat()
        {
            var car = new Car("A");
            car.Wheels[0].Puncture();
            car.Wheels[0].Inflate(1.8);   // 1.8 бар: вище мінімуму 1.5, шина не спущена
            car.Wheels[1].Puncture();
            car.Wheels[1].Inflate(1.0);   // 1.0 бар: нижче мінімуму, шина спущена

            Assert.True(car.CountFlatWheels() == 1,
                $"Спущеною вважається шина з Wheel.IsFlat (тиск < {Wheel.MinPressure}): очікується 1, а повернуто {car.CountFlatWheels()}");
        }

        [Fact]
        public void Task5_PumpAllWheels_InflatesEveryWheel()
        {
            var car = new Car("A");

            car.PumpAllWheels(0.3);

            for (int i = 0; i < car.Wheels.Length; i++)
            {
                Assert.True(Near(2.5, car.Wheels[i].Pressure),
                    $"Wheels[{i}].Pressure має бути 2.5 після PumpAllWheels(0.3), а зараз {car.Wheels[i].Pressure}: треба підкачати КОЖНУ шину");
            }
        }

        [Fact]
        public void Task5_PumpAllWheels_FixesPuncturedWheels_AndRespectsMax()
        {
            var car = new Car("A");
            car.Wheels[0].Puncture();
            car.Wheels[2].Puncture();

            car.PumpAllWheels(2.0);

            Assert.True(Near(2.0, car.Wheels[0].Pressure) && Near(2.0, car.Wheels[2].Pressure), "Проколоті шини після PumpAllWheels(2.0) мають мати тиск 2.0");
            Assert.True(Near(Wheel.MaxPressure, car.Wheels[1].Pressure) && Near(Wheel.MaxPressure, car.Wheels[3].Pressure),
                "Звичайні шини (2.2 + 2.0) не можуть перевищити MaxPressure");
            Assert.True(car.CountFlatWheels() == 0, "Після підкачування спущених шин немає");
        }

        // =====================================================================
        // Завдання 6. Car.ReplaceWheel()
        // =====================================================================

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Task6_ReplaceWheel_PutsNewWheel_AndReturnsOld(int index)
        {
            var car = new Car("A");
            Wheel[] before = (Wheel[])car.Wheels.Clone();
            var fresh = new Wheel(17);

            Wheel? removed = car.ReplaceWheel(index, fresh);

            Assert.True(ReferenceEquals(removed, before[index]), "ReplaceWheel має повернути зняте (старе) колесо");
            Assert.True(ReferenceEquals(car.Wheels[index], fresh), "На позиції index має стояти нове колесо");
            Assert.True(car.Wheels.Length == 4, "Кількість коліс лишається 4");
            for (int i = 0; i < 4; i++)
            {
                if (i != index)
                {
                    Assert.True(ReferenceEquals(car.Wheels[i], before[i]), $"Колесо {i} не має змінитися");
                }
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        [InlineData(10)]
        public void Task6_ReplaceWheel_InvalidIndex_ReturnsNull_AndChangesNothing(int index)
        {
            var car = new Car("A");
            Wheel[] before = (Wheel[])car.Wheels.Clone();

            Wheel? removed = car.ReplaceWheel(index, new Wheel(17));

            Assert.True(removed == null, $"Для index = {index} (поза межами масиву) метод має повернути null і не кидати винятків");
            for (int i = 0; i < 4; i++)
            {
                Assert.True(ReferenceEquals(car.Wheels[i], before[i]), "Колеса не мають змінитися");
            }
        }

        [Fact]
        public void Task6_ReplaceWheel_NullWheel_ReturnsNull_AndChangesNothing()
        {
            var car = new Car("A");
            Wheel original = car.Wheels[2];

            Wheel? removed = car.ReplaceWheel(2, null!);

            Assert.True(removed == null, "Якщо нове колесо дорівнює null, метод повертає null");
            Assert.True(ReferenceEquals(car.Wheels[2], original), "Колесо не має бути прибране (не можна поставити null замість колеса)");
        }

        // =====================================================================
        // Завдання 7. Car.IsReadyToDrive() та Car.Drive()
        // =====================================================================

        [Fact]
        public void Task7_IsReadyToDrive_DriverEngineAndGoodWheels_IsTrue()
        {
            var car = ReadyCar();

            Assert.True(car.IsReadyToDrive(), "Є водій, двигун працює, шини в нормі: авто готове до поїздки");
        }

        [Fact]
        public void Task7_IsReadyToDrive_NoDriver_IsFalse()
        {
            var car = ReadyCar();
            car.RemoveDriver();

            Assert.False(car.IsReadyToDrive(), "Без водія авто не готове до поїздки (і метод не кидає винятків)");
        }

        [Fact]
        public void Task7_IsReadyToDrive_EngineOff_IsFalse()
        {
            var car = ReadyCar();
            car.CarEngine.Stop();

            Assert.False(car.IsReadyToDrive(), "Із вимкненим двигуном авто не готове до поїздки");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Task7_IsReadyToDrive_AnyFlatWheel_IsFalse(int index)
        {
            var car = ReadyCar();
            car.Wheels[index].Puncture();

            Assert.False(car.IsReadyToDrive(), $"Якщо шина Wheels[{index}] спущена, авто не готове до поїздки");
        }

        [Fact]
        public void Task7_Drive_ReadyCar_ReturnsKm_AndIncreasesMileage()
        {
            var car = ReadyCar();

            double driven = car.Drive(100);

            Assert.True(Near(100, driven), $"Drive(100) має повернути 100, а повернув {driven}");
            Assert.True(Near(100, car.Mileage), $"Одометр має показувати 100, а зараз {car.Mileage}");
        }

        [Fact]
        public void Task7_Drive_TwoTrips_AccumulateMileage()
        {
            var car = ReadyCar();

            double first = car.Drive(100);
            double second = car.Drive(50);

            Assert.True(Near(100, first) && Near(50, second), $"Кожна поїздка повертає свою відстань (100 і 50), а повернено {first} і {second}");
            Assert.True(Near(150, car.Mileage), $"Одометр накопичує пробіг: 100 + 50 = 150, а зараз {car.Mileage}");
        }

        [Fact]
        public void Task7_Drive_NoDriver_ReturnsZero_AndMileageUnchanged()
        {
            var car = ReadyCar();
            car.RemoveDriver();

            double driven = car.Drive(100);

            Assert.True(Near(0, driven) && Near(0, car.Mileage), "Без водія авто не їде: Drive повертає 0, одометр не змінюється");
        }

        [Fact]
        public void Task7_Drive_EngineOff_ReturnsZero_AndMileageUnchanged()
        {
            var car = ReadyCar();
            car.CarEngine.Stop();

            double driven = car.Drive(100);

            Assert.True(Near(0, driven) && Near(0, car.Mileage), "Із вимкненим двигуном авто не їде: Drive повертає 0, одометр не змінюється");
        }

        [Fact]
        public void Task7_Drive_FlatWheel_ReturnsZero_AndMileageUnchanged()
        {
            var car = ReadyCar();
            car.Wheels[1].Puncture();

            double driven = car.Drive(100);

            Assert.True(Near(0, driven) && Near(0, car.Mileage), "Зі спущеною шиною авто не їде: Drive повертає 0, одометр не змінюється");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Task7_Drive_NonPositiveKm_ReturnsZero_AndMileageUnchanged(double km)
        {
            var car = ReadyCar();

            double driven = car.Drive(km);

            Assert.True(Near(0, driven) && Near(0, car.Mileage), $"Drive({km}) повертає 0 і не змінює одометр (пробіг не можна «скрутити» назад)");
        }

        [Fact]
        public void Task7_Scenario_Puncture_Repair_ContinueTrip()
        {
            var car = ReadyCar();
            Assert.True(Near(100, car.Drive(100)), "Перша поїздка 100 км має вдатися");

            car.Wheels[2].Puncture();
            Assert.True(Near(0, car.Drive(10)) && Near(100, car.Mileage), "Після проколу авто не їде, одометр лишається 100");

            Wheel? removed = car.ReplaceWheel(2, new Wheel(16));
            Assert.True(removed != null && removed.IsFlat, "Зняте колесо має бути тим самим, проколотим");
            Assert.True(car.IsReadyToDrive(), "Після заміни колеса авто знову готове до поїздки");

            Assert.True(Near(50, car.Drive(50)) && Near(150, car.Mileage), "Після ремонту поїздка вдається, одометр 150");
        }

        // =====================================================================
        // Завдання 8. Main
        // =====================================================================

        [Fact]
        public void Task8_Main_RunsWithoutExceptions_AndPrintsReport()
        {
            string text = RunMain();

            int lines = text
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Length;

            Assert.True(lines >= 11,
                $"Main має вивести звіт за Кроками 1-6; зараз виведено лише {lines} непорожніх рядків");
        }

        [Fact]
        public void Task8_Main_CatchesNullReferenceException_AndPrintsSafeVersion()
        {
            string text = RunMain();

            Assert.True(text.Contains("NullReferenceException"),
                "Крок 6б: після try / catch (NullReferenceException) виведіть ex.GetType().Name");
            Assert.True(text.Contains("Водія немає", StringComparison.OrdinalIgnoreCase),
                "Крок 6в: безпечна версія з ?. та ?? має вивести \"Водія немає\"");
        }

        private static string RunMain()
        {
            var originalOut = Console.Out;
            var originalIn = Console.In;
            var output = new StringWriter();

            try
            {
                Console.SetOut(output);
                Console.SetIn(new StringReader(string.Empty));

                Program.Main(Array.Empty<string>());
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetIn(originalIn);
            }

            return output.ToString();
        }
    }
}
