using System.Reflection;

namespace ClickOnceShortcutReplacer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = Assembly.GetExecutingAssembly().Location;
            label2.Text = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        }

    }
}