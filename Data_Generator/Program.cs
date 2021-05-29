using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using EntitiesProcessingLib.Repositories;
using EntitiesProcessingLib.Entities;

class Program
{
    static LectureRepository _lectureRep;
    static CourseRepository _courseRep;
    static UserRepository _userRep;

    static void Main(string[] args)
    {
        string dataBaseFilePath = "./../data/data.db";
        _lectureRep = new LectureRepository(dataBaseFilePath);
        _courseRep = new CourseRepository(dataBaseFilePath);
        _userRep = new UserRepository(dataBaseFilePath);

        ParseArgs(args);
    }

    static void ParseArgs(string[] args)
    {
        // 1 type 2 count
        if (args.Length != 2)
        {
            throw new ArgumentException();
        }

        Dictionary<string, Action<int>> functions = new Dictionary<string, Action<int>>();
        functions.Add("lectures", GenerateLectures);
        functions.Add("courses", GenerateCourses);
        functions.Add("users", GenerateUsers);

        Action<int> generate = null;
        if (!functions.TryGetValue(args[0], out generate))
        {
            throw new ArgumentException($"Invalid operation {args[0]}.");
        }

        int count;
        if (!int.TryParse(args[1], out count))
        {
            throw new ArgumentException("Invalid count type.");
        }

        generate(count);
    }

    static void GenerateLectures(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            PutRandomLecture();
        }
    }

    static void GenerateCourses(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            PutRandomCourse();
        }
    }

    static void GenerateUsers(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            PutRandomUser();
        }
    }

    static void PutRandomLecture()
    {
        Lecture lection = new Lecture();
        lection.Theme = GetRandomLineFromFile("./../data/generator/lectures_themes.csv");
        lection.course = new Course();

        var coursesIDs = _courseRep.GetAllIDs();
        if (coursesIDs.Count == 0)
        {
            throw new Exception("Can't make a relationship with course, courses table is empty.");
        }

        lection.course.ID = coursesIDs[new Random().Next(0, coursesIDs.Count)];
        long ID = _lectureRep.Insert(lection);
    }

    static void PutRandomCourse()
    {
        string name = GetRandomLineFromFile("./../data/generator/courses_names.csv");
        Course course = new Course();
        course.Name = name;
        course.Author = new User();
        
        var usersIDs = _userRep.GetAllIDs();
        if (usersIDs.Count == 0)
        {
            throw new Exception("Can't make a relationship with course, courses table is empty.");
        }
        course.Author.ID = usersIDs[new Random().Next(0, usersIDs.Count)];

        long ID = _courseRep.Insert(course);
    }

    static void PutRandomUser()
    {
        User user = new User();
        user.Login = GetRandomSentence("./../data/generator/words.txt", 1, 1);
        user.Password = GetRandomSentence("./../data/generator/words.txt", 1, 1);
        string fname = GetRandomSentence("./../data/generator/first_names.txt", 1, 1);
        string lname = GetRandomSentence("./../data/generator/last_names.txt", 1, 1);
        user.Fullname = fname + lname;

        long userID = _userRep.Insert(user);

        var coursesIDs = _courseRep.GetAllIDs();
        Random rand = new Random();
        int subsribers = rand.Next(1, 10);

        for (int i = 0; i < subsribers; ++i)
        {
            long courseID = coursesIDs[rand.Next(0, coursesIDs.Count - 1)];
            Course course = new Course();
            course.ID = courseID;
            user.ID = userID;
            _courseRep.MakeRelationship(user, course);
        }
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

    static string GetRandomLineFromFile(string fileDataPath)
    {
        Random rand = new Random();

        int fileLinesCount = CountLinesInFile(fileDataPath);
        int pos = rand.Next(1, fileLinesCount + 1);

        StreamReader streamReader = new StreamReader(fileDataPath);
        string word = "";

        for (int j = 0; j < pos; ++j)
        {
            word = streamReader.ReadLine();
        }

        streamReader.Close();

        return word;
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
