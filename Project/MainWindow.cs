using Terminal.Gui;
using EntitiesProcessingLib.Repositories;
using EntitiesProcessingLib.DataProcessing;
using System;

namespace UserInterface
{
    public class MainWindow : Window
    {
        private LectureRepository _lectureRep;
        private CourseRepository _courseRep;
        private UserRepository _userRep;

        public MainWindow(string dataBaseFilePath)
        {
            // geometry
            this.Height = Dim.Fill();
            this.Width = Dim.Fill();
            this.Y = 1;
            this.X = 0;
            int xShift = 2, yShift = 2;

            MenuBar menuBar = new MenuBar(new MenuBarItem[]
            {
                new MenuBarItem ("_File", new MenuItem[]
                {
                    new MenuItem("_Export...", "Export courses into XML", OnExport),
                    new MenuItem("_Import...", "Import courses from XML", OnImport),
                    new MenuItem("_Quit", "Quit program", Application.RequestStop),
                }),
                new MenuBarItem("_Help", new MenuItem[]
                {
                    new MenuItem("_Help", "", Application.RequestStop),
                }),
            });

            Button createButton = new Button(xShift, 1 * yShift, "New");
            Button viewButton = new Button(xShift, 2 * yShift, "Open view");
            createButton.Clicked += OnCreateButton;
            viewButton.Clicked += OnViewButton;
            this.Add(createButton, viewButton);

            Application.Top.Add(menuBar, this);
        }

        private void OnViewButton()
        {
            EntityChoosingDialog dlg = new EntityChoosingDialog();
            Application.Run(dlg);

            if (dlg.canceled)
            {
                return;
            }

            switch (dlg.Entity)
            {
                case EntityType.User:
                    UserView userView = new UserView();
                    Application.Top.Add(userView);
                    userView.SetRepository(_userRep);
                    Application.Run(userView);
                    Application.Top.Remove(userView);
                    break;
                case EntityType.Course:
                    CourseView courseView = new CourseView();
                    Application.Top.Add(courseView);
                    courseView.SetRepository(_courseRep);
                    Application.Run(courseView);
                    Application.Top.Remove(courseView);
                    break;
                case EntityType.Lecture:
                    LectureView lectureView = new LectureView();
                    Application.Top.Add(lectureView);
                    lectureView.SetRepository(_lectureRep);
                    Application.Run(lectureView);
                    Application.Top.Remove(lectureView);
                    break;
            }
        }

        public void SetRepositories(UserRepository userRepository, 
            CourseRepository courseRepository, LectureRepository lectureRepository)
        {
            _userRep = userRepository;
            _courseRep = courseRepository;
            _lectureRep = lectureRepository;
        }

        private void OnCreateButton()
        {
            EntityChoosingDialog dlg = new EntityChoosingDialog();
            Application.Run(dlg);

            if (dlg.canceled)
            {
                return;
            }

            switch (dlg.Entity)
            {
                case EntityType.User:
                    UserCreatingDialog userDlg = new UserCreatingDialog();
                    Application.Run(userDlg);

                    if (userDlg.canceled) return;

                    _userRep.Insert(userDlg.user);

                    break;
                case EntityType.Course:
                    CourseCreatingDialog courseDlg = new CourseCreatingDialog();
                    Application.Run(courseDlg);

                    if (courseDlg.canceled) return;

                    _courseRep.Insert(courseDlg.course);

                    break;
                case EntityType.Lecture:
                    LectureCreatingDialog lectureDlg = new LectureCreatingDialog();
                    Application.Run(lectureDlg);

                    if (lectureDlg.canceled) return;

                    _lectureRep.Insert(lectureDlg.lecture);

                    break;
            }
        }

        private void OnExport()
        {
            ExportCourseDialog dlg = new ExportCourseDialog();
            Application.Run(dlg);

            if (dlg.canceled) return;

            CourseProcessor processor = new CourseProcessor(_courseRep, _lectureRep);
            try
            {
                processor.Export(dlg.Word, dlg.FilePath);
                MessageBox.Query("Message", "Succesfull!", "Ok");
            }
            catch (System.Exception ex)
            {
                MessageBox.ErrorQuery("Error", ex.Message, "Ok");
            }
        }

        private void OnImport()
        {
            OpenDialog dlg = new OpenDialog("Open XML file", "Open?")
            {
                CanChooseDirectories = false,
            };
            Application.Run(dlg);

            if (dlg.Canceled) return;

            CourseProcessor processor = new CourseProcessor(_courseRep, _lectureRep);
            try
            {
                processor.Import(dlg.FilePath.ToString());
                MessageBox.Query("Message", "Succesfull!", "Ok");
            }
            catch (System.Exception ex)
            {
                MessageBox.ErrorQuery("Error", ex.Message, "Ok");
            }
        }
    }
}