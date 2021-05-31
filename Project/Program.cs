using UserInterface;
using Terminal.Gui;
using EntitiesProcessingLib.Repositories;
using EntitiesProcessingLib.Entities;

string dataBaseFilePath = "./../data/data.db";

Application.Init();

LectureRepository lectureRep = new LectureRepository(dataBaseFilePath);
CourseRepository courseRep = new CourseRepository(dataBaseFilePath);
UserRepository userRep = new UserRepository(dataBaseFilePath);
SubscriptionRepository subRep = new SubscriptionRepository(dataBaseFilePath);

Application.Init();
Toplevel top = Application.Top;

Rect frame = new Rect(4, 8, top.Frame.Width, 200);

// AuthenticationWindow authWindow = new AuthenticationWindow(userRep);
// Application.Run(authWindow);

var admin = userRep.GetUser("admin");

// MainWindow mainWindow = new MainWindow(authWindow.User);
MainWindow mainWindow = new MainWindow(admin);
mainWindow.SetRepositories(userRep, courseRep, lectureRep, subRep);
top.Add(mainWindow);
Application.Run();
