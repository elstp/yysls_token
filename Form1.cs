using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using CaptureProxy;
using CaptureProxy.MyEventArgs;

namespace yysls_token
{
    public partial class Form1 : Form
    {
        private const string TargetHost = "s3.game.163.com";
        private const string TargetPathPrefix = "/7540694694f2dddc/";
        private const int ProxyPortStart = 8877;
        private const int ProxyPortEnd = 8887;
        private const string MiniProgramLaunchUri = "weixin://launchapplet/?app_id=wx3f78b6f96e5992f4";
        private const string MiniProgramShortcutName = "燕云十六声官方.lnk";

        private static readonly Regex AccessTokenRegex = new(
            @"[?&]access_token=([^&\s#]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private HttpProxy? _proxy;
        private bool _capturing;
        private string? _capturedToken;

        private bool _originalSaved;
        private int _originalProxyEnable;
        private string? _originalProxyServer;
        private string? _originalProxyOverride;
        private bool _systemProxySet;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopProxy();
            base.OnFormClosing(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ClearLeftoverProxy();
            StartCapture();
        }

        private void BtnToggle_Click(object? sender, EventArgs e)
        {
            if (_capturing)
            {
                StopCapture();
            }
            else
            {
                StartCapture();
            }
        }

        private void StartCapture()
        {
            if (!IsWeChatRunning())
            {
                ShowWeChatPrompt();
                return;
            }

            EnsureCertificateInstalled();

            if (!TryStartProxy(out int port, out string error))
            {
                SetStatus("启动失败：" + error, isWarn: true);
                return;
            }

            _capturing = true;
            _capturedToken = null;

            txtToken.Visible = false;
            btnCopy.Visible = false;
            btnToggle.Text = "停止捕获";

            SetStatus(
                $"正在监听 127.0.0.1:{port}，系统代理已自动设置。\r\n" +
                "如果没有自动打开小程序请手动在PC微信中搜索\r\n[燕云十六声官方]小程序\r\n" +
                "打开后即可自动抓取 access_token。");

            SetSystemProxy(port);
            LaunchMiniProgram();
        }

        private void StopCapture()
        {
            StopProxy();
            _capturing = false;
            btnToggle.Text = _capturedToken != null ? "重新捕获" : "开始捕获";
            SetStatus(_capturedToken != null
                ? "已停止捕获，Token 仍保留在下方。"
                : "已停止捕获。");
        }

        private void StopProxy()
        {
            try { _proxy?.Stop(); } catch { /* ignore */ }
            try { _proxy?.Dispose(); } catch { /* ignore */ }
            _proxy = null;
            RestoreSystemProxy();
        }

        private bool TryStartProxy(out int port, out string error)
        {
            for (var p = ProxyPortStart; p <= ProxyPortEnd; p++)
            {
                try
                {
                    var proxy = new HttpProxy(p);
                    proxy.Events.BeforeTunnelEstablish += OnBeforeTunnelEstablish;
                    proxy.Events.BeforeRequest += OnBeforeRequest;
                    proxy.Start();
                    _proxy = proxy;
                    port = p;
                    error = string.Empty;
                    return true;
                }
                catch
                {
                    try { _proxy?.Dispose(); } catch { /* ignore */ }
                    _proxy = null;
                }
            }

            port = 0;
            error = $"端口 {ProxyPortStart}~{ProxyPortEnd} 均被占用，请关闭占用端口的程序后重试。";
            return false;
        }

        private void OnBeforeTunnelEstablish(object? sender, BeforeTunnelEstablishEventArgs e)
        {
            // 只对目标域名开启数据包捕获（解密），其他流量原样转发
            if (string.Equals(e.Host, TargetHost, StringComparison.OrdinalIgnoreCase))
            {
                e.PacketCapture = true;
            }
        }

        private void OnBeforeRequest(object? sender, BeforeRequestEventArgs e)
        {
            try
            {
                var uri = e.Request.Uri;
                if (!string.Equals(uri.Host, TargetHost, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                if (!uri.AbsolutePath.StartsWith(TargetPathPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                var match = AccessTokenRegex.Match(uri.Query);
                if (!match.Success)
                {
                    return;
                }

                var token = Uri.UnescapeDataString(match.Groups[1].Value);
                if (string.IsNullOrWhiteSpace(token))
                {
                    return;
                }

                if (IsDisposed || !IsHandleCreated)
                {
                    return;
                }

                BeginInvoke(new Action(() => HandleTokenCaptured(token)));
            }
            catch
            {
                // 忽略单个请求的解析异常，不中断捕获
            }
        }

        private void HandleTokenCaptured(string token)
        {
            _capturedToken = token;
            _capturing = false;

            txtToken.Text = token;
            txtToken.Visible = true;
            btnCopy.Visible = true;
            btnToggle.Text = "重新捕获";

            SetStatus(
                $"已捕获 access_token！来源：{TargetHost}\r\n" +
                "捕获已停止，系统代理已自动还原。\r\n" +
                "点击「复制 Token」即可使用，也可手动选中文本复制。");

            StopProxy();
        }

        private void BtnCopy_Click(object? sender, EventArgs e) => CopyToken();

        private void TxtToken_MouseClick(object? sender, MouseEventArgs e) => CopyToken();

        private void CopyToken()
        {
            if (string.IsNullOrEmpty(_capturedToken))
            {
                return;
            }

            try
            {
                Clipboard.SetText(_capturedToken);
                SetStatus("已复制到剪贴板，可直接粘贴使用。");
            }
            catch
            {
                SetStatus("复制失败，请手动选中 Token 文本后按 Ctrl+C 复制。", isWarn: true);
            }
        }

        private static bool IsWeChatRunning()
        {
            foreach (var name in new[] { "WeChat", "Weixin" })
            {
                try
                {
                    if (Process.GetProcessesByName(name).Length > 0)
                    {
                        return true;
                    }
                }
                catch
                {
                    // 忽略单个进程名查询异常
                }
            }

            return false;
        }

        private void LaunchMiniProgram()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                };

                // 优先使用桌面快捷方式（与双击图标完全一致），缺失时回退到协议直启
                var shortcut = FindMiniProgramShortcut();
                if (shortcut != null)
                {
                    startInfo.FileName = shortcut;
                }
                else
                {
                    startInfo.FileName = MiniProgramLaunchUri;
                }

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                SetStatus("拉起微信小程序失败：" + ex.Message, isWarn: true);
            }
        }

        private static string? FindMiniProgramShortcut()
        {
            try
            {
                var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var path = Path.Combine(desktop, MiniProgramShortcutName);
                return File.Exists(path) ? path : null;
            }
            catch
            {
                return null;
            }
        }

        private void ShowWeChatPrompt()
        {
            btnToggle.Text = "重试检测微信";
            SetStatus(
                "未检测到 PC 版微信进程。\r\n" +
                "请先启动并登录微信，然后点击「重试检测微信」。",
                isWarn: true);
        }

        private void SaveOriginalProxyState()
        {
            if (_originalSaved)
            {
                return;
            }

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(InternetSettingsKey);
                if (key != null)
                {
                    _originalProxyEnable = (int)(key.GetValue("ProxyEnable", 0) ?? 0);
                    _originalProxyServer = key.GetValue("ProxyServer") as string;
                    _originalProxyOverride = key.GetValue("ProxyOverride") as string;
                }
            }
            catch
            {
                // 读取失败时按无代理处理
            }

            _originalSaved = true;
        }

        private void SetSystemProxy(int port)
        {
            SaveOriginalProxyState();

            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(InternetSettingsKey);
                if (key == null)
                {
                    return;
                }

                key.SetValue("ProxyEnable", 1, RegistryValueKind.DWord);
                key.SetValue("ProxyServer", $"127.0.0.1:{port}", RegistryValueKind.String);
                key.SetValue("ProxyOverride", "<local>", RegistryValueKind.String);
                _systemProxySet = true;
                RefreshSystemProxy();
            }
            catch (Exception ex)
            {
                SetStatus("设置系统代理失败：" + ex.Message, isWarn: true);
            }
        }

