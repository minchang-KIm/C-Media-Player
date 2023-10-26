using System;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using WMPLib;
using System.Drawing;
using AxWMPLib;

namespace VideoPlayerApp
{
    public partial class MainForm : Form
    {
        Button playPauseButton; // 재생/일시정지 버튼
        Button stopButton; // 정지 버튼
        TrackBar speedTrackBar; // 속도 조절 슬라이더
        TrackBar trackBar; //트랙 바 추가
        bool isPlaying = true; // 재생 중 여부를 나타내는 플래그
        

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();
            LoadVideo();
        }
        public MainForm(string videoFilePath)
        {
            InitializeComponent();
            InitializeUI();
            LoadVideo(videoFilePath);
        }

        private void InitializeUI()
        {
            // 재생/일시정지 버튼 초기화
            playPauseButton = new Button();
            playPauseButton.Text = "일시정지";
            playPauseButton.Click += PlayPauseButton_Click;
            playPauseButton.Location = new Point(10, 20); // 버튼 위치 조정
            Controls.Add(playPauseButton);
            playPauseButton.Parent =splitContainer1.Panel2;

            // 정지 버튼 초기화
            stopButton = new Button();
            stopButton.Text = "정지";
            stopButton.Click += StopButton_Click;
            stopButton.Location = new Point(100, 20); // 버튼 위치 조정
            Controls.Add(stopButton);
            stopButton.Parent =splitContainer1.Panel2;

            // 속도 조절 슬라이더 초기화
            speedTrackBar = new TrackBar();
            speedTrackBar.Minimum = 1; // 최소 배속
            speedTrackBar.Maximum = 4; // 최대 배속
            speedTrackBar.Value = 1; // 초기 배속
            speedTrackBar.TickFrequency = 1;
            speedTrackBar.LargeChange = 1;
            speedTrackBar.SmallChange = 1;
            speedTrackBar.Location = new Point(200, 20); // 슬라이더 위치 조정
            speedTrackBar.Scroll += SpeedTrackBar_Scroll;
            Controls.Add(speedTrackBar);
            speedTrackBar.Parent =splitContainer1.Panel2;

            //플레이어 기본 UI 안보이게
            player.uiMode = "None";

            trackBar = new TrackBar();
            trackBar.Minimum = 0;
            trackBar.Maximum = 100;
            trackBar.Value = 0;
            trackBar.TickFrequency = 1;
            trackBar.LargeChange = 10;
            trackBar.SmallChange = 1;
            trackBar.Location = new Point(300, 10);
            //trackBar.Width = 700;
            trackBar.Dock = DockStyle.Fill;
            Controls.Add(trackBar);
            trackBar.Parent =splitContainer1.Panel2;

            // 트랙 바 이벤트 핸들러 추가
            trackBar.Scroll += TrackBar_Scroll;
        }

        private void LoadVideo(string videoFilePath = "None")
        {
            //Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(videoFilePath);
            
            if (videoFilePath == "None")
            {
                MessageBox.Show("영상 파일을 찾을 수 없습니다. 영상 파일 위치를 선택해주세요");
                OpenFileDialog openFileDialog = new OpenFileDialog();
                //openFileDialog.Filter = "Video Files|*.mp4;*.avi;*.mkv|All Files|*.*";
                openFileDialog.Filter = "Video Files|*.wmv";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    videoFilePath = openFileDialog.FileName;
                    LoadVideo(videoFilePath);
                }
                else
                {
                    return;
                }
            }
            player.URL = videoFilePath;
            //player.URL = videoFilePath;
        }
        

        private void PlayPauseButton_Click(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                // 일시정지 상태로 변경
                player.Ctlcontrols.pause();
                playPauseButton.Text = "재생";
            }
            else
            {
                // 재생 상태로 변경
                player.Ctlcontrols.play();
                playPauseButton.Text = "일시정지";
            }

            isPlaying = !isPlaying; // 재생 상태를 반전
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                // 정지 상태로 변경
                player.Ctlcontrols.stop();
                player.Ctlcontrols.currentPosition = 0;  //맨 앞으로 이동
                playPauseButton.Text = "재생";
                isPlaying = false;
            }
        }

        private void SpeedTrackBar_Scroll(object sender, EventArgs e)
        {
            // 속도 조절 슬라이더의 값이 변경될 때마다 배속 업데이트
            player.settings.rate = speedTrackBar.Value;
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            // 트랙 바 이벤트 핸들러
            double newPosition = (double)trackBar.Value / 100;
            player.Ctlcontrols.currentPosition = newPosition * player.currentMedia.duration;
        }

        private void TrackBarUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (player.currentMedia != null)
            {
                // 현재 재생 위치를 트랙 바에 업데이트
                double currentPosition = player.Ctlcontrols.currentPosition;
                double totalDuration = player.currentMedia.duration;
                int trackBarValue = (int)(currentPosition / totalDuration * 100);
                trackBar.Value = trackBarValue;
            }
        }
    }
}
