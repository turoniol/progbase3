using System;
using EntitiesProcessingLib.Entities;
using EntitiesProcessingLib.Repositories;
using Terminal.Gui;

namespace UserInterface
{
    public class LectureUpdate : Dialog
    {
        public bool canceled;
        public Lecture Lecture {get; private set;}
        private TextField _idView;
        private TextField _themeView;
        private TextField _courseIDView;
        private CheckBox _importedBox;

        public LectureUpdate()
        {
            this.Width = 60;
            this.Height = 14;
            Button okBtn = new Button("Ok");
            Button cancelBtn = new Button("Cancel");
            okBtn.Clicked += OnApply;
            cancelBtn.Clicked += OnCancel;

            this.AddButton(okBtn);
            this.AddButton(cancelBtn);

            int yShift = 2;
            Label id = new Label(1, 0 * yShift, "ID: ");
            Label theme = new Label(1, 1 * yShift, "Theme: ");
            Label courseID = new Label(1, 2 * yShift, "Course ID: ");
            Label imported = new Label(1, 3 * yShift, "Is imported: ");
            this.Add(id, theme, courseID, imported);

            int xShift = 15;
            _idView = new TextField() {
                X = xShift, Y = 0 * yShift, Width = 50, ReadOnly = true,
            };
            _themeView = new TextField() {
                X = xShift, Y = 1 * yShift, Width = 50,
            };
            _courseIDView = new TextField() {
                X = xShift, Y = 2 * yShift, Width = 50,
            };
            _importedBox = new CheckBox() {
                X = xShift, Y = 3* yShift,
            };

            _courseIDView.TextChanging += OnCourseIDInput;

            this.Add(_idView, _themeView, _courseIDView, _importedBox);
        }

        private void OnCancel()
        {
            canceled = true;
            Lecture = null;
            Application.RequestStop();
        }

        private void OnApply()
        {
            canceled = false;
            Lecture = new Lecture {
                ID = long.Parse(_idView.Text.ToString()),
                Theme = _themeView.Text.ToString(),
                Course = new Course { ID = long.Parse(_courseIDView.Text.ToString())},
                IsImported = _importedBox.Checked,
            };
            Application.RequestStop();
        }

        public void SetLecture(Lecture l)
        {
            _idView.Text = l.ID.ToString();
            _themeView.Text = l.Theme;
            _courseIDView.Text = l.Course.ID.ToString();
            _importedBox.Checked = l.IsImported;
        }

        private void OnCourseIDInput(TextChangingEventArgs args)
        {
            foreach (var c in args.NewText)
            {
                if (!char.IsDigit((char) c))
                {
                    args.Cancel = true;
                    return;
                }
            }
        }
    }
}