using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportRent
{
    static partial class Program
    {
        static List<string> rentsTable = new List<string>();
        static State RentsMenu()
        {
            int prevBuffSize = Console.BufferWidth;
            var state = State.Cancel;

            do
            {
                if (state == State.Decide)
                    GenerateRentsTable();

                rentsTable.ForEach(t => Console.WriteLine(t));

                Console.WriteLine();
                if (state == State.Error)
                    Console.WriteLine("Ошибка ввода");

                state = MenuOutput(false, (() => AddEditRent(), "Добавить прокат"),
                                         (EditRent, "Редактировать прокат"),
                                         (DeleteRent, "Удалить прокат"));
                Console.Clear();
            } while (state != State.Quit);
            Console.BufferWidth = prevBuffSize;
            return State.Decide;
        }

        private static void GenerateRentsTable()
        {
            string[] columns = new string[] { "ID", "Клиент", "Товар", "Количество", "Дата/время начала", 
                "Дата/время окончания", "Стоимость" };
            int[] widths = new int[columns.Length];

            rentsTable.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            foreach (var r in Lists.rents)
            {
                int max;
                if (r.Id.ToString().Length > widths[0])
                    widths[0] = r.Id.ToString().Length;
                if (r.client.ToString().Length > widths[1])
                    widths[1] = r.client.ToString().Length;
                if (r.product.ToString().Length > widths[2])
                    widths[2] = r.product.ToString().Length;
                if (r.count.ToString().Length > widths[3])
                    widths[3] = r.count.ToString().Length;
                max = r.beginDate.ToString("dd.MM.yy HH:mm").Length;
                if (max > widths[4])
                    widths[4] = max;
                max = r.endDate.ToString("dd.MM.yy HH:mm").Length;
                if (max > widths[5])
                    widths[5] = max;
                if (r.Price.ToString().Length > widths[6])
                    widths[6] = r.Price.ToString().Length;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            int w = widths.Sum() + Environment.NewLine.Length;
            if (w > Console.BufferWidth)
                Console.BufferWidth = widths.Sum() + Environment.NewLine.Length;

            var temp = new (string, int)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            TableOutput(rentsTable, temp);

            foreach (var r in Lists.rents)
            {
                temp[0] = (r.Id.ToString(), widths[0]);
                temp[1] = (r.client.ToString(), widths[1]);
                temp[2] = (r.product.ToString(), widths[2]);
                temp[3] = (r.count.ToString(), widths[3]);
                temp[4] = (r.beginDate.ToString("dd.MM.yy HH:mm"), widths[4]);
                temp[5] = (r.endDate.ToString("dd.MM.yy HH:mm"), widths[5]);
                temp[6] = (r.Price.ToString(), widths[4]);
                TableOutput(rentsTable, temp);
            }
        }

        static State AddEditRent(Rent r = null)
        {
            string clientStr, productStr, countStr, beginDateStr, endDateStr;
            int client = 0;
            int product = 0;
            int count = 0;
            DateTime beginDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            bool success;

            clientsTable.ForEach(t => Console.WriteLine(t));
            do
            {
                clientStr = ReadLine(String.Format("Введите ID клиента{0}: ",
                    r != null ? " (" + r.client.Id + ")" : ""), r != null);
                if (string.IsNullOrWhiteSpace(clientStr))
                    break;

                success = int.TryParse(clientStr, out client) && Lists.clients.Any(t => t.Id == client);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            productsTable.ForEach(t => Console.WriteLine(t));
            do
            {
                productStr = ReadLine(String.Format("Введите ID товара{0}: ",
                    r != null ? " (" + r.product.Id + ")" : ""), r != null);
                if (string.IsNullOrWhiteSpace(productStr))
                    break;

                success = int.TryParse(productStr, out product) && Lists.products.Any(t => t.Id == product);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            do
            {
                countStr = ReadLine(String.Format("Введите количество товара{0}: ",
                    r != null ? " (" + r.count + ")" : ""), r != null);
                if (string.IsNullOrWhiteSpace(countStr))
                    break;

                success = int.TryParse(countStr, out count) && count > 0;
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                beginDateStr = ReadLine(String.Format("Введите дату и время начала проката{0}: ",
                    r != null ? " (" + r.beginDate.ToString("dd.MM.yy HH:mm") + ")" : ""), r != null);
                if (string.IsNullOrWhiteSpace(beginDateStr))
                    break;

                success = DateTime.TryParse(beginDateStr, out beginDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                endDateStr = ReadLine(String.Format("Введите дату и время окончания проката{0}: ",
                    r != null ? " (" + r.endDate.ToString("dd.MM.yy HH:mm") + ")" : ""), r != null);
                if (string.IsNullOrWhiteSpace(endDateStr))
                    break;

                success = DateTime.TryParse(endDateStr, out endDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Клиент: {0}", string.IsNullOrWhiteSpace(clientStr) ? 
                r.client : Lists.clients.First(t => t.Id == client));
            Console.WriteLine("Товар: {0}", string.IsNullOrWhiteSpace(productStr) ? 
                r.product : Lists.products.First(t => t.Id == product));
            Console.WriteLine("Количество: {0}", count);
            Console.WriteLine("Дата и время начала проката: {0}", string.IsNullOrWhiteSpace(beginDateStr) ? 
                r.beginDate.ToString("dd.MM.yy HH:mm") : beginDate.ToString("dd.MM.yy HH:mm"));
            Console.WriteLine("Дата и время окончания проката: {0}", string.IsNullOrWhiteSpace(endDateStr) ?
                r.endDate.ToString("dd.MM.yy HH:mm") : endDate.ToString("dd.MM.yy HH:mm"));
            Console.WriteLine("{0} данный прокат?", r == null ? "Добавить" : "Сохранить");
            Console.WriteLine("1 - Да");
            Console.WriteLine("2 - Нет");
            do
            {
                var k = Console.ReadKey(true);
                if (k.KeyChar == '1')
                    break;
                else if (k.KeyChar == '2')
                    return State.Cancel;

            } while (true);

            if (r == null)
            {
                Lists.rents.Add(new Rent
                {
                    Id = Lists.MinimalId(Lists.rents),
                    client = Lists.clients.First(t => t.Id == client),
                    product = Lists.products.First(t => t.Id == product),
                    count = count,
                    beginDate = beginDate,
                    endDate = endDate
                });
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(clientStr)) r.client = Lists.clients.First(t => t.Id == client);
                if (!string.IsNullOrWhiteSpace(productStr)) r.product = Lists.products.First(t => t.Id == product);
                if (!string.IsNullOrWhiteSpace(countStr)) r.count = count;
                if (!string.IsNullOrWhiteSpace(beginDateStr)) r.beginDate = beginDate;
                if (!string.IsNullOrWhiteSpace(endDateStr)) r.endDate = endDate;
            }
            return State.Decide;
        }

        static State EditRent()
        {
            rentsTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID проката, который необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Rent d;
            if (!success || (d = Lists.rents.FirstOrDefault(t => t.Id == id)) == null)
                return State.Error;

            Console.Clear();
            return AddEditRent(d);
        }

        static State DeleteRent()
        {
            rentsTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID проката, который необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Rent d;
            if (!success || (d = Lists.rents.FirstOrDefault(t => t.Id == id)) == null)
                return State.Error;

            Lists.rents.Remove(d);
            return State.Decide;
        }
    }
}
