using System.IO.Compression;

namespace DotnetAppCompressor;

public partial class Form1 : Form
{
    private ProgressBar progressBar;
    public Form1()
    {
        InitializeComponent();
        TextBox textBox = new TextBox
        {
            AllowDrop = true,
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            PlaceholderText = "Drop a folder"
        };

        progressBar = new ProgressBar
        {
            Dock = DockStyle.Bottom,
            Minimum = 0,
            Maximum = 100,
            Value = 0
        };


        textBox.DragEnter += TextBox_DragEnter;
        textBox.DragDrop += TextBox_DragDrop;
        Controls.Add(textBox);
        Controls.Add(progressBar);
    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }

    private void TextBox_DragEnter(object sender, DragEventArgs e)
    {

        // Check if the data contains folder paths
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (paths.All(path => Directory.Exists(path)))
            {
                e.Effect = DragDropEffects.Copy; // Allow dropping
            }
            else
            {
                e.Effect = DragDropEffects.None; // Disallow
            }
        }
    }

    private async void TextBox_DragDrop(object sender, DragEventArgs e)
    {
        try
        {
            // Get the dropped folder paths
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                string[] foldersToBeDeleted = ["bin", "obj"];

                progressBar.Value = 0;
                int totalSteps = paths.Length;
                int currentStep = 0;

                foreach (var path in paths)
                {
                    await Task.Run(() => DeleteFoldersAndCompress(path, foldersToBeDeleted));
                    currentStep++;
                    UpdateProgress(currentStep, totalSteps);
                }
                MessageBox.Show("Successfully compressed..");
                // Display folder paths in the TextBox
                TextBox textBox = sender as TextBox;
                textBox.Text = ""; // Clear old content
                textBox.Text = string.Join(Environment.NewLine, paths);
            }
        }
        catch(Exception ex)
        {
            MessageBox.Show("Error is occured");
        }
    }

    private void DeleteFoldersAndCompress(string filePath, string[] foldersToBeDeleted)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentNullException(filePath);
        }
        foreach (string folder in foldersToBeDeleted)
        {
            if (Directory.Exists(Path.Combine(filePath, folder)))
            {
                Directory.Delete(Path.Combine(filePath, folder), true);
            }
        }
        var zipFileName = $"{Path.GetFileName(filePath)}_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
        string zipPath = Path.Combine(Path.GetDirectoryName(filePath), zipFileName);
        ZipFile.CreateFromDirectory(filePath, zipPath);
    }

    private void UpdateProgress(int currentStep, int totalSteps)
    {
        if (totalSteps > 0)
        {
            int progress = (currentStep * 100) / totalSteps;
            progressBar.Invoke((Action)(() => progressBar.Value = progress));
        }
    }
}
