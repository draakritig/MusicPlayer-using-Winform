using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using WMPLib;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace MusicPlayer
{
    public partial class Form1 : Form
    {
        public static WindowsMediaPlayer wplayer = new WindowsMediaPlayer();
        private PlayList currObj;

        public Form1()
        {
            Thread t = new Thread(new ThreadStart(StartForm));
            t.Start();
            Thread.Sleep(2000);
            InitializeComponent();
            t.Abort();
        }
        public void StartForm()
        {
            Application.Run(new SplashScreen());
        }
        private void importMusicButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Multiselect = false;
                ofd.Filter = "All Supported Audio |*.mp3;*wma | MP3s |*.mp3 |WMAs|*.wma";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    wplayer.settings.autoStart = false;
                    wplayer.URL = ofd.FileName;
                }
            }
        }

        public void playButton_Click(object sender, EventArgs e)
        {
            wplayer.controls.play();
            playButton.Hide();
            pauseButton.Show();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            wplayer.controls.stop();
        }

        private void x_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void fastForwardButton_Click(object sender, EventArgs e)
        {
            wplayer.controls.fastForward();
        }

        private void fastReverseButton_Click(object sender, EventArgs e)
        {
            wplayer.controls.fastReverse();
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            wplayer.controls.pause();
            pauseButton.Hide();
            playButton.Show();
        }

        private void btnRedirect_Click(object sender, EventArgs e)
        {
            Program.playlist.Show();
            Program.musicplayer.Hide();
        }

        private void playListItems_DoubleClick(object sender, EventArgs e)
        {
            playListItems.Hide();

            ListBox sent = (ListBox)sender;
            object playListName = "";
            try
            {
                playListName = sent.SelectedItem;
            }
            catch (System.NullReferenceException n)
            {
                return;
            }
            PlayList currPlayList = PlayList.PlayLists[PlayList.PlaylistNames.IndexOf(playListName.ToString())];
            currObj = PlayList.PlayLists[PlayList.PlaylistNames.IndexOf(playListName.ToString())];
            songsList.Items.AddRange(currPlayList.songNames.ToArray());
            songsList.BringToFront();
            chkAutoPlay.Show();

            txtBListHeader.Text = playListName.ToString();
            txtBListHeader.ReadOnly = true;

            playListItems.Items.Clear();
            playListItems.Items.AddRange(PlayList.PlaylistNames.ToArray());
            chkAutoPlay.Checked = true;
            if (chkAutoPlay.Checked)
            {
                currObj.PlaySongs(this);
            }
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            string deserial = File.ReadAllText(@"playListData.json");
            List<List<string>> dataList = JsonSerializer.Deserialize<List<List<string>>>(deserial);


            //string playlistData = File.ReadAllText(@"playListData.txt");
            //List<string> dataList = playlistData.Split(',').ToList();
            PlayList playList1 = new PlayList(dataList[0], "First Play List");
            playListItems.Items.AddRange(PlayList.PlaylistNames.ToArray());
            chkAutoPlay.Hide();
        }

        private void songsList_DoubleClick(object sender, EventArgs e)
        {
            //converts the sender to the needed type
            ListBox sent = (ListBox)sender;
            wplayer.URL = @"Playlist\" + sent.SelectedItem.ToString();
            wplayer.controls.play();
            playButton.Hide();
            pauseButton.Show();
        }

        private void chkAutoPlay_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkAutoPlay.Checked)
            {
                stopButton_Click(this, e);
                pauseButton_Click(this, e);
            }
            else
            {
                currObj.PlaySongs(this);
            }
        }
    }
}
