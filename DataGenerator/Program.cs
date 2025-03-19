using Bogus;
using Model;
using DAL;
using System.Collections.Generic;

namespace DataGenerator
{

    public static class DataGenerator
    {
        public static void GenerateData(WebstoreContext context)
        {
            // Generowanie kategorii
            var categoryFaker = new Faker<ProductGroup>()
                .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
                .RuleFor(c => c.ParentId, f => f.Random.Bool(0.3f) ? f.Random.Int(1, 5) : (int?)null);

            var categories = categoryFaker.Generate(10);
            context.ProductGroups.AddRange(categories);
            context.SaveChanges();

            // Generowanie produktów
            var productFaker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => f.Finance.Amount(10, 500))
                .RuleFor(p => p.Image, f => f.Image.ToString())
                .RuleFor(p => p.IsActive, f => f.Random.Bool(0.3f))
                .RuleFor(p => p.GroupID, f => f.PickRandom(categories).ID);

            var products = productFaker.Generate(50);
            context.Products.AddRange(products);
            context.SaveChanges();

            // Generowanie grup użytkowników
            var userGroupsFaker = new Faker<UserGroup>()
                .RuleFor(ug => ug.Name, f => f.Random.Word());

            var userGroups = userGroupsFaker.Generate(10);
            context.UserGroups.AddRange(userGroups);
            context.SaveChanges();

            // Generowanie użytkowników
            var userFaker = new Faker<User>()
                .RuleFor(u => u.Login, f => f.Person.UserName)
                .RuleFor(u => u.Password, f => f.Random.Word());

            var users = userFaker.Generate(20);
            context.Users.AddRange(users);
            context.SaveChanges();

            // Generowanie pozycji koszyka
            var basketPositionFaker = new Faker<BasketPosition>()
                .RuleFor(bp => bp.UserID, f => f.PickRandom(users).ID)
                .RuleFor(bp => bp.ProductID, f => f.PickRandom(products).ID)
                .RuleFor(bp => bp.Amount, f => f.Random.Int());

            var basketPositions = new HashSet<(int userID, int productID)>();
            var basketList =new List<BasketPosition>();

            foreach (var user in users)
            {
                foreach (var product in products.OrderBy(_ => Guid.NewGuid()).Take(3))
                {
                    if (basketPositions.Add((user.ID, product.ID)))
                    {
                        basketList.Add(new BasketPosition { UserID = user.ID, ProductID = product.ID, Amount = new Random().Next(1, 5) });
                    }
                }
            }
            context.BasketPositions.AddRange(basketList);
            context.SaveChanges();
            // Generowanie zamówień
            var orderFaker = new Faker<Order>()
                .RuleFor(o => o.UserID, f => f.PickRandom(users).ID)
                .RuleFor(o => o.IsPaid, f => f.Random.Bool(0.3f))
                .RuleFor(o => o.Date, f => f.Date.Past(1));

            var orders = orderFaker.Generate(30);
            context.Orders.AddRange(orders);
            context.SaveChanges();

            // Generowanie pozycji zamówień
            var orderItemFaker = new Faker<OrderPosition>()
                .RuleFor(op => op.OrderID, f => f.PickRandom(orders).ID)
                .RuleFor(op => op.ProductID, f => f.PickRandom(products).ID)
                .RuleFor(op => op.Amount, f => f.Random.Int(1, 5))
                .RuleFor(op => op.Price, (f, op) => products.First(p => p.ID == op.ProductID).Price);

            var orderItems = orderItemFaker.Generate(100);
            var orderPositions = new HashSet<(int orderId, int productId)>();
            var orderPositionsList = new List<OrderPosition>();

            foreach (var order in orders)
            {
                foreach (var product in products.OrderBy(_ => Guid.NewGuid()).Take(3))
                {
                    if (orderPositions.Add((order.ID, product.ID)))
                    {
                        orderPositionsList.Add(new OrderPosition
                        {
                            OrderID = order.ID,
                            ProductID = product.ID,
                            Amount = new Random().Next(1, 5),
                            Price = product.Price
                        });
                    }
                }
            }
            context.OrderPositions.AddRange(orderPositionsList);
            context.SaveChanges();

            // Wyświetlenie wyników
            Console.WriteLine("Kategorie:");
            categories.ForEach(c => Console.WriteLine($"{c.ID}: {c.Name} (Parent: {c.ParentId})"));

            Console.WriteLine("\nProdukty:");
            products.ForEach(p => Console.WriteLine($"{p.ID}: {p.Name} - {p.Price:C} (Kategoria: {p.GroupID})"));

            Console.WriteLine("\nUżytkownicy:");
            users.ForEach(u => Console.WriteLine($"{u.ID}: {u.Login} - {u.Password}"));

            Console.WriteLine("\nZamówienia:");
            //orders.ForEach(o =>
            //{
            //    Console.WriteLine($"Zamówienie {o.ID}: Użytkownik {o.UserID}, Data: {o.Date}, Kwota: {o.Value:C}");
            //    foreach (var item in o.OrderPositions)
            //    {
            //        Console.WriteLine($"\tProdukt {item.ProductID} - Ilość: {item.Amount}, Cena: {item.Price:C}");
            //    }
            //});

            context.SaveChanges();
        }
    }
    public class Program
    {
        static void Main(string[] args)
        {
            WebstoreContext context = new WebstoreContext();
            DataGenerator.GenerateData(context);
        }
    }
}
