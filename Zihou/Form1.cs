using System.Media;

namespace Zihou
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// (分*60+秒, (sound内の)ファイル名)
        /// </summary>
        internal Dictionary<int, string> plays = [];

        private void Form1_Load(object sender, EventArgs e)
        {
            Directory.CreateDirectory("sound");
            if (!File.Exists("config.csv"))
            {
                File.WriteAllText("config.csv", "再生時刻(mm:ss),パス(sound\\に続くファイル名)\n");
                var res = MessageBox.Show("config.csvに設定を入力してください。OKを押すと終了します。");
                Environment.Exit(0);
            }
            else
            {
                var config = File.ReadAllLines("config.csv").Skip(1).Where(x => x.Contains(','));
                if (!config.Any())
                {
                    var res = MessageBox.Show("config.csvに設定を入力してください。OKを押すと終了します。", "Zihou", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Environment.Exit(0);
                    return;
                }
                else
                {
                    foreach (var item in config)
                    {
                        var il = item.Split(',');
                        var time = il[0].Split(':').Select(int.Parse).ToArray();
                        plays.Add(time[0] * 60 + time[1], il[1]);
                        L_Config.Text += "\n"+item.Replace(",", " - ");
                    }
                }
            }
            PlayS.Enabled = true;
        }


        internal static SoundPlayer? player;
        private void PlayS_Tick(object sender, EventArgs e)
        {
            while (DateTime.Now.Millisecond > 800) ;
            PlayS.Enabled = false;
            PlayS.Interval = 1000 - DateTime.Now.Millisecond;
            PlayS.Enabled = true;
            if (plays.TryGetValue(DateTime.Now.Minute * 60 + DateTime.Now.Second, out string? path))
            {
                if (player != null)
                {
                    player.Stop();
                    player.Dispose();
                    player = null;
                }
                player = new SoundPlayer("sound\\" + path);
                player.Play();
            }
        }
    }
}
