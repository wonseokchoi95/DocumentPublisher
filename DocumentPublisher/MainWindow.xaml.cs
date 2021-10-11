using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace DocumentPublisher
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        // TODO: 하드코딩되어 있으므로 지워야 함.
        private string recentDirectory = "C:\\Users\\cwss0";
        private List<String> mInputFiles = new List<String>();
        private String mOutputFileName = "output1";
        private String mOutputDirectory = "";
        private String mLatexEngine = "--pdf-engine=xelatex";
        private String mMainFont = "NanumMyeongjoOTF";
        private String mPaperSize = "a4";
        private String mResourcePath = "C:\\Users\\cwss0\\source\\repos\\Git\\Xinnos_2021-Business-Record\\";
        private String mTemplate = ""; 
        



        public MainWindow()
        {   
            InitializeComponent();
            
        }

        void item_Expanded(object sender, RoutedEventArgs e)
        {
            // as 키워드는 cast와 유사하게 형식을 변환하여 리턴하는 키워드. cast와 달리 형식을 변환할 수 없는 경우 null을 리턴한다.
            TreeViewItem itemParent = sender as TreeViewItem;
            if (itemParent == null) return;
            if (itemParent.Items.Count == 0) return;
            foreach (TreeViewItem item in itemParent.Items)
            {
                getSubDirectories(item);
            }

        }
        private void getSubDirectories(TreeViewItem item)
        {
            if (item == null) return;
            if (item.Items.Count != 0) return;

            try
            {
                // Tag: 전체 디렉터리 반환
                // Header: 폴더 이름 반환
                string strPath = item.Tag as string;
                DirectoryInfo directoryInfo = new DirectoryInfo(strPath);
                // 하위 디렉터리 검색
                foreach (DirectoryInfo child in directoryInfo.GetDirectories())
                {
                    TreeViewItem subItem = new TreeViewItem();
                    subItem.Header = child.Name;
                    subItem.Tag = child.FullName;
                    subItem.Expanded += new RoutedEventHandler(item_Expanded);
                    item.Items.Add(subItem);
                }
                // Markdown 형식의 파일 탐색
                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    if (fileInfo.Extension != ".MD" && fileInfo.Extension != ".md")
                        continue;
                    TreeViewItem subItem = new TreeViewItem();
                    subItem.Header = fileInfo.Name;
                    subItem.Tag = fileInfo.FullName;
                    subItem.Expanded += new RoutedEventHandler(item_Expanded);
                    item.Items.Add(subItem);
                }
            }
            catch (Exception e)
            {
                // 예외 처리 코드 추가 필요
            }
        }

        
        private void CompileButton_Click(object sender, RoutedEventArgs e)
        {
            // 출력 파일 명을 썼는지 확인

            // 파일 경로가 

            // pandoc 설치 여부 확인

            if (mInputFiles.Count != 0) mInputFiles.Clear();
            // DocumentTODViewer에 포함된 파일들을 가져옴(내부적으로는 파일 전체 경로)
            foreach(TreeViewItem item in listView_documents.Items)
            {
                String _inputFile = item.Tag.ToString();
                mInputFiles.Add(_inputFile);
            }

            CommandLine.CommandLineBuilder commandLineBuilder = new CommandLine.CommandLineBuilder(mInputFiles, mOutputFileName, mOutputDirectory);
            CommandLine commandLine = commandLineBuilder.setLatexEngine(mLatexEngine)
                                                        .setTemplate(mTemplate)
                                                        .setPaperSize(mPaperSize)
                                                        .setResourcePath(mResourcePath)
                                                        .setMainFont(mMainFont).Build();
            

            // 템플릿을 가져옴

            // CMD 창을 띄움(내용 정리할 것)
            System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
            System.Diagnostics.Process process = new System.Diagnostics.Process();

            processStartInfo.FileName = "cmd.exe";
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            process.StartInfo = processStartInfo;
            process.Start();
            MessageBox.Show(commandLine.Command);
            process.StandardInput.Write(commandLine.Command + Environment.NewLine);
            
            process.StandardInput.Close();
            try
            {
                String result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                MessageBox.Show(result);
            } catch(IOException ex)
            {
                MessageBox.Show("입출력 예외가 발생하였습니다.");
                process.Close();
            }catch (OutOfMemoryException ex)
            {
                MessageBox.Show("메모리가 부족하여 오류가 발생하였습니다.");
                process.Close();
            }
                
            
            

            // Pandoc을 실행하여 document 생성 (어느 디렉터리에?)

        }
        

        private bool isExtension(TreeViewItem treeViewItem)
        {
            if (!treeViewItem.Tag.ToString().Contains(".md")
                && !treeViewItem.Tag.ToString().Contains(".MD"))
                return false;
            else return true;
        }

        private bool isExist(TreeViewItem item)
        {
            foreach(TreeViewItem _item in listView_documents.Items)
            {
                if (item.Header == _item.Header)
                    return true;
                
            }
            return false;
        }

        private void DocumentListViewer_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Save_Workspace_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Add_Document_Click(object sender, RoutedEventArgs e)
        {
            GetFilePathByDialog();
        }

        private void GetFilePathByDialog()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".md";
            dialog.Filter = "Markdown files (*.md)|*.md|All files (*.*)|*.*";
            dialog.Multiselect = true;
            dialog.Title = "Markdown File Browser";
            dialog.ShowDialog();
            foreach(String file in dialog.FileNames)
            {
                TreeViewItem item = new TreeViewItem();
                item.Tag = file;
                item.Header = file.Substring(file.LastIndexOf("\\") + ("\\").Length);
                AddDocument(item);
            }
        }

        private void AddDocument(TreeViewItem selectedItem)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = (selectedItem as TreeViewItem).Header;
            item.Tag = (selectedItem as TreeViewItem).Tag;
            listView_documents.Items.Add(item);
        }

        private void MenuItem_Save_as_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (listView_documents.SelectedItem == null)
            {
                MessageBox.Show("선택된 파일이 없습니다");
                return;
            }
            listView_documents.Items.Remove(listView_documents.SelectedItem);
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            GetFilePathByDialog();
        }

        private void SearchOutputDirectory_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };
            commonOpenFileDialog.ShowDialog();
            if(commonOpenFileDialog.FileName != null)
                textBox_OutputDirectory.Text = commonOpenFileDialog.FileName;
        }

        private void SearchImageSource_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };
            commonOpenFileDialog.ShowDialog();
            if(commonOpenFileDialog.FileName != null)
                textBox_ImageSourceDirectory.Text = commonOpenFileDialog.FileName;
        }
    }
}
