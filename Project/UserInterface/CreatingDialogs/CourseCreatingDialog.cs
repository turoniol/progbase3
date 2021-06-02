using Terminal.Gui;
using EntitiesProcessingLib.Entities;

namespace UserInterface
{
    public class CourseCreatingDialog : Dialog
    {
        public Course course { get; private set; }
        private User _loginedUser;

        public bool canceled;

        private TextField _courseTitleField;

        public CourseCreatingDialog(User user) : base("New course")
        {
            _loginedUser = user;

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
            this.Add(titleLbl);

            // text fields
            _courseTitleField = new TextField(14, 1 * yShift, 30, "");
            this.Add(_courseTitleField);
        }

        private void OnCancel()
        {
            canceled = true;
            Application.RequestStop();
        }

        private void OnApply()
        {
            if (_courseTitleField.Text.IsEmpty)
            {
                MessageBox.ErrorQuery("Error", "The field is empty!", "Ok");
            }

            canceled = false;
            course = new Course() 
            {
                Title = _courseTitleField.Text.ToString(),
                Author = new User {ID = _loginedUser.ID},
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