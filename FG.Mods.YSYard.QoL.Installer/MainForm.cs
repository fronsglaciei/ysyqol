using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FG.Mods.YSYard.QoL.Installer
{
    public partial class MainForm : Form
    {
        private readonly SynchronizationContext _sc;

        private readonly string _userSelectAppPath;

        private CancellationTokenSource _cts;

        public MainForm()
        {
            this._sc = SynchronizationContext.Current;
            InitializeComponent();

            var ii = Installer.GetInstallInfo(out var errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                if (DialogResult.Yes != MessageBox.Show(
                    $"{errorMessage}\n\nゲームのインストール先ディレクトリを手動で設定しますか？",
                    "確認", MessageBoxButtons.YesNo))
                {
                    this.Shown += (_, __) => this.Close();
                    return;
                }

                this.MainFolderBrowser.Description = "ゲームのインストール先ディレクトリを選択";
                this.MainFolderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;
                if (DialogResult.OK != this.MainFolderBrowser.ShowDialog())
                {
                    this.Shown += (_, __) => this.Close();
                    return;
                }
                this._userSelectAppPath = this.MainFolderBrowser.SelectedPath;

                ii = Installer.GetInstallInfo(out errorMessage, this._userSelectAppPath);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    this.Shown += (_, __) => this.Close();
                    MessageBox.Show(errorMessage);
                }
            }
            this.SetVersionLabel(ii);
        }

        private void InstallButton_Click(object sender, EventArgs e)
        {
            var ii = Installer.GetInstallInfo(
                out var errorMessage, this._userSelectAppPath);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }
            if (DialogResult.Yes != MessageBox.Show(
                $"インストール対象のゲームを発見しました。\n\"{ii.AppPath}\"にMOD環境をインストールしてもよろしいですか？",
                "確認", MessageBoxButtons.YesNo))
            {
                return;
            }
            if (!(ii.ModVersion is null))
            {
                if (DialogResult.Yes != MessageBox.Show(
                    "MODは既にインストール済みです。上書きインストールしますか？",
                    "確認", MessageBoxButtons.YesNo))
                {
                    return;
                }
            }
            if (ii.IsYsytransInstalled)
            {
                if (DialogResult.Yes != MessageBox.Show(
                    "非公式日本語翻訳MODとQOL-MODは共存できません。\n非公式日本語翻訳MODをアンインストールしてもよろしいですか？",
                    "確認", MessageBoxButtons.YesNo)
                    || !Installer.UninstallYsytrans(ii.AppPath, this.ShowErrorMessage))
                {
                    return;
                }
            }

            this.DisableButtons();
            
            this._cts?.Cancel();
            this._cts?.Dispose();
            this._cts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                var res = await Installer.InstallAsync(
                    ii.AppPath,
                    this.SetProgress, this.ShowErrorMessage,
                    this._cts.Token)
                    .ConfigureAwait(false);
                this.SetMainMessage(
                    res is null ? "インストールが中断されました" : "インストール成功");
                this.EnableButtons();
                this.SetProgress(0.0);
                if (!(res is null))
                {
                    this.SetVersionLabel(res);
                }
            }, this._cts.Token);
        }

        private void UninstallButton_Click(object sender, EventArgs e)
        {
            var ii = Installer.GetInstallInfo(
                out var errorMessage, this._userSelectAppPath);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }
            if (ii.ModVersion is null)
            {
                MessageBox.Show("アンインストールするMODが見つかりません。");
                return;
            }
            if (DialogResult.Yes != MessageBox.Show(
                "MODをアンインストールしようとしています。よろしいですか？\nヒント: 他のMODを導入していない場合、BepInExフレームワークごと削除します。",
                "確認", MessageBoxButtons.YesNo))
            {
                return;
            }

            this.DisableButtons();

            this._cts?.Cancel();
            this._cts?.Dispose();
            this._cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                var res = Installer.Uninstall(
                    ii.AppPath,
                    this.SetProgress, this.ShowErrorMessage,
                    this._cts.Token);
                this.SetMainMessage(
                    res ? "アンインストール成功" : "アンインストールが中断されました");
                this.EnableButtons();
                this.SetProgress(0.0);
                if (res)
                {
                    this.SetVersionLabel(null);
                }
            }, this._cts.Token);
        }

        private void Do(Action action)
            => this._sc.Post(_ => action?.Invoke(), null);

        private void SetVersionLabel(InstallInfo ii) => this.Do(() =>
        {
            this.VersionLabel.Text =
                ii?.ModVersion is null
                ? "未インストール" : $"インストール済: {ii.ModVersion}";
        });

        private void EnableButtons() => this.Do(() =>
        {
            this.InstallButton.Enabled = true;
            this.UninstallButton.Enabled = true;
        });

        private void DisableButtons() => this.Do(() =>
        {
            this.InstallButton.Enabled = false;
            this.UninstallButton.Enabled = false;
        });

        private void SetProgress(double value) => this.Do(() =>
        {
            if (value < 0.0 || 1.0 < value)
            {
                return;
            }
            var p = this.MainProgressBar;
            p.Value = (int)(value * (p.Maximum - p.Minimum) + p.Minimum);
        });

        private void ShowErrorMessage(Exception ex) => this.Do(() =>
        {
            MessageBox.Show($"{ex}");
        });

        private void SetMainMessage(string message) => this.Do(() =>
        {
            this.MainMessage.Text = $"{message}";
        });
    }
}
