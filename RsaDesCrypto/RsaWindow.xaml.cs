using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace RsaDesCrypto;
/// <summary>
/// Interaction logic for RsaWindow.xaml
/// </summary>
public partial class RsaWindow : Window
{
    public RsaWindow()
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
        var pk = File.ReadAllText(Path.Combine(Path.GetDirectoryName(ToEncryptInfo.FullPath), PubKey.Text));
        var data = File.ReadAllBytes(FromEncryptInfo.FullPath);
        var enc = Crypto.RSA.Encrypt(data, pk);
        File.WriteAllBytes(ToEncryptInfo.FullPath, enc);
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
        var pk = String.IsNullOrEmpty(PrivKey.Text.Trim()) ? File.ReadAllText("Private key") : File.ReadAllText(PrivKey.Text);
        var data = File.ReadAllBytes(FromDecryptInfo.FullPath);
        var enc = Crypto.RSA.Decrypt(data, pk);
        File.WriteAllBytes(ToDecryptInfo.FullPath, enc);
    }

    private void GeneratePathBtn_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog dlg = new SaveFileDialog();
        dlg.FileName = "Public key";

        var result = dlg.ShowDialog();
        if (result == true)
        {
            GeneratePath.Text = dlg.FileName;
        }
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        var keys = Crypto.RSA.GenerateKeys(2048);
        File.WriteAllText(GeneratePath.Text, keys.pub);
        File.WriteAllText("Private key", keys.priv);
    }

    private void PubKey_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        e.Handled = true;
    }

    private void PrivKey_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        e.Handled = true;
    }
}
