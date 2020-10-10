using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;

namespace Megah_Motor_Label_5_Kolom
{
  public partial class frmMain : Form
  {
    private System.Windows.Forms.Button printButton;
    private Font printFont;
    private StreamReader streamToPrint;
    public frmMain()
    {
      InitializeComponent();
    }

    private String angka_cina(int angka)
    {
      String[] digits = { "冬", "元", "月", "东", "西", "南", "北", "車", "來", "財" };
      Char[] charArray = angka.ToString().ToCharArray();
      String result = "";

      for (int i = 0; i < charArray.Length; i++)
      {
        Char ch = charArray[i];
        result += digits[(int)Char.GetNumericValue(ch)];
      }
      return result;
    }

    private void txtAsalBarang_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Down)
      {
        txtNamaBarang.Focus();
      }
    }

    private void txtNamaBarang_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Up)
      {
        txtAsalBarang.Focus();
      }
      if (e.KeyCode == Keys.Escape)
      {
        txtAsalBarang.Text = "";
        txtAsalBarang.Focus();
      }
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Down)
      {
        txtTipeMobil.Focus();
      }
    }

    private void txtTipeMobil_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Up)
      {
        txtNamaBarang.Focus();
      }
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Down)
      {
        txtKodeJual.Focus();
      }
    }

    private void txtKodeMandarin_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Up)
      {
        txtKodeJual.Focus();
      }
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Down)
      {
        var input = 0;
        if (int.TryParse(txtKodeMandarin.Text, out input))
        {
          txtJumlahCetak.Focus();
        }
        else
        {
          MessageBox.Show("Kode mandarin harus berupa angka!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
      }
    }

    private void txtKodeJual_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Up)
      {
        txtTipeMobil.Focus();
      }
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Down)
      {
        txtKodeMandarin.Focus();
      }
    }

    private void numJumlahCetak_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
      {
        btnCetak.Focus();
      }
    }

    private void btnCetak_Click(object sender, EventArgs e)
    {
      DialogResult result = MessageBox.Show("Apakah anda yakin untuk mencetak?", "Peringatan", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
      if (result == DialogResult.Yes)
      {
        var input = 0;
        if (int.TryParse(txtJumlahCetak.Text, out input))
        {
          var inp = 0;
          if (int.TryParse(txtKodeMandarin.Text, out inp))
          {
            cetak_data();
          }
          else
          {
            MessageBox.Show("Kode mandarin harus berupa angka!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          }
        }
        else
        {
          MessageBox.Show("Jumlah cetak harus berupa angka!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
      }
      reset_data();
    }

    private string toStringLimit(String str)
    {
      int all = 20;
      int lenght = str.Length;
      if (lenght >= 20)
      {
        return str.Substring(0, 10);
      }
      else
      {
        int rest = all - lenght;
        for (int i = 0; i < rest; i++)
        {
          str += " ";
        }
        return str;
      }
    }

    private string toMandarinLimit(String str)
    {
      int all = 10;
      int lenght = str.Length;
      if (lenght >= 10)
      {
        return str.Substring(0, 9);
      }
      else
      {
        int rest = all - lenght;
        for (int i = 0; i < rest; i++)
        {
          str += "  ";
        }
        return str;
      }
    }

    private void cetak_data()
    {
      String asalBarang = txtAsalBarang.Text;
      String namaBarang = txtNamaBarang.Text;
      String tipeMobil = txtTipeMobil.Text;
      String kodeJual = txtKodeJual.Text;
      String hurufMandarin = angka_cina(int.Parse(txtKodeMandarin.Text));

      String lineAsalBarang = " ";
      String lineNamaBarang = " ";
      String lineTipeMobil = " ";
      String lineKodeJual = " ";
      String lineHurufMandarin = " ";

      for (int i = 0; i < 5; i++)
      {
        lineAsalBarang += toStringLimit(asalBarang);
        lineNamaBarang += toStringLimit(namaBarang);
        lineKodeJual += toStringLimit(kodeJual);
        lineTipeMobil += toStringLimit(tipeMobil);
        lineHurufMandarin += toMandarinLimit(hurufMandarin);
      }
      String str = "";
      String batch = lineAsalBarang + "\n" + lineNamaBarang + "\n" + lineKodeJual + "\n" + lineTipeMobil + "\n" + lineHurufMandarin;
      int jumlahCetak = int.Parse(txtJumlahCetak.Text);
      for (int i = 0; i < jumlahCetak; i++)
      {
        str += batch + "\n\n";
      }

      PrintDocument p = new PrintDocument();
      p.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom", 827, jumlahCetak * 200);
      Margins margins = new Margins(200, 200, 200, 200);
      p.DefaultPageSettings.Margins = margins;
      //p.DefaultPageSettings.PaperSize = new PaperSize("Label 5", 210, 50);
      p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
      {
        e1.Graphics.DrawString(str, new Font("Consolas", 10), new SolidBrush(Color.Black), new RectangleF(0, 0, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));
      };
      try
      {
        p.Print();
      }
      catch (Exception ex)
      {
        throw new Exception("Exception Occured While Printing", ex);
      }
    }

    private void printfunction(string cmd)
    {
      string command = cmd;

      // Create a buffer with the command
      Byte[] buffer = new byte[command.Length];
      buffer = System.Text.Encoding.ASCII.GetBytes(command);

      // Use the CreateFile external functo connect to the LPT1 port
      SafeFileHandle printer = CreateFile("LPT1:", FileAccess.ReadWrite, 0, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

      // Aqui verifico se a impressora é válida
      if (printer.IsInvalid == true)
      {
        MessageBox.Show("Printer not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // Open the filestream to the lpt1 port and send the command
      FileStream lpt1 = new FileStream(printer, FileAccess.ReadWrite);
      lpt1.Write(buffer, 0, buffer.Length);

      // Close the FileStream connection
      lpt1.Close();
    }

    private SafeFileHandle CreateFile(string v1, FileAccess readWrite, int v2, IntPtr zero1, FileMode open, int v3, IntPtr zero2)
    {
      throw new NotImplementedException();
    }

    public void Printing()
    {
       string s = "	ESC k 1 Hello world\n"; // device-dependent string, need a FormFeed?

      // Allow the user to select a printer.
      PrintDialog pd = new PrintDialog();
      pd.PrinterSettings = new PrinterSettings();
      if (DialogResult.OK == pd.ShowDialog(this))
      {
        // Send a printer-specific to the printer.
        RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, s);
      }
    }

    private void btnSetting_Click(object sender, EventArgs e)
    {
      string text = System.IO.File.ReadAllText("./settings.txt");
      string input = Interaction.InputBox("Masukkan nama printer", "Setting Printer", text);
      string dirParameter = AppDomain.CurrentDomain.BaseDirectory + @"\settings.txt";
      FileStream fParameter = new FileStream(dirParameter, FileMode.Create, FileAccess.Write);
      StreamWriter m_WriterParameter = new StreamWriter(fParameter);
      m_WriterParameter.BaseStream.Seek(0, SeekOrigin.End);
      m_WriterParameter.Write(input);
      m_WriterParameter.Flush();
      m_WriterParameter.Close();
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
      reset_data();
    }

    private void reset_data()
    {
      txtNamaBarang.Text = "";
      txtTipeMobil.Text = "";
      txtKodeJual.Text = "";
      txtKodeMandarin.Text = "";
      txtJumlahCetak.Text = "";
      txtNamaBarang.Focus();
    }

    private void frmMain_Shown(object sender, EventArgs e)
    {
      txtAsalBarang.Focus();
    }

    private void txtJumlahCetak_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter)
      {
        var input = 0;
        if (int.TryParse(txtJumlahCetak.Text, out input))
        {
          btnCetak.Focus();
        }
        else
        {
          txtJumlahCetak.Text = "";
          MessageBox.Show("Jumlah cetak harus berupa angka!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
      }
      if (e.KeyCode == Keys.Up)
      {
        txtKodeMandarin.Focus();
      }
    }
  }
}
