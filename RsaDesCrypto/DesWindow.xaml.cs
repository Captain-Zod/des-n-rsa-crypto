using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RsaDesCrypto;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class DesWindow : Window
{
    public DesWindow()
    {
        InitializeComponent();
    }

    public FileInfo FromEncryptInfo { get; set; }
    public FileInfo ToEncryptInfo { get; set; }

    private void FromEncBtn_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();

        var result = dlg.ShowDialog();
        if (result == true)
        {
            FromEncrypt.Text = dlg.FileName;
            FromEncryptInfo = new()
            {
                FullPath = dlg.FileName,
                Name = dlg.SafeFileName
            };
        }
    }

    private void ToEncBtn_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog dlg = new SaveFileDialog();
        dlg.FileName = (FromEncryptInfo?.Name ?? "FileName") + ".rsa";
        dlg.DefaultExt = ".rsa";
        dlg.Filter = "RSA encrypted files (.rsa)|*.rsa";

        var result = dlg.ShowDialog();
        if (result == true)
        {
            ToEncrypt.Text = dlg.FileName;
            ToEncryptInfo = new()
            {
                FullPath = dlg.FileName,
                Name = dlg.SafeFileName
            };
        }
    }

    private void EncryptButton_Click(object sender, RoutedEventArgs e)
    {
        var keyiv = PubKey.Text.Trim().Split(System.Environment.NewLine);
        var (key, iv) = (Convert.FromBase64String(keyiv[0]), Convert.FromBase64String(keyiv[1]));
        var data = File.ReadAllBytes(FromEncryptInfo.FullPath);
        Crypto.Des.Encrypt(data, ToEncryptInfo.FullPath, key, iv);
    }

    public FileInfo FromDecryptInfo { get; set; }
    public FileInfo ToDecryptInfo { get; set; }
    private void FromDecBtn_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();

        var result = dlg.ShowDialog();
        if (result == true)
        {
            FromDecrypt.Text = dlg.FileName;
            FromDecryptInfo = new()
            {
                FullPath = dlg.FileName,
                Name = dlg.SafeFileName
            };
        }
    }

    private void ToDecBtn_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog dlg = new SaveFileDialog();
        dlg.FileName = FromDecryptInfo?.Name.Replace(".rsa", "") ?? "FileName";

        var result = dlg.ShowDialog();
        if (result == true)
        {
            ToDecrypt.Text = dlg.FileName;
            ToDecryptInfo = new()
            {
                FullPath = dlg.FileName,
                Name = dlg.SafeFileName
            };
        }
    }

    private void DecryptButton_Click(object sender, RoutedEventArgs e)
    {
        var keyiv = PrivKey.Text.Trim().Split("\n");
        var (key, iv) = (Convert.FromBase64String(keyiv[0]), Convert.FromBase64String(keyiv[1]));
        var enc = Crypto.Des.Decrypt(FromDecryptInfo.FullPath, key, iv);
        File.WriteAllBytes(ToDecryptInfo.FullPath, enc);
    }

    private void GeneratePathBtn_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(GeneratePath.Text);
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        var keys = Crypto.Des.GenerateKey();
        GeneratePath.Text = keys;
    }
}
