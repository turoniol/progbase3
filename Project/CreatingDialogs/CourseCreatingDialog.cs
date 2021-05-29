using Terminal.Gui;
using EntitiesProcessingLib.Entities;

namespace UserInterface
{
    public class CourseCreatingDialog : Dialog
    {
        public Course course { get; private set; }

        public bool canceled;

        private TextField _courseTitleField;

        private TextField _authorIDField;

        public CourseCreatingDialog() : base("New course")
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
            Label titleLbl = new Label(2, 1 * yShift, "Title:");
            Label authorLbl = new Label(2, 2 * yShift, "Author ID:");
            this.Add(titleLbl);
            this.Add(authorLbl);

            // text fields
            _courseTitleField = new TextField(14, 1 * yShift, 30, "");
            _authorIDField = new TextField(14, 2 * yShift, 30, "");
            _authorIDField.TextChanging += OnAuthorIDInput;
            this.Add(_courseTitleField);
            this.Add(_authorIDField);
        }

        private void OnCancel()
        {
            canceled = true;
            Application.RequestStop();
        }

        private void OnApply()
        {
            canceled = false;
            course = new Course() 
            {
                Title = _courseTitleField.Text.ToString(),
                Author = new User {ID = long.Parse(_authorIDField.Text.ToString())},
            };
            Application.RequestStop();
        }

        private void OnAuthorIDInput(TextChangingEventArgs args)
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