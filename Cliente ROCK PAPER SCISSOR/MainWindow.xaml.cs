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
using Program;
using System.ComponentModel;
using System.Windows.Threading;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;

namespace Cliente_ROCK_PAPER_SCISSOR
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {

        int rounds = 3;
        int TimerPerRound = 30;
        bool gameOver = false;
        int Player_1_Score=0;
        int Player_2_Score=0;
        string Player_1_Choice;
        string Player_2_Choice;
        int Rounds = 0;
        string[] Dados_Jogo = new string[5];
        private Socket sock;
        private TcpListener server = null;
        private TcpClient _client;

        private Boolean _isConnected;
        public readonly BackgroundWorker worker = new BackgroundWorker();
        private Boolean ready2;
        private Boolean ready1;
        private Boolean carregados = false;

        Semaphore semaphoreObject = new Semaphore(initialCount: 1, maximumCount: 1);
        ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        ConcurrentQueue<string> queue2 = new ConcurrentQueue<string>();
        Boolean Hoster = false;
        private static Semaphore _pool = new Semaphore(initialCount: 0, maximumCount: 1);
        public static Semaphore cona = new Semaphore(initialCount: 0, maximumCount: 1);
        public static Semaphore Pedro = new Semaphore(initialCount: 0, maximumCount: 1);
        public MainWindow()
        {
            InitializeComponent();

            BackgroundWorker worker = new BackgroundWorker();
            BackgroundWorker MessageReceiver = new BackgroundWorker();
            MessageReceiver.DoWork += MessageReceiver_DoWork;
            MessageReceiver.RunWorkerAsync();

            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();

        }
        private void MessageReceiver_DoWork(object sender, DoWorkEventArgs e)
        {


        }
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            // run all background tasks here
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {

            while (true)
            {

                if (_isConnected == true)
                {
                    //ReceiveMove();
                    RM();
                }
                //if (Hoster == true)
                //{

                //    HM();

                //}

                Dados_Jogo[0] = Player_1_Choice;
                Dados_Jogo[1] = Player_2_Choice;
                Dados_Jogo[2] = Player_1_Score.ToString();
                Dados_Jogo[3] = Player_2_Score.ToString();
                Dados_Jogo[4] = rounds.ToString();

                if (Dados_Jogo[0] != "none" && Dados_Jogo[1] != "none" && carregados == true)
                {
                    switch (Dados_Jogo[1])
                    {
                        case "rock":

                            Application.Current.Dispatcher.BeginInvoke(
                             DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 Player_2_Image.Source = new BitmapImage(new Uri(@"rock.jpg", UriKind.Relative));
                             }));
                            try
                            {
                                _pool.Release();
                            }
                            catch (Exception )
                            {

                            }
                            //Player_2_Choice = "rock";
                            break;
                        case "paper":

                            Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(() =>
                            {
                                Player_2_Image.Source = new BitmapImage(new Uri(@"paper.jpg", UriKind.Relative));
                            }));
                            try
                            {
                                _pool.Release();
                            }
                            catch (Exception )
                            {

                            }

                            //Player_2_Choice = "paper";
                            break;
                        case "scissor":


                            Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(() =>
                            {
                                Player_2_Image.Source = new BitmapImage(new Uri(@"scissors.jpg", UriKind.Relative));
                            }));
                            try
                            {
                                _pool.Release();
                            }
                            catch (Exception )
                            {

                            }

                            //Player_2_Choice = "scissor";
                            break;
                    }
                    switch (Dados_Jogo[0])
                    {
                        case "rock":

                            Application.Current.Dispatcher.BeginInvoke(
                             DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 Player_1_Image.Source = new BitmapImage(new Uri(@"rock.jpg", UriKind.Relative));
                             }));
                            try
                            {
                                _pool.Release();
                            }
                            catch (Exception )
                            {

                            }
                            //Player_2_Choice = "rock";
                            break;
                        case "paper":

                            Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(() =>
                            {
                                Player_1_Image.Source = new BitmapImage(new Uri(@"paper.jpg", UriKind.Relative));
                            }));
                            try
                            {
                                _pool.Release();
                            }
                            catch (Exception )
                            {

                            }

                            //Player_2_Choice = "paper";
                            break;
                        case "scissor":

                            Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(() =>
                            {
                                Player_1_Image.Source = new BitmapImage(new Uri(@"scissors.jpg", UriKind.Relative));
                            }));
                            try
                            {
                                _pool.Release();
                            }
                            catch (Exception )
                            {

                            }
                            //Player_2_Choice = "scissor";
                            break;
                    }

                    checkGame(Dados_Jogo);
                    Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(() =>
                            {
                                Score.Text = "Player1: " + Player_1_Score + " - Player2: " + Player_2_Score;
                                Round.Text = "Round: " + Rounds;
                            }));
                   
                }

            }
        }
        private void Rock1(object sender, RoutedEventArgs e)
        {

            byte[] num = { 1 };
            queue.Enqueue("rock;1;\n\r");
            //queue2.Enqueue("rock;2;\n\r");
            //sock.Send(num);
            Player_1_Image.Source = new BitmapImage(new Uri(@"rock.jpg", UriKind.Relative));
            Player_1_Choice = "rock";
            ready1 = true;
            //try
            //{
            //    Pedro.Release();
            //}
            //catch (Exception)
            //{
            //}
        }


        private void Paper1(object sender, RoutedEventArgs e)
        {
            byte[] num = { 2 };
            queue.Enqueue("paper;1;\n\r");
            //queue2.Enqueue("paper;2;\n\r");
            //sock.Send(num);
            Player_1_Image.Source = new BitmapImage(new Uri(@"paper.jpg", UriKind.Relative));
            Player_1_Choice = "paper";
            ready1 = true;
            //try
            //{
            //    Pedro.Release();
            //}
            //catch (Exception)
            //{
            //}
        }

        private void Scissor1(object sender, RoutedEventArgs e)
        {
            byte[] num = { 3 };
            queue.Enqueue("scissor;1;\n\r");
            //queue2.Enqueue("scissor;2;\n\r");
            //sock.Send(num);
            Player_1_Image.Source = new BitmapImage(new Uri(@"scissors.jpg", UriKind.Relative));
            Player_1_Choice = "scissor";
            ready1 = true;
            //try
            //{
            //    Pedro.Release();
            //}
            //catch (Exception)
            //{
            //}
        }




        private void nextRound(string[] Dados_Jogo)
        {
            _pool.WaitOne();
            Dados_Jogo[0] = "none";
            Dados_Jogo[1] = "none";
            Player_1_Choice = "none";
            Player_2_Choice = "none";
            Thread.Sleep(500);

            Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(() =>
            {
                Player_1_Image.Source = new BitmapImage(new Uri(@"qq.jpg", UriKind.Relative));
                Player_2_Image.Source = new BitmapImage(new Uri(@"qq.jpg", UriKind.Relative));
            }));


        }

        public string[] checkGame(string[] Dados_Jogo)
        {

            if (Dados_Jogo[0] != "none" && Dados_Jogo[1] != "none")
            {

                if (Dados_Jogo[0] == "rock" && Dados_Jogo[1] == "paper")
                {
                    //MessageBox.Show("Player 1 Wins");
                    Player_2_Score++;
                    rounds--;
                    Rounds++;
                    nextRound(Dados_Jogo);
                }
                else if (Dados_Jogo[0] == "paper" && Dados_Jogo[1] == "rock")
                {
                    //MessageBox.Show("Player 2 Wins");
                    Player_1_Score++;
                    rounds--;
                    Rounds++;
                    nextRound(Dados_Jogo);
                }
                else if (Dados_Jogo[0] == "paper" && Dados_Jogo[1] == "scissor")
                {
                    //MessageBox.Show("Player 2 Wins");
                    Player_2_Score++;
                    rounds--;
                    Rounds++;
                    nextRound(Dados_Jogo);
                }
                else if (Dados_Jogo[0] == "scissor" && Dados_Jogo[1] == "paper")
                {
                    //MessageBox.Show("Player 1 Wins");
                    Player_1_Score++;
                    rounds--;
                    Rounds++;
                    nextRound(Dados_Jogo);
                }
                else if (Dados_Jogo[0] == "scissor" && Dados_Jogo[1] == "rock")
                {
                    //MessageBox.Show("Player 2 Wins");
                    Player_2_Score++;
                    rounds--;
                    Rounds++;
                    nextRound(Dados_Jogo);
                }
                else if (Dados_Jogo[0] == "rock" && Dados_Jogo[1] == "scissor")
                {
                    //MessageBox.Show("Player 1 Wins");
                    Player_1_Score++;
                    rounds--;
                    Rounds++;
                    nextRound(Dados_Jogo);
                }
                else if (Dados_Jogo[0] == "none")
                {
                    //MessageBox.Show(" Player 1 Make a choice");
                    nextRound(Dados_Jogo);

                }
                else if (Dados_Jogo[1] == "none")
                {
                    //MessageBox.Show(" Player 2 Make a choice");
                    nextRound(Dados_Jogo);


                }
                else
                {
                    //MessageBox.Show("Draw");
                    nextRound(Dados_Jogo);

                }

            }
            return Dados_Jogo;
        }







        private void Host(object sender, RoutedEventArgs e)
        {

            TcpServer Servidor = new TcpServer(50000, queue,queue2, Player_2_Choice,ref cona);
            Hoster = true;
            
            //int port;
            //Int32.TryParse(HostText.Text, out port);
            ////IPAddress localAddr = IPAddress.Parse("localhost");

            //IPAddress ipAddress = Dns.Resolve("localhost").AddressList[0];

            //try
            //{
            //    server = new TcpListener(ipAddress, 50000);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("" + ex);
            //}

            //server.Start();
            //sock = server.AcceptSocket();
            //_isConnected = true;
            //TimerPerRound = 30;
        }
        //private void ReceiveMove()
        //{
        //    byte[] buffer = new byte[1];
        //    sock.Receive(buffer);

        //        if (buffer[0] == 1)
        //            Player_2_Choice = "rock";
        //            ready2 = true;
        //            carregados = true;
        //        if (buffer[0] == 2)
        //            Player_2_Choice = "paper";
        //            carregados = true;
        //            ready2 = true;
        //        if (buffer[0] == 3)
        //            Player_2_Choice = "scissor";
        //            carregados = true;
        //            ready2 = true;
        //        if (buffer[0] == 0)
        //            Player_2_Choice = "none";
        //            ready2 = true;
        //            carregados = true;
        //    buffer[0] = 0;


        //}
        private void RM()
        {

        
                StreamWriter sWriter = new StreamWriter(_client.GetStream(), Encoding.ASCII);
                StreamReader sReader = new StreamReader(_client.GetStream(), Encoding.ASCII);

                if (sReader.Peek() != -1)
                {

                    string t = sReader.ReadLine();

                    switch (t)
                    {
                        case "rock":
                            Player_2_Choice = "rock";
                            carregados = true;
                        sWriter.WriteLine(Player_1_Choice);
                        sWriter.Flush();
                        //cona.Release();
                        break;
                        case "paper":
                            Player_2_Choice = "paper";
                            carregados = true;
                        sWriter.WriteLine(Player_1_Choice);
                        sWriter.Flush();
                        //cona.Release();
                        break;

                        case "scissor":
                            Player_2_Choice = "scissor";
                            carregados = true;
                        sWriter.WriteLine(Player_1_Choice);
                        sWriter.Flush();
                        //cona.Release();
                        break;
                        default:
                            Player_2_Choice = "none";
                            break;
                    }
                    t = null;

                    sReader.DiscardBufferedData();
                //Player_2_Choice = sReader.ReadLine();
                //sReader.DiscardBufferedData();
                //Pedro.WaitOne();
                //sWriter.WriteLine(Player_1_Choice);
                //sWriter.Flush();
                //cona.Release();
            }
            
         

            //try
            //{
            //    _pool.Release();
            //}
            //catch (Exception e)
            //{

            //}
        }

        private void HM()
        {
            string l = null;
            if (queue2.Count() == 0)
            {
                queue2.Enqueue("JaEsta");


            }
            queue2.TryPeek(out l);
            if (l!= "JaEsta")
            {
                
                queue2.TryDequeue(out Player_2_Choice);

            }


        }
        private void Connect(object sender, RoutedEventArgs e)
        {
            try
            {
                //ClientDemo _client = new ClientDemo("localhost", 50000);
                _client = new TcpClient();
                _client.Connect("localhost", 50000);




                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.IsBackground = false;
                t.Start(_client);





                //TcpClient _client;
                //_client = new TcpClient();
                //_client.Connect("localhost", 50000);
                //TcpClient _client = default(TcpClient);

                //int port;
                //string ip;
                //Int32.TryParse(HostText.Text, out port);
                //ip = IPText.ToString();
                //_client.Connect("localhost", port);
                ////sock = _client.Client;
                _isConnected = true;
                TimerPerRound = 30;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();

            }


        }
        public Array Processar_Codigo_Teste(StreamReader leitor, string codigo)
        {
            char[] delimiterChars = { '-', ';' };
            string[] words = codigo.Split(delimiterChars);
            return words;

        }
        public void HandleClient(object obj)
        {

            TcpClient client = (TcpClient)obj;
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            Boolean bClientConnected = true;

            while (bClientConnected)
            {
                //client.SendTimeout = 500000;
                //client.ReceiveTimeout = 500000;
                string codigo = null;
                string codigo2 = null;
                string[] jogadas2 = new string[1000];
                string[] jogadas = new string[1000];
             
                //if (queue2.Count > 0)
                //{
                //    queue2.TryDequeue(out codigo);
                //    while (codigo2 == "rock;2;\n\r" || codigo2 == "\r" || codigo2 == "" || codigo2 == "scissor;2;\n\r" || codigo2 == "paper;2;\n\r")
                //    {


                //        jogadas2 = (string[])Processar_Codigo_Teste(sReader, codigo2);
                //        for (int l = 0; l < jogadas2.Length - 1; l++)
                //        {
                //            if (jogadas[l] == "2")
                //            {
                //                sWriter.WriteLine(jogadas2[l - 1] + "\n\r");
                //                sWriter.Flush();

                //            }


                //        }
                //        codigo = null;
                //    }


                //}
                //if (queue2.Count > 0)
                //{
                //    queue2.TryDequeue(out codigo2);
                //    while (codigo2 == "rock;2;\n\r" || codigo2 == "\r" || codigo2 == "" || codigo2 == "scissor;2;\n\r" || codigo2 == "paper;2;\n\r")
                //    {


                //        jogadas2 = (string[])Processar_Codigo_Teste(sReader, codigo2);
                //        for (int l = 0; l < jogadas2.Length - 1; l++)
                //        {
                //            if (jogadas2[l] == "2")
                //            {
                //                sReader.ReadLine();

                //            }


                //        }
                //        codigo2 = null;
                //    }


                //}


            }
            server.Stop();
            sWriter.WriteLine("Servidor Fechado!");
        }

    }
}




