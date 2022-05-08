using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PersonalManagement
{
    class Program
    {
        private static Timer _timer = null;
        private static void TimerCallback(Object o)
        {
            foreach (Meeting meeting in Meeting.meetings)
            {
                DateTime dt = meeting.GetNotification();
                dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Kind);
                DateTime dtnow = DateTime.Now;
                dtnow = new DateTime(dtnow.Year, dtnow.Month, dtnow.Day, dtnow.Hour, dtnow.Minute, dtnow.Second, dtnow.Kind);
                if (dt == dtnow)
                {
                    Console.WriteLine("\nНапоминание: встреча начнется в " + meeting.GetDateTimeStart());
                }
            }
        }

        enum Act
        {
            add,
            change
        }

        static void Menu()
        {
            Console.Clear();
            Console.WriteLine("1. Добавить встречу");
            Console.WriteLine("2. Изменить параметры встречи");
            Console.WriteLine("3. Отменить встречу");
            Console.WriteLine("4. Посмотреть расписание встреч за день");
            Console.WriteLine("5. Выход");
            Console.Write("\nВведите номер пункта и нажмите клавишу enter: ");
        }

        static void AddOrChangeMeeting(Act act, DateTime dt = new DateTime())
        {
            Console.Clear();
            if (act == Act.change)
            {
                Console.WriteLine("Введите новые данные для встречи:");
            }
            DateTime dateTimeStart, dateTimeEnd, notification;
            string description;
            dateTimeStart = InputDateTime("Введите дату и время начала встречи в формате dd.MM.yyyy hh:mm:ss: ");
            if (!CheckDateTime(dateTimeStart))
            {
                return;
            }
            dateTimeEnd = InputDateTime("Введите дату и время окончания встречи в формате dd.MM.yyyy hh:mm:ss: ");
            if (!CheckDateTime(dateTimeEnd))
            {
                return;
            }
            Console.Write("Введите описание встречи: ");
            description = Console.ReadLine();
            notification = InputDateTime("Введите дату и время оповещения о встрече в формате dd.MM.yyyy hh:mm:ss: ");
            if (!CheckDateTime(notification))
            {
                return;
            }
            if (act == Act.change)
            {
                Meeting desiredMeeting = SearchMeeting(dt);
                if (desiredMeeting == null)
                {
                    Console.Clear();
                    Console.Write("Встреча не найдена. Для продолжения нажмите любую клавишу");
                    Console.ReadKey();
                    return;
                }
                if (desiredMeeting.Check(dateTimeStart, dateTimeEnd, notification))
                {
                    desiredMeeting.SetDateTimeStart(dateTimeStart);
                    desiredMeeting.SetDateTimeEnd(dateTimeEnd);
                    desiredMeeting.SetDescription(description);
                    desiredMeeting.SetNotification(notification);
                    Console.Clear();
                    Console.Write("Встреча успешно изменена. Для продолжения нажмите любую клавишу");
                }
                else
                {
                    Console.Clear();
                    Console.Write("Ошибка при вводе данных. Данные встречи не изменены. Для продолжения нажмите любую клавишу");
                    Console.ReadKey();
                    return;
                }
            }
            else if (act == Act.add)
            {
                Meeting meeting = new Meeting(dateTimeStart, dateTimeEnd, description, notification);
                if (!meeting.Check(meeting.GetDateTimeStart(), meeting.GetDateTimeEnd(), meeting.GetNotification()))
                {
                    Console.Clear();
                    Console.Write("Ошибка при вводе данных. Встреча не добавлена. Для продолжения нажмите любую клавишу");
                    Console.ReadKey();
                    return;
                }
                Console.Clear();
                Console.Write("Встреча успешно добавлена. Для продолжения нажмите любую клавишу");
            }
            Console.ReadKey();
        }

        static void ExportToFile(string result, DateTime dt)
        {
            result = result.Insert(0, "Встречи на " + dt.Day.ToString() + "." + dt.Month.ToString() + "." + dt.Year.ToString() + "\n");
            try
            {
                using (FileStream fs = new FileStream("report.txt", FileMode.Create))
                {
                    byte[] byteArray = Encoding.Default.GetBytes(result);
                    fs.Write(byteArray, 0, byteArray.Length);
                    fs.Position = 0;
                    Console.WriteLine();
                }
                Console.Write("Данные экспортированы в файл. Для продолжения нажмите любую клавишу");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.Write("Для продолжения нажмите любую клавишу");
                Console.ReadKey();
            }
        }

        static DateTime InputDateTime(string message)
        {
            Console.Write(message);
            DateTime dt = new DateTime();
            try
            {
                dt = Convert.ToDateTime(Console.ReadLine());
            }
            catch
            {
                Console.Write("Ошибка ввода. Для продолжения нажмите любую клавишу");
                Console.ReadKey();
            }
            return dt;
        }

        static bool CheckDateTime(DateTime dt)
        {
            DateTime dateTime = new DateTime();
            return dateTime != dt;
        }

        static Meeting SearchMeeting(DateTime dt)
        {
            foreach (Meeting meeting in Meeting.meetings)
            {
                if (dt == meeting.GetDateTimeStart())
                {
                    return meeting;
                }
            }
            return null;
        }

        static void Main(string[] args)
        {
            _timer = new Timer(TimerCallback, null, 0, 1000);
            bool fl = true;
            DateTime dt;
            while (fl)
            {
                Menu();
                int action;
                try
                {
                    action = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.Write("Ошибка ввода. Для продолжения нажмите любую клавишу");
                    Console.ReadKey();
                    continue;
                }
                switch (action)
                {
                    case 1:
                        AddOrChangeMeeting(Act.add);
                        break;
                    case 2:
                        Console.Clear();
                        dt = InputDateTime("Введите дату начала изменяемого события в формате dd.MM.yyyy hh:mm:ss: ");
                        if (!CheckDateTime(dt))
                        {
                            continue;
                        }
                        AddOrChangeMeeting(Act.change, dt);
                        break;
                    case 3:
                        Console.Clear();
                        dt = InputDateTime("Введите дату начала удаляемого события в формате dd.MM.yyyy hh:mm:ss: ");
                        if (!CheckDateTime(dt))
                        {
                            continue;
                        }
                        Meeting meet = SearchMeeting(dt);
                        if (meet != null)
                        {
                            meet.DeleteMeeting();
                            Console.Clear();
                            Console.WriteLine("Встреча успешно отменена. Для продолжения нажмите любую клавишу");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Встреча не найдена. Для продолжения нажмите любую клавишу");
                            Console.ReadKey();
                        }
                        break;
                    case 4:
                        Console.Clear();
                        dt = InputDateTime("Введите дату в формате dd.MM.yyy: ");
                        if (!CheckDateTime(dt))
                        {
                            continue;
                        }
                        string result = Meeting.GetMeetings(dt);
                        Console.WriteLine(result);
                        Console.Write("Хотите экспортировать данные в файл? (yes - экспортировать, иной ответ - нет): ");
                        string answer = Console.ReadLine();
                        if (answer == "yes")
                        {
                            ExportToFile(result, dt);
                        }
                        break;
                    case 5:
                        fl = false;
                        break;
                    default:
                        Console.WriteLine("Ошибка ввода. Для продолжения нажмите любую клавишу");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}