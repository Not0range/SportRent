using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportRent
{
    static partial class Program
    {
        static void Main(string[] args)
        {
            bool error = false;
            ReadProducts(ref error);
            ReadClients(ref error);
            ReadRents(ref error);

            if (error)
            {
                Console.WriteLine("При чтении файла были обнаружены некорректные записи.");
                Console.WriteLine("Они не были добавлены в таблицы и будут удалены");
                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить");
                Console.ReadKey();
            }
            GenerateProductsTable();
            GenerateClientsTable();
            GenerateRentsTable();

            Console.Title = "Прокат спортивного оборудования";

            var state = State.Decide;

            do
            {
                if (state == State.Decide)
                {
                    Console.Write(new string('~', Console.BufferWidth));
                    Console.WriteLine("Добро пожаловать в систему управления прокатом спортоборудования");
                    Console.Write(new string('~', Console.BufferWidth));
                }

                if (state == State.Error)
                    Console.WriteLine("Ошибка ввода");
                state = MenuOutput(true, (ProductMenu, "Работа со списком товаров"),
                                (ClientsMenu, "Работа со списком клиентов"),
                                (RentsMenu, "Работа со списком прокатов"));
            } while (state != State.Quit);
            SaveAll();
        }

        static void ReadProducts(ref bool error)
        {
            int id;
            string[] data;
            var reader = new StreamReader(new FileStream("products.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite));

            while (!reader.EndOfStream)
            {
                try
                {
                    data = reader.ReadLine().Split('\t');
                    id = int.Parse(data[0]);
                    if (Lists.products.Any(t => t.Id == id))
                        throw new ArgumentException();

                    Lists.products.Add(new Product
                    {
                        Id = id,
                        name = data[1],
                        price = decimal.Parse(data[2]),
                        count = int.Parse(data[3]),
                        description = data[4]
                    });
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();
        }

        static void ReadClients(ref bool error)
        {
            int id;
            string[] data;
            var reader = new StreamReader(new FileStream("clients.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite));
            while (!reader.EndOfStream)
            {
                try
                {
                    data = reader.ReadLine().Split('\t');
                    id = int.Parse(data[0]);
                    if (Lists.clients.Any(t => t.Id == id))
                        throw new ArgumentException();

                    Lists.clients.Add(new Client
                    {
                        Id = id,
                        lastName = data[1],
                        firstName = data[2],
                        midName = data[3],
                        address = data[4],
                        contacts = data[5],
                        discount = decimal.Parse(data[6]),
                    });
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();
        }

        static void ReadRents(ref bool error)
        {
            int id;
            string[] data;
            var reader = new StreamReader(new FileStream("rents.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite));
            while (!reader.EndOfStream)
            {
                try
                {
                    data = reader.ReadLine().Split('\t');
                    id = int.Parse(data[0]);
                    if (Lists.rents.Any(t => t.Id == id))
                        throw new ArgumentException();

                    Lists.rents.Add(new Rent
                    {
                        Id = id,
                        client = Lists.clients.First(t => t.Id == int.Parse(data[1])),
                        product = Lists.products.First(t => t.Id == int.Parse(data[2])),
                        count = int.Parse(data[3]),
                        beginDate = DateTime.ParseExact(data[4], "dd.MM.yy HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                        endDate = DateTime.ParseExact(data[5], "dd.MM.yy HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                    });
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();
        }

        static void SaveAll()
        {
            StreamWriter writer;
            if (Lists.products.Count > 0)
            {
                writer = new StreamWriter("products.txt");
                foreach (var p in Lists.products)
                    writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", p.Id, p.name, p.price,
                        p.count, p.description);
                writer.Close();
            }

            if (Lists.clients.Count > 0)
            {
                writer = new StreamWriter("clients.txt");
                foreach (var c in Lists.clients)
                    writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", c.Id, c.lastName, c.firstName,
                        c.midName, c.address, c.contacts, c.discount);
                writer.Close();
            }

            if (Lists.rents.Count > 0)
            {
                writer = new StreamWriter("rents.txt");
                foreach (var c in Lists.rents)
                    writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", c.Id, c.client.Id, c.product.Id,
                        c.count, c.beginDate.ToString("dd.MM.yy HH:mm"), c.endDate.ToString("dd.MM.yy HH:mm"));
                writer.Close();
            }
        }

        static State MenuOutput(bool main, params (Func<State> action, string text)[] lines)
        {
            do
            {
                Console.WriteLine("Выберите необходимое действие");
                for (int i = 0; i < lines.Length && i < 9; i++)
                    Console.WriteLine("{0} - {1}", i + 1, lines[i].text);

                if (main)
                    Console.WriteLine("{0} - Выход из программы", lines.Length + 1 < 10 ? lines.Length + 1 : 0);
                else
                    Console.WriteLine("{0} - В предыдущее меню", lines.Length + 1 < 10 ? lines.Length + 1 : 0);

                var key = Console.ReadKey(true);
                Console.Clear();
                if (lines.Length >= 10 && key.KeyChar < '0' || lines.Length < 10 && key.KeyChar <= '0' || key.KeyChar > '9' ||
                    key.KeyChar.ToInt() > lines.Length + 1)
                    return State.Error;
                else if (lines.Length >= 10 && key.KeyChar == '0' || key.KeyChar.ToInt() == lines.Length + 1)
                    return State.Quit;
                else
                    return lines[key.KeyChar.ToInt() - 1].action.Invoke();
            } while (true);
        }

        static void TableOutput(List<string> list, params (string text, int width)[] fields)
        {
            list.Add(string.Join("", fields.Select(f => f.text.PadRight(f.width))));
        }

        static string ReadLine(string text, bool allowEmpty = false)
        {
            bool success;
            string str;
            do
            {
                Console.Write(text);
                str = Console.ReadLine().Trim();
                success = !string.IsNullOrWhiteSpace(str) || allowEmpty;
                if (!success)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new string('\0', Console.BufferWidth));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                }
            } while (!success);
            return str;
        }

        static int ToInt(this char c)
        {
            return int.Parse(c.ToString());
        }

        enum State
        {
            Decide,
            Cancel,
            Quit,
            Error
        }
    }
}
