using Terminal.Gui;

namespace UserInterface
{
    public class ReportDialog : Dialog
    {
        public bool canceled;

        private TextField _word;

        private TextField _path;

        private TextField _fileName;

        public string FilePath { get; private set; }

        public string Word { get; private set; }

        public ReportDialog() : base("Export")
        {
            // geometry
            int yShift = 2;
            this.Width = 60;
            this.Height = 10;

            Label wordLabel = new Label(1, 1 * yShift, "Enter a word:");
            Label pathLbl = new Label(2, 2 * yShift, "Path:");
            Label fileLbl = new Label(3, 3 * yShift, "File:");
            _word = new TextField(15, 1 * yShift, 42, "");
            _path = new TextField(15, 2 * yShift, 30, "") {ReadOnly = true};
            _fileName = new TextField(15, 3 * yShift, 20, "export.xml");
            _path.Text = System.IO.Directory.GetCurrentDirectory();
            Button choosePath = new Button(47, 2 * yShift, "Choose") { Width = 10, };
            choosePath.Clicked += OnChoose;

            this.Add(wordLabel, pathLbl, fileLbl, _word, _path, _fileName, choosePath);

            Button okBtn = new Button("Ok");
            Button cancelBtn = new Button("Cancel");
            okBtn.Clicked += OnApply;
            cancelBtn.Clicked += OnCancel;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);
        }

        public void OnApply()
        {
            Word = _word.Text.ToString();
            string file = _fileName.Text.ToString();
            file = file == "" ? "report.docx" : file;
            FilePath = _path.Text.ToString() + "/" + file;
            canceled = false;
            Application.RequestStop();
        }

        public void OnChoose()
        {
            OpenDialog dlg = new OpenDialog("Choose a directory", "") {CanChooseFiles = false};
            Application.Run(dlg);
            if (dlg.Canceled) return;

            _path.Text = dlg.FilePath;
            _path.CursorPosition =_path.Text.Length;
        }

        public void OnCancel()
        {
            canceled = true;
            Application.RequestStop();
        }
    }
}