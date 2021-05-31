using EntitiesProcessingLib.Entities;
using EntitiesProcessingLib.Repositories;
using Terminal.Gui;

namespace UserInterface
{
    public class LectureInfo : Dialog
    {
        private User _loginedUser;
        private CourseRepository _courseRep;
        private LectureRepository _lectureRep;
        private TextField _idView;
        private TextField _themeView;
        private TextField _courseIDView;
        private CheckBox _importedBox;
        private Button _deleteBtn;
        private Button _updateBtn;

        public LectureInfo()
        {
            this.Width = 60;
            this.Height = 14;
            Button okBtn = new Button("Ok");
            okBtn.Clicked += Application.RequestStop;
            this.AddButton(okBtn);

            int yShift = 2;
            Label id = new Label(1, 0 * yShift, "ID: ");
            Label theme = new Label(1, 1 * yShift, "Theme: ");
            Label courseID = new Label(1, 2 * yShift, "Course ID: ");
            Label imported = new Label(1, 3 * yShift, "Is imported: ");
            this.Add(id, theme, courseID, imported);

            int xShift = 15;
            _idView = new TextField()
            {
                X = xShift,
                Y = 0 * yShift,
                ReadOnly = true,
                Width = 50,
            };
            _themeView = new TextField()
            {
                X = xShift,
                Y = 1 * yShift,
                ReadOnly = true,
                Width = 50,
            };
            _courseIDView = new TextField()
            {
                X = xShift,
                Y = 2 * yShift,
                ReadOnly = true,
                Width = 50,
            };
            _importedBox = new CheckBox()
            {
                X = xShift,
                Y = 3 * yShift,
            };
            _importedBox.Toggled += OnToggled;
            this.Add(_idView, _themeView, _courseIDView, _importedBox);

            _deleteBtn = new Button("Delete")
            {
                X = 1,
                Y = 4 * yShift,
            };
            _updateBtn = new Button("Update")
            {
                X = 15,
                Y = 4 * yShift,
            };
            _deleteBtn.Clicked += OnDelete;
            _updateBtn.Clicked += OnUpdate;
            this.Add(_deleteBtn, _updateBtn);
        }

        public void LoginUser(User user) => _loginedUser = user;

        private void OnUpdate()
        {

            try
            {
                LectureUpdate dlg = new LectureUpdate();
                long id = long.Parse(_idView.Text.ToString());
                dlg.SetLecture(_lectureRep.GetLecture(id));
                Application.Run(dlg);

                if (dlg.canceled)
                {
                    return;
                }            
                var lecture = dlg.Lecture;
                var course = _courseRep.GetCourse(lecture.Course.ID);
                if (course == null || course.Author.ID != _loginedUser.ID)
                {
                    MessageBox.ErrorQuery("Error", $"The course with id [{lecture.Course.ID}] isn't your", "Ok");
                    return;
                }

                this.SetLecture(dlg.Lecture);
                _lectureRep.Update(id, dlg.Lecture);
            }
            catch
            {
                MessageBox.ErrorQuery("Error", "Some fields is empty!", "Ok");
            }
        }

        private void OnDelete()
        {
            int decision = MessageBox.Query("Deleting", "Do you reaaly want to delete?", "Yes", "No");
            if (decision != 0)
            {
                return;
            }

            _lectureRep.Delete(long.Parse(_idView.Text.ToString()));
            Application.RequestStop();
        }

        public void SetRepository(LectureRepository lectureRepository, CourseRepository courseRepository)
        {
            _lectureRep = lectureRepository;
            _courseRep = courseRepository;
        }

        public void SetLecture(Lecture l)
        {
            _idView.Text = l.ID.ToString();
            _themeView.Text = l.Theme;
            _courseIDView.Text = l.Course.ID.ToString();
            _importedBox.Checked = l.IsImported;

            var course = _courseRep.GetCourse(l.Course.ID);
            if (course == null)
            {
                _deleteBtn.Visible = false;
                _updateBtn.Visible = false;
                return;
            }

            _deleteBtn.Visible = course.Author.ID == _loginedUser.ID;
            _updateBtn.Visible = course.Author.ID == _loginedUser.ID;
        }

        private void OnToggled(bool obj)
        {
            _importedBox.Checked = obj;
        }
    }
}