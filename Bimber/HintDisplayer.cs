using Bimber;
using System.Windows.Forms;

public class HintDisplayer
{
    private readonly int _displayTime;

    public HintDisplayer(int displayTimeMilliseconds = 3000)
    {
        _displayTime = displayTimeMilliseconds;
    }

    public void ShowHint(string message)
    {
        var hintForm = new HintForm(message);
        hintForm.Show();

        var timer = new System.Windows.Forms.Timer 
        {
            Interval = _displayTime
        };

        timer.Tick += (sender, e) => CloseHint(hintForm, timer);
        timer.Start();
    }

    private void CloseHint(Form hintForm, System.Windows.Forms.Timer timer)
    {
        timer.Stop();
        timer.Dispose();
        hintForm.Close();
    }
}