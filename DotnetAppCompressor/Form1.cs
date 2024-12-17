namespace DotnetAppCompressor;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        TextBox textBox = new TextBox
        {
            AllowDrop = true,
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            PlaceholderText= "Drop a folder"
        };

        textBox.DragEnter += TextBox_DragEnter;
        textBox.DragDrop += TextBox_DragDrop;
        Controls.Add(textBox);
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

    private void TextBox_DragDrop(object sender, DragEventArgs e)
    {
        // Get the dropped folder paths
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Display folder paths in the TextBox
            TextBox textBox = sender as TextBox;
            textBox.Text = ""; // Clear old content
            textBox.Text = string.Join(Environment.NewLine, paths);
        }
    }
}
