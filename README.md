燕云Token获取小工具 (yysls-token)
🚀 一款专为《燕云十六声》PC端微信小程序设计的 access_token 自动捕获工具。无需抓包复杂操作，一键启动，自动完成系统代理设置与Token提取。

📖 项目简介
本工具是一个基于 .NET 8 开发的 Windows 桌面小工具。它能智能检测 PC 微信进程，自动设置系统代理，并监听 s3.game.163.com 域名的特定 API 请求，从中提取出用于登录或验证的 access_token。捕获到 Token 后，工具会自动停止代理并还原系统网络设置，安全便捷。

✨ 核心功能
✅ 微信进程智能检测：启动前自动检查 PC 版微信是否运行，若未运行则友好提示。

✅ 一键开启捕获：点击“开始捕获”按钮，自动完成系统代理设置、拉起微信小程序。

✅ HTTPS 流量解密：自动安装/检查 CaptureProxy 根证书，实现对目标域名的 HTTPS 请求解析。

✅ 精准匹配目标：专注于拦截 s3.game.163.com 并匹配 /7540694694f2dddc/ 路径中的 access_token 参数。

✅ 自动还原系统配置：捕获成功或点击“停止捕获”后，自动清理并恢复用户的原始代理设置，不留痕迹。

✅ 一键复制 Token：捕获到的 Token 显示在文本框中，支持点击复制或手动选中复制。

🛠️ 使用方法
下载并运行：在 Releases 页面 下载最新版本的 exe 文件，双击运行。

确保微信已登录：请确保您的 PC 版微信已启动并登录。

开始捕获：

点击主界面 “开始捕获” 按钮。

如果未安装根证书，程序将弹出提示框，点击 “是” 即可完成安装。

拉起小程序：

工具将自动拉取 PC 微信中的 “燕云十六声官方” 小程序（若桌面有快捷方式则优先使用）。

若未自动拉起，请手动在微信搜索框输入 燕云十六声官方 并进入小程序。

获取 Token：

进入小程序正常操作即可。一旦请求命中，底部状态栏会提示“已捕获 access_token”，Token 文本将显示在输入框中。

点击 “复制 Token” 按钮，即可将其粘贴到其他需要授权的地方。

停止捕获：点击主界面 “停止捕获” 或 “重新捕获” 按钮，即可关闭代理并恢复网络。

⚙️ 工作原理
代理设置：工具通过修改 Windows 注册表 (HKCU\Software\Microsoft\Windows\CurrentVersion\Internet Settings) 将系统代理设置为本地监听端口 (127.0.0.1:8877~8887)。

流量劫持：使用 CaptureProxy 库启动本地 HTTP(S) 代理服务器。通过 BeforeTunnelEstablish 事件，仅对目标域名 s3.game.163.com 开启流量解密。

正则匹配：在 BeforeRequest 事件中，筛选请求 URI，通过正则表达式 [?&]access_token=([^&\s#]+) 从 URL 查询字符串中提取 Token。

自动清理：捕获完成后，工具将注册表中的代理设置还原为启动前的原始状态，确保不影响用户日常上网。

🔧 技术栈与配置
框架：.NET 8 (Windows Forms)

运行环境：Windows 10 / 11 (x64)

依赖库：CaptureProxy (用于实现 HTTP 代理与 TLS 解密)

🏗️ 开发与构建
如果您想从源码编译本项目，请按照以下步骤操作：

安装 .NET SDK 8.0：请确保您的开发环境已安装 .NET 8 SDK。

克隆仓库：

bash
git clone https://github.com/您的用户名/yysls-token.git
cd yysls-token
还原依赖：

bash
dotnet restore
编译发布：

bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
编译后的单文件可执行程序位于 bin\Release\net8.0-windows\win-x64\publish\ 目录下。

⚠️ 注意事项
安全提示：本工具仅用于提取与游戏小程序交互的 access_token，不涉及 账号密码泄露或隐私收集。所有流量解密仅在本地进行。

证书信任：首次运行安装的 CaptureProxy CA 证书仅限于当前用户，您可以在系统设置（certmgr.msc -> 受信任的根证书颁发机构）中随时卸载。

网络冲突：若您在使用 VPN、加速器或其他代理软件，可能会导致代理设置冲突。建议在捕获结束后点击“停止捕获”以恢复网络。

权限问题：修改系统代理和安装证书需要管理权限吗？不需要。程序使用当前用户注册表设置，无需 UAC 提升。

📄 开源协议
本项目遵循 MIT 协议 开源。您可以自由使用、修改和分发。
