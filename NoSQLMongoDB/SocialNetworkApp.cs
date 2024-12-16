using MongoDB.Driver;
using System.Collections.Generic;
using System;
using System.Linq;

public class SocialNetworkApp
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly UserService _userService;
    private readonly PostService _postService;

    private User _currentUser;

    public SocialNetworkApp()
    {
        // Підключення до MongoDB
        _client = new MongoClient("mongodb+srv://ermolovaira47:q1w2e3r4t5@cluster0.rjoxs.mongodb.net/");
        _database = _client.GetDatabase("SocialNetwork");
        _userService = new UserService(_database);
        _postService = new PostService(_database);
    }

    public void Run()
    {
        while (true)
        {
            if (_currentUser == null)
            {
                ShowMainMenu();
            }
            else
            {
                ShowUserMenu();
            }
        }
    }

    private void ShowMainMenu()
    {
        Console.WriteLine("\n=== Головне меню ===");
        Console.WriteLine("1. Зареєструватися");
        Console.WriteLine("2. Увiйти");
        Console.WriteLine("0. Вийти");
        Console.Write("Виберiть опцiю: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Register();
                break;
            case "2":
                Login();
                break;
            case "0":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Невiрний вибiр. Спробуйте ще раз.");
                break;
        }
    }

    private void ShowUserMenu()
    {
        Console.WriteLine($"\n=== Меню користувача: {_currentUser.FirstName} {_currentUser.LastName} ===");
        Console.WriteLine("1. Додати пост");
        Console.WriteLine("2. Переглянути стрiчку");
        Console.WriteLine("3. Знайти користувача");
        Console.WriteLine("4. Переглянути свої пости");
        Console.WriteLine("5. Вийти");
        Console.Write("Виберiть опцiю: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                CreatePost();
                break;
            case "2":
                ViewFeed();
                break;
            case "3":
                SearchUsers();
                break;
            case "4":
                ViewOwnPosts();
                break;
            case "5":
                _currentUser = null;
                break;
            default:
                Console.WriteLine("Невiрний вибiр. Спробуйте ще раз.");
                break;
        }
    }

    private void Register()
    {
        Console.WriteLine("\n=== Реєстрацiя ===");
        Console.Write("Введiть E-mail: ");
        var email = Console.ReadLine();
        Console.Write("Введiть пароль: ");
        var password = Console.ReadLine();
        Console.Write("Введiть iм'я: ");
        var firstName = Console.ReadLine();
        Console.Write("Введiть прiзвище: ");
        var lastName = Console.ReadLine();
        Console.Write("Введiть вашi iнтереси через кому: ");
        var interests = Console.ReadLine()?.Split(',').Select(s => s.Trim()).ToList() ?? new List<string>();

        var user = _userService.Register(email, password, firstName, lastName, interests);
        if (user != null)
        {
            Console.WriteLine("Реєстрацiя успiшна!");
        }
        else
        {
            Console.WriteLine("Користувач з таким email вже iснує.");
        }
    }

    private void Login()
    {
        Console.WriteLine("\n=== Логiн ===");
        Console.Write("Введiть E-mail: ");
        var email = Console.ReadLine();
        Console.Write("Введiть пароль: ");
        var password = Console.ReadLine();

        _currentUser = _userService.Login(email, password);
        if (_currentUser != null)
        {
            Console.WriteLine($"Ласкаво просимо, {_currentUser.FirstName}!");
        }
        else
        {
            Console.WriteLine("Невiрний E-mail або пароль.");
        }
    }

    private void CreatePost()
    {
        Console.WriteLine("\n=== Додавання посту ===");
        Console.Write("Введiть текст посту: ");
        var content = Console.ReadLine();
        _postService.CreatePost(_currentUser, content);
        Console.WriteLine("Пост успiшно додано!");
    }

    private void ViewFeed()
    {
        Console.WriteLine("\n=== Стрiчка ===");
        var posts = _postService.GetFeed();
        foreach (var post in posts)
        {
            Console.WriteLine($"[{post.CreatedAt}] {post.User.FirstName} {post.User.LastName}: {post.Content}");
        }
    }

    private void SearchUsers()
    {
        Console.WriteLine("\n=== Пошук користувачiв ===");
        Console.Write("Введіть iм'я або прiзвище для пошуку: ");
        var query = Console.ReadLine();
        var users = _userService.SearchUsers(query);
        foreach (var user in users)
        {
            Console.WriteLine($"{user.FirstName} {user.LastName} (Interests: {string.Join(", ", user.Interests)})");
        }
    }

    private void ViewOwnPosts()
    {
        Console.WriteLine("\n=== Вашi пости ===");
        var posts = _postService.GetPostsByUser(_currentUser);
        foreach (var post in posts)
        {
            Console.WriteLine($"[{post.CreatedAt}] {post.Content}");
        }
    }
}