        private void RestoreSystemProxy()
        {
            if (!_systemProxySet)
            {
                return;
            }

            _systemProxySet = false;

            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(InternetSettingsKey);
                if (key == null)
                {
                    return;
                }

                key.SetValue("ProxyEnable", _originalProxyEnable, RegistryValueKind.DWord);
                if (!string.IsNullOrEmpty(_originalProxyServer))
                {
                    key.SetValue("ProxyServer", _originalProxyServer, RegistryValueKind.String);
                }
                else
                {
                    key.DeleteValue("ProxyServer", false);
                }

                if (!string.IsNullOrEmpty(_originalProxyOverride))
                {
                    key.SetValue("ProxyOverride", _originalProxyOverride, RegistryValueKind.String);
                }
                else
                {
                    key.DeleteValue("ProxyOverride", false);
                }

                RefreshSystemProxy();
            }
            catch
            {
                // 还原失败时忽略，下次启动会再清理
            }
        }

        private void ClearLeftoverProxy()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(InternetSettingsKey, true);
                if (key == null)
                {
                    return;
                }

                var enable = (int)(key.GetValue("ProxyEnable", 0) ?? 0);
                var server = key.GetValue("ProxyServer") as string;

                // 只清除本程序留下的代理（指向 127.0.0.1 的），不碰用户原有代理
                if (enable == 1 && server != null &&
                    server.Contains("127.0.0.1:", StringComparison.OrdinalIgnoreCase))
                {
                    key.SetValue("ProxyEnable", 0, RegistryValueKind.DWord);
                    key.DeleteValue("ProxyServer", false);
                    key.DeleteValue("ProxyOverride", false);
                    RefreshSystemProxy();
                }
            }
            catch
            {
                // 清理失败不影响程序启动
            }
        }

        private static void RefreshSystemProxy()
        {
            const int InternetOptionSettingsChanged = 39;
            const int InternetOptionRefresh = 37;

            InternetSetOption(IntPtr.Zero, InternetOptionSettingsChanged, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, InternetOptionRefresh, IntPtr.Zero, 0);
        }

        private const string InternetSettingsKey =
            @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(
            IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        private void EnsureCertificateInstalled()
        {
            try
            {
                var hasCert = CertMaker.GetCertsByCommonName(CertMaker.CommonName).Any();
                if (!hasCert)
                {
                    var choice = MessageBox.Show(
                        this,
                        "首次捕获 HTTPS 请求需要安装本地证书（CaptureProxy CA），" +
                        "用于解密 s3.game.163.com 的 HTTPS 流量以读取 access_token。\r\n\r\n" +
                        "证书仅安装到当前用户（无需管理员权限），可随时在系统证书管理器中卸载。是否继续？",
                        "安装本地证书",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);
                    if (choice != DialogResult.Yes)
                    {
                        SetStatus("未安装证书，将无法解密 HTTPS 请求，只能捕获明文 HTTP 请求。", isWarn: true);
                        return;
                    }
                }

                CertMaker.RemoveCertsByCommonName(CertMaker.CommonName);
                if (CertMaker.InstallCert(CertMaker.CaCert))
                {
                    SetStatus("已安装本地证书（CaptureProxy CA）。");
                }
                else
                {
                    SetStatus("证书安装失败，HTTPS 请求可能无法解密。", isWarn: true);
                }
            }
            catch (Exception ex)
            {
                SetStatus("证书处理出错：" + ex.Message, isWarn: true);
            }
        }

        private void SetStatus(string text, bool isWarn = false)
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                try { BeginInvoke(new Action(() => SetStatus(text, isWarn))); } catch { /* ignore */ }
                return;
            }

            lblStatus.Text = text;
            lblStatus.ForeColor = isWarn
                ? Color.FromArgb(238, 120, 120)
                : Color.FromArgb(184, 190, 201);
        }
    }
}
