using Terminal.Gui;
using EntitiesProcessingLib.Entities;
using ServiceLib;
using System;

namespace UserInterface
{
    public class MainWindow : Window
    {
        private RemoteService _service;
        private Label _userInfo;
        private User _loginedUser;
        private EntityChoosingWindow _choosingWin;
        private CourseView _courseView;
        private UserView _userView;
        private LectureView _lectureView;

        public MainWindow(User user)
        {
            _loginedUser = user;

            // geometry
            this.Height = Dim.Fill();
            this.Width = Dim.Fill();
            this.Y = 1;
            this.X = Pos.Center();
            int xShift = 15;

            MenuBar menuBar = new MenuBar(new MenuBarItem[]
            {
                new MenuBarItem ("_File", new MenuItem[]
                {
                    new MenuItem("_Export...", "Export courses into XML", OnExport),
                    new MenuItem("_Import...", "Import courses from XML", OnImport),
                    new MenuItem("_Generate report...", "Generate .docx file", OnReport),
                    new MenuItem("_Quit", "Quit program", Application.RequestStop),
                }),
                new MenuBarItem("_Help", new MenuItem[]
                {
                    new MenuItem("_Help", "", OnHelp),
                }),
            });

            Button createCourseButton = new Button(1 + 0 * xShift, 1, "New course");
            Button createLectureButton = new Button(1 + 1 * xShift, 1, "New lecture");
            Button logoutButton = new Button(3 * xShift, 1, "Log out");

            _lectureView = new LectureView() { Visible = false, };
            _userView = new UserView() { Visible = false, };
            _courseView = new CourseView();

            _userView.UserDeleted += OnLogout;
            Login(user);
            
            int choosingWinHeight = 10, choosingWinWidth = 30;
            _choosingWin = new EntityChoosingWindow()
            {
                X = Pos.AnchorEnd(choosingWinWidth),
                Y = Pos.AnchorEnd(choosingWinHeight),
            };

            createCourseButton.Clicked += OnCourseCreate;
            createLectureButton.Clicked += OnLectureCreate;
            logoutButton.Clicked += OnLogout;
            _choosingWin.ItemChanged += OnSelectedItemChanged;

            this.Add(_courseView, _lectureView, _userView);
            this.Add(createCourseButton, createLectureButton, logoutButton, _choosingWin);

            Application.Top.Add(menuBar, this);
            SetUserInfo();
        }


        private void OnReport()
        {
            try
            {
                _service.GenerateReport();
            }
            catch (System.Exception ex)
            {
                MessageBox.ErrorQuery("Error", ex.Message, "Ok");
            }
        }

        private void Login(User user)
        {
            _courseView.LoginUser(_loginedUser);
            _userView.LoginUser(_loginedUser);
            _lectureView.LoginUser(_loginedUser);
        }

        private void OnSelectedItemChanged()
        {
            _userView.Visible = false;
            _courseView.Visible = false;
            _lectureView.Visible = false;

            switch (_choosingWin.Selected())
            {
                case EntityType.Course:
                    _courseView.Visible = true;
                    _courseView.UpdateView();
                    break;
                case EntityType.Lecture:
                    _lectureView.Visible = true;
                    _lectureView.UpdateView();
                    break;
                case EntityType.User:
                    _userView.Visible = true;
                    _userView.UpdateView();
                    break;
            }
        }

        private void OnLectureCreate()
        {
            LectureCreatingDialog dlg = new LectureCreatingDialog();
            Application.Run(dlg);

            if (dlg.canceled) return;

            var lecture = dlg.lecture;
            var course = _service.GetCourse(lecture.Course.ID);
            if (course == null || course.Author.ID != _loginedUser.ID)
            {
                MessageBox.ErrorQuery("Error", $"The course with id [{lecture.Course.ID}] isn't your", "Ok");
                return;
            }
            
            _service.Insert(lecture);
            _lectureView.UpdateView();
        }

        private void OnCourseCreate()
        {
            CourseCreatingDialog dlg = new CourseCreatingDialog(_loginedUser);
            Application.Run(dlg);

            if (dlg.canceled) return;

            _service.Insert(dlg.course);
            _courseView.UpdateView();
        }

        private void OnLogout()
        {
            int decision = MessageBox.Query("Log out", "Are you sure?", "Yes", "No");

            if (decision == 1) return;

            this.Visible = false;
            AuthenticationWindow window = new AuthenticationWindow(_service);
            Application.Run(window);
            _loginedUser = window.User;
            SetUserInfo();

            Login(_loginedUser);
            this.Visible = true;
        }

        private void SetUserInfo()
        {
            if (_userInfo != null) 
            {
                this.Remove(_userInfo);
            }
            _userInfo = new Label($"Logined as: {_loginedUser.Login}")
            {
                X = 2,
            };
            this.Add(_userInfo);
        }

        public void SetService(RemoteService service)
        {
            _service = service;
            _courseView.SetService(service);
            _lectureView.SetService(service);
            _userView.SetService(service);
        }

        private void OnExport()
        {
            ExportCourseDialog dlg = new ExportCourseDialog();
            Application.Run(dlg);

            if (dlg.canceled) return;

            try
            {
                _service.Export(dlg.Word, dlg.FilePath);
                MessageBox.Query("Message", "Succesfull!", "Ok");
            }
            catch (System.Exception ex)
            {
                MessageBox.ErrorQuery("Error", ex.Message, "Ok");
            }
        }

        private void OnHelp()
        {
            MessageBox.Query("About", "This is a simple program for courses managment.", "Ok");
        }

        private void OnImport()
        {
            OpenDialog dlg = new OpenDialog("Open XML file", "Open?")
            {
                CanChooseDirectories = false,
            };
            Application.Run(dlg);

            if (dlg.Canceled) return;

            try
            {
                _service.Import(dlg.FilePath.ToString());
                MessageBox.Query("Message", "Succesfull!", "Ok");
            }
            catch (System.Exception ex)
            {
                MessageBox.ErrorQuery("Error", ex.Message, "Ok");
            }
        }
    }
}