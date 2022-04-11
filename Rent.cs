using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportRent
{
    public class Rent : IId
    {
        int id;
        public Client client;
        public Product product;
        public int count;
        public DateTime beginDate;
        public DateTime endDate;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public decimal Price
        {
            get
            {
                return product.price * count * (100 - client.discount) * 
                    (int)Math.Floor((endDate - beginDate).TotalHours);
            }
        }
    }

    public class Product : IId
    {
        int id;
        public string name;
        public decimal price;
        public int count;
        public string description;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class Client : IId
    {
        int id;
        public string lastName;
        public string firstName;
        public string midName;
        public string address;
        public string contacts;
        public decimal discount;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}.{2}.", lastName, firstName[0], midName[0]);
        }
    }

    public interface IId
    {
        int Id { get; set; }
    }

    static class Lists
    {
        public static List<Product> products = new List<Product>();
        public static List<Rent> rents = new List<Rent>();
        public static List<Client> clients = new List<Client>();

        public static int ProductRent(Product p)
        {
            return rents.Count(t => t.product == p && t.endDate > DateTime.Now);
        }

        public static int MinimalId(IEnumerable<IId> list)
        {
            if (list.Count() == 0)
                return 0;

            for (int i = 0; i < list.Count(); i++)
            {
                if (list.ElementAt(i).Id != i)
                    return i;
            }
            return list.Last().Id + 1;
        }
    }
}
