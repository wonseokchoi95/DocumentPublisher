using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentPublisher
{
    class CommandLine
    {
        private String command;
        public List<String> mInputFiles = new List<String>();
        public String mOutputFileName;
        public String mFileDirectory;
        public String mOutputDirectory;

        // 필수적이지 않은 변수
        public String mPaperSize;
        public String mTemplate;
        public String mLatexEngine;
        public String mMainFont;
        public String mResourcePath;
        private CommandLine()
        { }
        private CommandLine(CommandLineBuilder commandLineBuilder)
        {
            /*
             * pandoc .\Scheduler.md -o Test.pdf -V mainfont="NanumMyeongjoOTF" --toc --pdf-engine=xelatex -V papersize=a4
             * 위의 명령문으로 조합하도록 구성
             */
            command = String.Format(@"pandoc {0} {4} -o {1}.pdf -V mainfont={2} {3}"
                             , generateInputFiles(commandLineBuilder.mInputFiles)
                             , commandLineBuilder.mOutputFileName
                             , commandLineBuilder.mMainFont
                             , commandLineBuilder.mLatexEngine
                             , commandLineBuilder.ResourcePath);
            

        }

        private String generateInputFiles(List<String> fileDirectories)
        {
            String mFileDirectories = "";
            foreach(String fileDirectory in fileDirectories)
            {
                mFileDirectories += @"""";
                mFileDirectories += fileDirectory;
                mFileDirectories += @"""";
                mFileDirectories += @" ";
            }
            return mFileDirectories;
        }

        public String Command
        {
            get { return command; }
        }

        // 빌더 패턴으로 작성
        public class CommandLineBuilder
        {
            // 반드시 필요한 변수
            public List<String> mInputFiles = new List<String>();
            public String mOutputFileName;
            public String mOutputDirectory;

            // 필수적이지 않은 변수
            public String mPaperSize;
            public String mTemplate;
            public String mLatexEngine;
            public String mMainFont;
            public String mResourcePath;

            public CommandLineBuilder() { }
            public CommandLineBuilder(List<String> inputFiles, String outputFileName, String outputDirectory)
            {
                mInputFiles = inputFiles;
                mOutputFileName = outputFileName;
                mOutputDirectory = outputDirectory;
            }

            public CommandLineBuilder setPaperSize(String paperSize)
            {
                mPaperSize = paperSize;
                return this;
            }

            public CommandLineBuilder setTemplate(String template)
            {
                mTemplate = template;
                return this;
            }

            public CommandLineBuilder setMainFont(String mainFont)
            {
                mMainFont = mainFont;
                return this;
            }
            public CommandLineBuilder setLatexEngine(String latexEngine)
            {
                mLatexEngine = latexEngine;
                return this;
            }
            public CommandLineBuilder setResourcePath(String directory)
            {
                mResourcePath = directory;
                return this;
            }
            public String ResourcePath
            {
                get {
                    if (mResourcePath != null)
                        return String.Format("--resource-path={0}", mResourcePath);
                    else return " ";
                }
            }

            public CommandLine Build()
            {
                return new CommandLine(this);
            }

        }
    }
}
