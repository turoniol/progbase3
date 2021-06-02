using System;
using UserInterface;
using ServiceLib;
using Terminal.Gui;

Application.Init();
Toplevel top = Application.Top;

RemoteService service = null;

try
{
    service = new RemoteService();
}
catch (Exception ex)
{
    MessageBox.ErrorQuery("Error", ex.Message, "Ok");
    Application.Shutdown();
    return;
}

Rect frame = new Rect(4, 8, top.Frame.Width, 200);

// try
{
    // AuthenticationWindow authWindow = new AuthenticationWindow(service);
    // Application.Run(authWindow);

    var admin = service.GetUser(23);
    MainWindow mainWindow = new MainWindow(admin);

    // MainWindow mainWindow = new MainWindow(authWindow.User);
    mainWindow.SetService(service);
    top.Add(mainWindow);
    Application.Run();
}
// catch (System.Exception ex)
// {
//     MessageBox.ErrorQuery("Error", ex.Message, "Ok");
// }

service.Close();