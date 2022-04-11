using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportRent
{
    static partial class Program
    {
        static List<string> productsTable = new List<string>();
        static State ProductMenu()
        {
            int prevBuffSize = Console.BufferWidth;
            var state = State.Cancel;

            do
            {
                if (state == State.Decide)
                    GenerateProductsTable();

                productsTable.ForEach(t => Console.WriteLine(t));

                Console.WriteLine();
                if (state == State.Error)
                    Console.WriteLine("Ошибка ввода");

                state = MenuOutput(false, (() => AddEditProduct(), "Добавить товар"),
                                         (EditProduct, "Редактировать товар"),
                                         (DeleteProduct, "Удалить товар"));
                Console.Clear();
            } while (state != State.Quit);
            Console.BufferWidth = prevBuffSize;
            return State.Decide;
        }

        private static void GenerateProductsTable()
        {
            string[] columns = new string[] { "ID", "Название", "Цена/час", "Описание" };
            int[] widths = new int[columns.Length];
            productsTable.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            foreach (var p in Lists.products)
            {
                if (p.Id.ToString().Length > widths[0])
                    widths[0] = p.Id.ToString().Length;
                if (p.name.Length > widths[1])
                    widths[1] = p.name.Length;
                if (p.price.ToString().Length > widths[2])
                    widths[2] = p.price.ToString().Length;
                if (p.description.Length > widths[3])
                    widths[3] = p.description.Length;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            int w = widths.Sum() + Environment.NewLine.Length;
            if (w > Console.BufferWidth)
                Console.BufferWidth = widths.Sum() + Environment.NewLine.Length;

            var temp = new (string, int)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            TableOutput(productsTable, temp);

            foreach (var p in Lists.products)
            {
                temp[0] = (p.Id.ToString(), widths[0]);
                temp[1] = (p.name, widths[1]);
                temp[2] = (p.price.ToString(), widths[2]);
                temp[3] = (p.description, widths[3]);
                TableOutput(productsTable, temp);
            }
        }

        static State AddEditProduct(Product p = null)
        {
            string name, description, priceStr;
            decimal price = 0;
            bool success;

            name = ReadLine(String.Format("Введите название товара{0}: ",
                p != null ? " (" + p.name + ")" : ""), p != null);
            do
            {
                priceStr = ReadLine(String.Format("Введите цену проката за час{0}: ",
                    p != null ? " (" + p.price + ")" : ""), p != null);
                if (string.IsNullOrWhiteSpace(priceStr))
                    break;

                success = decimal.TryParse(priceStr, out price);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);
            description = ReadLine(String.Format("Введите описание товара{0}: ",
                p != null ? " (" + p.description + ")" : ""), p != null);

            Console.Clear();
            Console.WriteLine("Название: {0}", string.IsNullOrWhiteSpace(name) ? p.name : name);
            Console.WriteLine("Цена: {0}", string.IsNullOrWhiteSpace(priceStr) ? p.price : price);
            Console.WriteLine("Описание: {0}", string.IsNullOrWhiteSpace(description) ? p.description : description);
            Console.WriteLine("{0} данный товар?", p == null ? "Добавить" : "Сохранить");
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

            if (p == null)
            {
                Lists.products.Add(new Product
                {
                    Id = Lists.MinimalId(Lists.products),
                    name = name,
                    price = price,
                    description = description
                });
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(name)) p.name = name;
                if (!string.IsNullOrWhiteSpace(priceStr)) p.price = price;
                if (!string.IsNullOrWhiteSpace(description)) p.description = description;
            }
            return State.Decide;
        }

        static State EditProduct()
        {
            productsTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID товара, который необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Product p;
            if (!success || (p = Lists.products.FirstOrDefault(t => t.Id == id)) == null)
                return State.Error;

            Console.Clear();
            var s = AddEditProduct(p);
            GenerateRentsTable();
            return s;
        }

        static State DeleteProduct()
        {
            productsTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID товара, который необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Product p;
            if (!success || (p = Lists.products.FirstOrDefault(t => t.Id == id)) == null)
                return State.Error;

            Lists.products.Remove(p);
            Lists.rents.RemoveAll(t => t.product == p);
            GenerateRentsTable();
            return State.Decide;
        }
    }
}
