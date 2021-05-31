using Terminal.Gui;
using EntitiesProcessingLib.Repositories;
using EntitiesProcessingLib.Entities;
using NStack;

namespace UserInterface
{
    public class LectureView : Window
    {
        private int _pageSize = 15;
        private int _totalPages;
        private int _page = 1;
        private User _loginedUser;
        private ListView _view;
        private LectureRepository _lectureRep;
        private CourseRepository _courseRep;
        private Label _pageLabel;
        private Button _prevPage;
        private Button _nextPage;
        private TextField _searchField;

        public LectureView()
        {
            FrameView frame = new FrameView
            {
                Title = "Lectures",
                X = 0,
                Y = 4, 
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            _view = new ListView
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            frame.Add(_view);
            this.Add(frame);

            _prevPage = new Button(2, 2, "Prev");
            _nextPage = new Button(20, 2, "Next");
            _pageLabel = new Label(12, 2, "??? / ???");
            _searchField = new TextField() {
                X = 2, Y = 3, Width = 40,
            };

            _prevPage.Clicked += OnPrev;
            _nextPage.Clicked += OnNext;
            _view.OpenSelectedItem += OnItemOpen;
            _searchField.TextChanged += OnSearch;
            
            this.Add(_prevPage, _pageLabel, _nextPage, _searchField);
        }
 
        public void LoginUser(User user) => _loginedUser = user;

        private void OnSearch(ustring obj)
        {
            UpdateView();
        }

        private void OnItemOpen(ListViewItemEventArgs obj)
        {
            Lecture selected = (Lecture) obj.Value;
            LectureInfo dlg = new LectureInfo();
            dlg.LoginUser(_loginedUser);
            dlg.SetRepository(_lectureRep, _courseRep);
            dlg.SetLecture(selected);
            Application.Run(dlg);
            UpdateView();
        }

        public void OnNext()
        {
            _page += 1;
            UpdateView();
        }

        public void OnPrev()
        {
            _page -= 1;
            UpdateView();
        }

        public void SetRepository(LectureRepository lectureRepository, CourseRepository courseRepository)
        {
            _lectureRep = lectureRepository;
            _courseRep = courseRepository;
            UpdateView();
        }

        public void UpdateView()
        {
            string name = _searchField.Text.ToString();
            _totalPages = _lectureRep.GetTotalPagesCount(_pageSize, name);
            _prevPage.Visible = (_page != 1);
            _nextPage.Visible = (_page != _totalPages);

            _view.SetSource(_lectureRep.GetPage(_page, _pageSize, name));
            _pageLabel.Text = $"{_page} / {_totalPages}";

            if (_totalPages == 0)
            {
                _prevPage.Visible = false;
                _nextPage.Visible = false;
                _pageLabel.Text = "0 / 0";
            }
        }
    }
}