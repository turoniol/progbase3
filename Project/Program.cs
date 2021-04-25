using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Repositories;

public class User
{
    public User(string login, string password, string status)
    {
        this.Login = login;
        this.Password = password;
        this.Status = status;

    }
    public string Login { get; private set; }
    public string Password { get; private set; }
    public string Status { get; private set; }

}
public class Lection
{
    public Lection(string theme)
    {
        // this.ID = iD;
        this.Theme = theme;

    }
    // public int ID { get; private set; }
    public string Theme { get; private set; }
}

public class Course
{
    // public int ID { get; private set; }
    public string Author { get; private set; }
    public string Name { get; private set; }
    public int NumberOfSubscribers { get; private set; }
    public int NumberOfLections { get; private set; }

    public Course(string author, string name, int numberOfSubscribers, int numberOfLections)
    {
        this.Author = author;
        this.Name = name;
        this.NumberOfSubscribers = numberOfSubscribers;
        this.NumberOfLections = numberOfLections;

    }
}

class Program
{
    static LectionRepository _lectionRep;
    static CourseRepository _courseRep;
    static UserRepository _userRep;

    static void Main(string[] args)
    {
        try
        {
            // 1 type 2 count 3 infimum 4 supremum
            if (args.Length != 4)
            {
                throw new ArgumentException();
            }

            _lectionRep = new LectionRepository("./../data/data.db");
            _courseRep = new CourseRepository("./../data/data.db");
            _userRep = new UserRepository("./../data/data.db");

            ParseArgs(args);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    static void ParseArgs(string[] args)
    {
        Dictionary<string, Action<int,int,int>> functions = new Dictionary<string, Action<int, int, int>>();
        functions.Add("lections", GenerateLections);
        functions.Add("courses", GenerateCourses);

        Action<int, int, int> generate = null;
        if (!functions.TryGetValue(args[0], out generate))
        {
            throw new ArgumentException($"Invalid operation {args[0]}.");
        }

        int count;
        if (!int.TryParse(args[1], out count))
        {
            throw new ArgumentException("Invalid count type.");
        }

        int a;
        if (!int.TryParse(args[2], out a))
        {
            throw new ArgumentException("Invalid a type.");
        }
        if (a < 0)
        {
            throw new ArgumentException("a must be greater than 0.");
        }

        int b;
        if (!int.TryParse(args[3], out b))
        {
            throw new ArgumentException("Invalid b type.");
        }
        if (b < 0)
        {
            throw new ArgumentException("b must be greater than 0.");
        }

        generate(count, Math.Min(a, b), Math.Max(a, b));
    }

    static void GenerateLections(int count, int infimum, int supremum)
    {
        for (int i = 0; i < count; ++i)
        {
            PutRandomLection(infimum, supremum);
        }
    }

    static void GenerateCourses(int count, int infimum, int supremum)
    {
        for (int i = 0; i < count; ++i)
        {
            PutRandomCourse(infimum, supremum);
        }
    }

    static void PutRandomLection(int inf, int sup)
    {
        string theme = GetRandomSentence("./../data/generator/words.txt", 1, 5);
        long ID = _lectionRep.Insert(new Lection(theme), new Random().Next(inf, sup));
    }

    static void PutRandomCourse(int infimum, int supremum)
    {
        string fname = GetRandomSentence("./../data/generator/first_names.txt", 1, 1);
        string lname = GetRandomSentence("./../data/generator/last_names.txt", 1, 1);
        string author = lname + fname;
        string name = GetRandomSentence("./../data/generator/words.txt", 1, 10);
        Random rand = new Random();
        int subsribers = rand.Next(infimum, supremum);
        int lections = rand.Next(infimum, supremum);

        long ID = _courseRep.Insert(new Course(author, name, subsribers, lections));
    }

    static string GetRandomSentence(string fileDataPath, int min, int max)
    {
        if (min < 1)
        {
            throw new ArgumentException();
        }

        StringBuilder stringBuilder = new StringBuilder();
        Random rand = new Random();

        int wordsCount = rand.Next(min, max + 1);
        int fileLinesCount = CountLinesInFile(fileDataPath);

        for (int i = 0; i < wordsCount; ++i)
        {
            StreamReader streamReader = new StreamReader(fileDataPath);
            int wordPosition = rand.Next(1, fileLinesCount + 1);

            for (int j = 0; j < wordPosition; ++j)
            {
                streamReader.ReadLine();
            }

            stringBuilder.Append(streamReader.ReadLine() + " ");
            streamReader.Close();
        }
        string text = stringBuilder.ToString();

        return text;
    }

    static int CountLinesInFile(string path)
    {
        StreamReader sr = new StreamReader(path);
        int counter = 0;
        while (sr.ReadLine() != null)
        {
            counter += 1;
        }
        return counter;
    }
}
