using Terminal.Gui;
using EntitiesProcessingLib.Entities;

namespace UserInterface
{
    public class LectureCreatingDialog : Dialog
    {
        public Lecture lecture { get; private set; }

        public bool canceled;

        private TextField _lectureTitleField;

        private TextField _courseIDField;

        public LectureCreatingDialog() : base("New lecture")
        {
            int yShift = 2;
            this.Height = 10;
            this.Width = 50;
            // buttons
            Button cancelBtn = new Button("Cancel");
            Button okBtn = new Button("Ok");
            cancelBtn.Clicked += OnCancel;
            okBtn.Clicked += OnApply;
            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            // labels
            Label themeLbl = new Label(2, 1 * yShift, "Theme:");
            Label courseLbl = new Label(2, 2 * yShift, "Course ID:");
            this.Add(themeLbl);
            this.Add(courseLbl);

            // text fields
            _lectureTitleField = new TextField(14, 1 * yShift, 30, "");
            _courseIDField = new TextField(14, 2 * yShift, 30, "");
            _courseIDField.TextChanging += OnCourseIDInput;
            this.Add(_lectureTitleField);
            this.Add(_courseIDField);
        }

        private void OnCancel()
        {
            canceled = true;
            Application.RequestStop();
        }

        private void OnApply()
        {
            canceled = false;
            if (_lectureTitleField.Text.IsEmpty || _courseIDField.Text.IsEmpty)
            {
                MessageBox.ErrorQuery("Error", "Some field is empty!", "Ok");
                return;
            }

            lecture = new Lecture() 
            {
                Theme = _lectureTitleField.Text.ToString(),
                Course = new Course {ID = long.Parse(_courseIDField.Text.ToString())},
            };
            Application.RequestStop();
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