using Terminal.Gui;
using EntitiesProcessingLib.Repositories;
using EntitiesProcessingLib.Entities;

namespace UserInterface
{
    public class LectureView : Dialog
    {
        private int _pageSize = 15;
        private int _totalPages;
        private int _page = 1;
        private ListView _view;
        private LectureRepository _repo;
        private Label _pageLabel;
        private Button _prevPage;
        private Button _nextPage;

        public LectureView()
        {
            FrameView frame = new FrameView
            {
                Title = "View",
                X = 0,
                Y = 4, 
                Width = Application.Top.Frame.Width,
                Height = Application.Top.Frame.Height - 4,
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

            _prevPage.Clicked += OnPrev;
            _nextPage.Clicked += OnNext;
            _view.OpenSelectedItem += OnItemOpen;

            Button okBtn = new Button("Ok");
            okBtn.Clicked += Application.RequestStop;
            this.AddButton(okBtn);
            
            this.Add(_prevPage, _pageLabel, _nextPage);
        }

        private void OnItemOpen(ListViewItemEventArgs obj)
        {
            Lecture selected = (Lecture) obj.Value;
            LectureInfo dlg = new LectureInfo();
            dlg.SetRepository(_repo);
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

        public void SetRepository(LectureRepository repo)
        {
            _repo = repo;
            _view.SetSource(repo.GetPage(_page, _pageSize));
            UpdateView();
        }

        private void UpdateView()
        {
            _totalPages = _repo.GetTotalPagesCount(_pageSize);
            _prevPage.Visible = (_page != 1);
            _nextPage.Visible = (_page != _totalPages);

            _view.SetSource(_repo.GetPage(_page, _pageSize));
            _pageLabel.Text = $"{_page} / {_totalPages}";
        }
    }
}