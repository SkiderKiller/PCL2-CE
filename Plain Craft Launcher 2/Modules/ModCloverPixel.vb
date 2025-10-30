Imports System.IO.Compression
Imports System.Net.Http
Imports PCL.Core.IO
Imports PCL.Core.Minecraft
Imports PCL.Core.Utils

''' <summary>
''' CloverPixel Launcher 专用模块
''' </summary>
Public Module ModCloverPixel

    ''' <summary>
    ''' CloverPixel 隐藏 Mod 文件名
    ''' </summary>
    Private Const CloverPixelModFileName As String = "cloverpixel-2.0.0.jar"
    
    ''' <summary>
    ''' CloverPixel 隐藏 Mod 标记文件名
    ''' </summary>
    Private Const CloverPixelModMarker As String = ".cloverpixel.hidden"
    
    ''' <summary>
    ''' Java 8 下载 URL
    ''' </summary>
    Private Const Java8DownloadUrl As String = "https://builds.openlogic.com/downloadJDK/openlogic-openjdk-jre/8u462-b08/openlogic-openjdk-jre-8u462-b08-windows-x64.zip"
    
    ''' <summary>
    ''' 目标 Minecraft 版本
    ''' </summary>
    Private Const TargetMcVersion As String = "1.8.9"
    
    ''' <summary>
    ''' CloverPixel Mod 源路径（项目根目录）
    ''' </summary>
    Private ReadOnly Property CloverPixelModSourcePath As String
        Get
            ' 首先尝试在可执行文件同目录查找
            Dim sameDir = Path.Combine(ExePath, CloverPixelModFileName)
            If File.Exists(sameDir) Then
                Return sameDir
            End If
            
            ' 在开发环境中，尝试在项目根目录查找
            Dim exeDir = Path.GetDirectoryName(ExePathWithName)
            If exeDir IsNot Nothing Then
                Dim projectRoot = Path.GetFullPath(Path.Combine(exeDir, "..", ".."))
                Dim projectFile = Path.Combine(projectRoot, CloverPixelModFileName)
                If File.Exists(projectFile) Then
                    Return projectFile
                End If
            End If
            
            ' 返回默认路径（即使不存在也返回，由调用者处理）
            Return Path.Combine(ExePath, CloverPixelModFileName)
        End Get
    End Property

    ''' <summary>
    ''' 初始化 CloverPixel Launcher，检查并自动下载所需组件
    ''' </summary>
    Public Sub InitializeCloverPixel()
        RunInNewThread(
            Sub()
                Try
                    Log("[CloverPixel] 开始初始化 CloverPixel Launcher")
                    
                    ' 检查 Java 8
                    If Not CheckJava8() Then
                        Log("[CloverPixel] 未找到 Java 8，开始下载")
                        If DownloadJava8() Then
                            Hint("Java 8 下载完成！", HintType.Finish)
                        Else
                            Hint("Java 8 下载失败，请手动安装", HintType.Critical)
                        End If
                    Else
                        Log("[CloverPixel] Java 8 已存在")
                    End If
                    
                    ' 检查 Minecraft 1.8.9
                    If Not CheckMinecraft189() Then
                        Log("[CloverPixel] 未找到 Minecraft 1.8.9，开始下载")
                        DownloadMinecraft189()
                    Else
                        Log("[CloverPixel] Minecraft 1.8.9 已存在")
                    End If
                    
                    ' 检查 Forge
                    If Not CheckForge189() Then
                        Log("[CloverPixel] 未找到 Forge for 1.8.9，开始安装")
                        InstallForge189()
                    Else
                        Log("[CloverPixel] Forge for 1.8.9 已存在")
                    End If
                    
                    Log("[CloverPixel] CloverPixel Launcher 初始化完成")
                Catch ex As Exception
                    Log(ex, "CloverPixel 初始化失败", LogLevel.Hint)
                End Try
            End Sub, "CloverPixel Initialize")
    End Sub

    ''' <summary>
    ''' 检查是否安装了 Java 8
    ''' </summary>
    Private Function CheckJava8() As Boolean
        Try
            Javas.CheckJavaAvailability()
            For Each javaInfo In Javas.JavaList
                If javaInfo.Version.Major = 1 AndAlso javaInfo.Version.Minor = 8 Then
                    Log($"[CloverPixel] 找到 Java 8: {javaInfo.Path}")
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            Log(ex, "检查 Java 8 失败", LogLevel.Debug)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 下载并安装 Java 8
    ''' </summary>
    Private Function DownloadJava8() As Boolean
        Try
            RunInUi(Sub() Hint("正在下载 Java 8...", HintType.Info))
            
            Dim tempZipPath As String = PathTemp & "java8_" & GetUuid() & ".zip"
            Dim extractPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.minecraft\runtime\clover-java-8\"
            
            ' 创建目标目录
            Directory.CreateDirectory(extractPath)
            
            ' 下载 Java 8
            Using client As New HttpClient()
                client.Timeout = TimeSpan.FromMinutes(10)
                Dim response = client.GetAsync(Java8DownloadUrl).Result
                response.EnsureSuccessStatusCode()
                
                Using fileStream As New FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None)
                    response.Content.CopyToAsync(fileStream).Wait()
                End Using
            End Using
            
            Log($"[CloverPixel] Java 8 下载完成，开始解压到: {extractPath}")
            RunInUi(Sub() Hint("正在解压 Java 8...", HintType.Info))
            
            ' 解压 ZIP 文件
            ZipFile.ExtractToDirectory(tempZipPath, extractPath)
            
            ' 删除临时文件
            File.Delete(tempZipPath)
            
            ' 重新扫描 Java
            Javas.ScanJavaAsync().GetAwaiter().GetResult()
            
            Log("[CloverPixel] Java 8 安装完成")
            Return True
        Catch ex As Exception
            Log(ex, "下载 Java 8 失败", LogLevel.Msgbox)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 检查是否安装了 Minecraft 1.8.9
    ''' </summary>
    Private Function CheckMinecraft189() As Boolean
        Try
            McFolderListLoader.WaitForExit()
            For Each folder In McFolderList
                For Each version In folder.VersionList
                    If version.Name = TargetMcVersion Then
                        Log($"[CloverPixel] 找到 Minecraft {TargetMcVersion}")
                        Return True
                    End If
                Next
            Next
            Return False
        Catch ex As Exception
            Log(ex, "检查 Minecraft 1.8.9 失败", LogLevel.Debug)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 下载 Minecraft 1.8.9
    ''' </summary>
    Private Sub DownloadMinecraft189()
        Try
            RunInUi(Sub() Hint($"正在下载 Minecraft {TargetMcVersion}...", HintType.Info))
            
            ' 找到第一个 MC 文件夹
            If Not McFolderList.Any() Then
                Log("[CloverPixel] 未找到 Minecraft 文件夹", LogLevel.Hint)
                Return
            End If
            
            Dim targetFolder = McFolderList.First()
            Dim versionFolder = targetFolder.Path & "versions\" & TargetMcVersion & "\"
            Directory.CreateDirectory(versionFolder)
            
            ' 使用现有的下载系统下载 MC 1.8.9
            Dim loader = DlClientDownload(TargetMcVersion, targetFolder.Path, True, False)
            loader.Start()
            loader.WaitForExit()
            
            If loader.State = LoadState.Finished Then
                Log($"[CloverPixel] Minecraft {TargetMcVersion} 下载完成")
                RunInUi(Sub() Hint($"Minecraft {TargetMcVersion} 下载完成！", HintType.Finish))
            Else
                Log($"[CloverPixel] Minecraft {TargetMcVersion} 下载失败", LogLevel.Hint)
            End If
        Catch ex As Exception
            Log(ex, $"下载 Minecraft {TargetMcVersion} 失败", LogLevel.Hint)
        End Try
    End Sub

    ''' <summary>
    ''' 检查是否安装了 Forge for 1.8.9
    ''' </summary>
    Private Function CheckForge189() As Boolean
        Try
            McFolderListLoader.WaitForExit()
            For Each folder In McFolderList
                For Each version In folder.VersionList
                    If version.Name.Contains("1.8.9") AndAlso version.Name.ToLower().Contains("forge") Then
                        Log($"[CloverPixel] 找到 Forge for 1.8.9: {version.Name}")
                        Return True
                    End If
                Next
            Next
            Return False
        Catch ex As Exception
            Log(ex, "检查 Forge 失败", LogLevel.Debug)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 安装 Forge for 1.8.9
    ''' </summary>
    Private Sub InstallForge189()
        Try
            RunInUi(Sub() Hint($"正在安装 Forge for {TargetMcVersion}...", HintType.Info))
            
            ' 这里应该触发 Forge 安装流程
            ' 由于需要与现有的 Forge 安装系统集成，这里只是记录日志
            Log($"[CloverPixel] 请在下载页面手动安装 Forge for {TargetMcVersion}", LogLevel.Hint)
            RunInUi(Sub() 
                Hint($"请在下载页面安装 Forge for {TargetMcVersion}", HintType.Info)
            End Sub)
        Catch ex As Exception
            Log(ex, "安装 Forge 失败", LogLevel.Hint)
        End Try
    End Sub

    ''' <summary>
    ''' 在游戏启动前准备 CloverPixel Mod
    ''' </summary>
    Public Sub PrepareCloverPixelMod(mcInstance As McInstance)
        Try
            If mcInstance Is Nothing Then Return
            If Not mcInstance.Version.HasForge Then Return ' 只对 Forge 版本启用
            
            Dim modsDir As String = mcInstance.Path & "mods\"
            Directory.CreateDirectory(modsDir)
            
            Dim targetModPath As String = modsDir & CloverPixelModFileName
            Dim markerPath As String = modsDir & CloverPixelModMarker
            
            ' 检查源文件是否存在
            If Not File.Exists(CloverPixelModSourcePath) Then
                Log($"[CloverPixel] 源 Mod 文件不存在: {CloverPixelModSourcePath}", LogLevel.Debug)
                Return
            End If
            
            ' 复制 CloverPixel Mod 到 mods 目录
            If Not File.Exists(targetModPath) Then
                File.Copy(CloverPixelModSourcePath, targetModPath, True)
                Log($"[CloverPixel] 已复制 CloverPixel Mod 到: {targetModPath}")
            End If
            
            ' 创建隐藏标记文件
            If Not File.Exists(markerPath) Then
                File.WriteAllText(markerPath, "This file marks cloverpixel-2.0.0.jar as a hidden system mod.")
                ' 设置为隐藏文件
                Try
                    File.SetAttributes(markerPath, FileAttributes.Hidden Or FileAttributes.System)
                Catch
                    ' 忽略设置属性失败
                End Try
            End If
            
            ' 设置 Mod 文件为隐藏和只读
            Try
                Dim attr = File.GetAttributes(targetModPath)
                If (attr And FileAttributes.Hidden) <> FileAttributes.Hidden Then
                    File.SetAttributes(targetModPath, attr Or FileAttributes.Hidden Or FileAttributes.System Or FileAttributes.ReadOnly)
                    Log($"[CloverPixel] 已将 CloverPixel Mod 设置为隐藏（只读+系统+隐藏）")
                End If
            Catch ex As Exception
                Log(ex, "设置 CloverPixel Mod 属性失败", LogLevel.Debug)
            End Try
        Catch ex As Exception
            Log(ex, "准备 CloverPixel Mod 失败", LogLevel.Debug)
        End Try
    End Sub

    ''' <summary>
    ''' 在游戏结束后清理 CloverPixel Mod（可选）
    ''' </summary>
    Public Sub CleanupCloverPixelMod(mcInstance As McInstance)
        Try
            ' 可以选择保留 Mod，不需要清理
            ' 如果需要每次清理，可以在这里删除文件
        Catch ex As Exception
            Log(ex, "清理 CloverPixel Mod 失败", LogLevel.Debug)
        End Try
    End Sub

    ''' <summary>
    ''' 检查文件是否为 CloverPixel 隐藏 Mod
    ''' </summary>
    Public Function IsCloverPixelHiddenMod(filePath As String) As Boolean
        Try
            Dim fileName = Path.GetFileName(filePath)
            Return fileName.Equals(CloverPixelModFileName, StringComparison.OrdinalIgnoreCase)
        Catch
            Return False
        End Try
    End Function

End Module
