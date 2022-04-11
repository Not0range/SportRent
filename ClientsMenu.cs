using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportRent
{
    static partial class Program
    {
        static List<string> clientsTable = new List<string>();
        static State ClientsMenu()
        {
            int prevBuffSize = Console.BufferWidth;
            var state = State.Cancel;

            do
            {
                if (state == State.Decide)
                    GenerateClientsTable();

                clientsTable.ForEach(t => Console.WriteLine(t));

                Console.WriteLine();
                if (state == State.Error)
                    Console.WriteLine("Ошибка ввода");

                state = MenuOutput(false, (() => AddEditClient(), "Добавить клиента"),
                                         (EditClient, "Редактировать клиента"),
                                         (DeleteClient, "Удалить клиента"));
                Console.Clear();
            } while (state != State.Quit);
            Console.BufferWidth = prevBuffSize;
            return State.Decide;
        }

        private static void GenerateClientsTable()
        {
            string[] columns = new string[] { "ID", "Фамилия", "Имя", "Отчество", "Адрес", "Контактные данные", "Скидка" };
            int[] widths = new int[columns.Length];
            clientsTable.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            foreach (var c in Lists.clients)
            {
                if (c.Id.ToString().Length > widths[0])
                    widths[0] = c.Id.ToString().Length;
                if (c.lastName.Length > widths[1])
                    widths[1] = c.lastName.Length;
                if (c.firstName.Length > widths[2])
                    widths[2] = c.firstName.Length;
                if (c.midName.Length > widths[3])
                    widths[3] = c.midName.Length;
                if (c.address.Length > widths[4])
                    widths[4] = c.address.Length;
                if (c.contacts.Length > widths[5])
                    widths[5] = c.contacts.Length;
                if (c.discount.ToString().Length > widths[6])
                    widths[6] = c.discount.ToString().Length;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            int w = widths.Sum() + Environment.NewLine.Length;
            if (w > Console.BufferWidth)
                Console.BufferWidth = widths.Sum() + Environment.NewLine.Length;

            var temp = new (string, int)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            TableOutput(clientsTable, temp);

            foreach (var c in Lists.clients)
            {
                temp[0] = (c.Id.ToString(), widths[0]);
                temp[1] = (c.lastName, widths[1]);
                temp[2] = (c.firstName, widths[2]);
                temp[3] = (c.midName, widths[3]);
                temp[4] = (c.address, widths[4]);
                temp[5] = (c.contacts, widths[5]);
                temp[6] = (c.discount.ToString(), widths[6]);
                TableOutput(clientsTable, temp);
            }
        }

        static State AddEditClient(Client c = null)
        {
            bool success;
            string lastName, firstName, midName, address, contacts, discountStr;
            decimal discount = 0;

            lastName = ReadLine(String.Format("Введите фамилию клиента{0}: ",
                c != null ? " (" + c.lastName + ")" : ""), c != null);
            firstName = ReadLine(String.Format("Введите имя клиента{0}: ",
                c != null ? " (" + c.firstName + ")" : ""), c != null);
            midName = ReadLine(String.Format("Введите отчество клиента{0}: ",
                c != null ? " (" + c.midName + ")" : ""), c != null);
            address = ReadLine(String.Format("Введите адрес клиента{0}: ",
                c != null ? " (" + c.address + ")" : ""), c != null);
            contacts = ReadLine(String.Format("Введите контактные данные клиента{0}: ",
                c != null ? " (" + c.contacts + ")" : ""), c != null);
            do
            {
                discountStr = ReadLine(String.Format("Введите скидку клиента{0}: ",
                    c != null ? " (" + c.discount + ")" : ""), c != null);
                if (string.IsNullOrWhiteSpace(discountStr))
                    break;

                success = decimal.TryParse(discountStr, out discount);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Фамилия: {0}", string.IsNullOrWhiteSpace(lastName) ? c.lastName : lastName);
            Console.WriteLine("Имя: {0}", string.IsNullOrWhiteSpace(firstName) ? c.firstName : firstName);
            Console.WriteLine("Отчество: {0}", string.IsNullOrWhiteSpace(midName) ? c.midName : midName);
            Console.WriteLine("Адрес: {0}", string.IsNullOrWhiteSpace(address) ? c.address : address);
            Console.WriteLine("Контактные данные: {0}", string.IsNullOrWhiteSpace(contacts) ? c.contacts : contacts);
            Console.WriteLine("Скидка: {0}", string.IsNullOrWhiteSpace(discountStr) ? c.discount : discount);
            Console.WriteLine("{0} данного клиента?", c == null ? "Добавить" : "Сохранить");
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

            if (c == null)
            {
                Lists.clients.Add(new Client
                {
                    Id = Lists.MinimalId(Lists.clients),
                    lastName = lastName,
                    firstName = firstName,
                    midName = midName,
                    address = address,
                    contacts = contacts,
                    discount = discount
                });
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(lastName)) c.lastName = lastName;
                if (!string.IsNullOrWhiteSpace(firstName)) c.firstName = lastName;
                if (!string.IsNullOrWhiteSpace(midName)) c.midName = midName;
                if (!string.IsNullOrWhiteSpace(address)) c.address = address;
                if (!string.IsNullOrWhiteSpace(contacts)) c.contacts = contacts;
                if (!string.IsNullOrWhiteSpace(discountStr)) c.discount = discount;
            }
            return State.Decide;
        }

        static State EditClient()
        {
            clientsTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID клиента, который необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Client c;
            if (!success || (c = Lists.clients.FirstOrDefault(t => t.Id == id)) == null)
                return State.Error;

            Console.Clear();
            var s = AddEditClient(c);
            GenerateRentsTable();
            return s;
        }

        static State DeleteClient()
        {
            clientsTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID клиента, который необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Client c;
            if (!success || (c = Lists.clients.FirstOrDefault(t => t.Id == id)) == null)
                return State.Error;

            Lists.clients.Remove(c);
            Lists.rents.RemoveAll(t => t.client == c);
            GenerateRentsTable();
            return State.Decide;
        }
    }
}
