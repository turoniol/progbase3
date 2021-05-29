using UserInterface;
using Terminal.Gui;
using EntitiesProcessingLib.Repositories;

string dataBaseFilePath = "./../data/data.db";

Application.Init();
MainWindow mainWindow = new MainWindow(dataBaseFilePath);

var lectureRep = new LectureRepository(dataBaseFilePath);
var courseRep = new CourseRepository(dataBaseFilePath);
var userRep = new UserRepository(dataBaseFilePath);

Application.Init();
Toplevel top = Application.Top;

Rect frame = new Rect(4, 8, top.Frame.Width, 200);

mainWindow.SetRepositories(userRep, courseRep, lectureRep);
top.Add(mainWindow);

Application.Run();