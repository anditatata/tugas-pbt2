using System;
using System.Windows.Forms;
namespace AplikasiWarga.GUI
{
static class Program

{
[STAThread]
static void Main()
{
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
Application.Run(new frmMainMenu());
}
}
}