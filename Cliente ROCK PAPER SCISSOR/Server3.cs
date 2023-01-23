using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Timers;
using System.Windows.Threading;

namespace Program
{
    public class TcpServer
    {
        public Boolean bClientConnected;
        private TcpListener server;
        private Boolean _isRunning;
        int i = 0;
        int Total_Bytes = 0 ;
        int u = 0;
        public TcpClient newClient = null;
        
        Semaphore semaphoreObject = new Semaphore(initialCount: 4, maximumCount: 6, name: "Pool");
        private static System.Timers.Timer aTimer;
        public ConcurrentQueue<string> queue = null;
        public ConcurrentQueue<string> queue2 = null;
        public bool RecebeuString=false;
        public bool siga2 = false;
        public String Player_2_Choice;
        int TimerPerRound = 5;
        public static Semaphore pila = new Semaphore(initialCount: 0, maximumCount: 1);
        public TcpServer(int port)
        {

            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            _isRunning = true;
            Thread k = new Thread(new ThreadStart(LoopClients));
            k.IsBackground = false;
            k.Start();

        }

        public TcpServer(int port, ConcurrentQueue<string> Fila_de_Respostas1, ConcurrentQueue<string> Fila_de_Respostas2, String Choice,ref Semaphore cona)
        {
            pila = cona;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timerTick;
            timer.Start();
            Player_2_Choice = Choice;
            queue2 = Fila_de_Respostas2;
                queue = Fila_de_Respostas1;
                server = new TcpListener(IPAddress.Any, port);

                
            server.Start();
                //MessageBox.Show("À espera que o cliente se connecte...");
                _isRunning = true;
                Thread k = new Thread(new ThreadStart(LoopClients));
                k.IsBackground = false;
                k.Start();
          
        }

        private void timerTick(object sender, EventArgs e)
        {
            TimerPerRound -= 1;

        }

        public void LoopClients()
        {
            
            while (_isRunning && i<2)
            {
             
                try
                {
                    
                    // wait for client connection
                    TcpClient newClient = server.AcceptTcpClient();
                    i++;
                    // client found.
                    // create a thread to handle communication
                    Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    t.IsBackground = false;
                    t.Start(newClient);
                    
                    
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Servidor Fechado!");
                }

            }

        }
        public Array Processar_Codigo_Teste(StreamReader leitor, string codigo)
        {
            char[] delimiterChars = { '-', ';' };
            string[] words = codigo.Split(delimiterChars);
            return words;

        }
        public void fechar(TcpServer server)
        {
            server.server.Stop();


        }



      

        public bool Escrever_Pedido_Cliente(StreamWriter escritor, int l, string Rps_Para_Cliente, Boolean bClientConnected)
        {
            try
            {

                escritor.WriteLine("A Mensagem Para o cliente{0} é:\n" + Rps_Para_Cliente, l);
                escritor.Flush();
            }
            catch (IOException e)
            {

                escritor.WriteLine("Tempo de espera por resposta ultrapassado");
                escritor.Flush();
                Console.WriteLine("Tempo de espera por resposta ultrapassado");
                bClientConnected = false;

            }
            return bClientConnected;
        }

        public bool Escrever_Servidor(StreamWriter escritor, StreamReader leitor, int l, Boolean bClientConnected )
        {
            Console.WriteLine("A Mensagem para o Servidor é:\n");

            String msg_Para_Server = null;

            try
            {
                while (msg_Para_Server == "" || msg_Para_Server == null)
                {
                    msg_Para_Server = leitor.ReadLine();
                }
                Console.WriteLine("Client {0}> " + msg_Para_Server, l);
            }
            catch (IOException e)
            {

                escritor.WriteLine("Tempo de espera por resposta ultrapassado");
                escritor.Flush();
                Console.WriteLine("Tempo de espera por resposta ultrapassado");
                bClientConnected = false;

                


            }
            return bClientConnected;
        }
        public bool Escrever_Cliente(StreamWriter escritor, int l, Boolean bClientConnected)
        {
            string lido = null;

            try
            {//Lê a mensagem escrita no terminal do servidor com o intuito de ser mandada para o cliente
                Console.WriteLine("A Mensagem Para o cliente{0} é:\n", l);
                lido = Console.ReadLine();

                // to write something back.
                escritor.WriteLine("{0}", lido);
                escritor.Flush();


            }
            catch (IOException e)
            {

                escritor.WriteLine("Tempo de espera por resposta ultrapassado");
                escritor.Flush();
                Console.WriteLine("Tempo de espera por resposta ultrapassado");
                bClientConnected = false;

            }
            return bClientConnected;

        }

        public bool Fields_Maquina (StreamWriter escritor, StreamReader leitor, int l, Boolean bClientConnected)
        {
            try
            {
               
                escritor.WriteLine("A Mensagem Para o cliente{0} é:\n" + "Blaze IT\n", l);
                escritor.Flush();
                string codigo = null;
                string[] Fields = new string[1024];

                while (codigo == "\n" && codigo == "\r" || codigo == null || codigo == "")
                {


                    codigo = leitor.ReadLine();
                }




                Array.Copy(Processar_Codigo_Teste(leitor, codigo), Fields, Processar_Codigo_Teste(leitor,codigo).Length);

                foreach (var word in Processar_Codigo_Teste(leitor, codigo))
                {
                    

                    System.Console.WriteLine($"<{word}>");
                }
               



            }
            catch (IOException e)
            {

                escritor.WriteLine("Tempo de espera por resposta ultrapassado");
                escritor.Flush();
                Console.WriteLine("Tempo de espera por resposta ultrapassado");
                bClientConnected = false;

            }

            return bClientConnected;


        }
        //private static void SetTimer()
        //{
        //    // Create a timer with a two second interval.
        //    aTimer = new System.Timers.Timer(30000);
        //    // Hook up the Elapsed event for the timer. 
        //    aTimer.Elapsed += OnTimedEvent;
        //    aTimer.AutoReset = true;
        //    aTimer.Enabled = true;
        //}

        //private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        //{
          
        //}

        public void HandleClient(object obj)
        {
            int j = i;   
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
                if (queue.Count > 0)
                {
                    queue.TryDequeue(out codigo);
                    while (codigo == "rock;1;\n\r" || codigo == "\r" || codigo == ""|| codigo == "scissor;1;\n\r" || codigo == "paper;1;\n\r")
                    {


                        jogadas = (string[])Processar_Codigo_Teste(sReader, codigo);
                        for (int l = 0; l < jogadas.Length - 1; l++)
                        {
                            if (jogadas[l] == "1")
                            {
                                sWriter.WriteLine(jogadas[l - 1]+"\n\r");
                                sWriter.Flush();
                                
                            }


                        }
                        //pila.WaitOne();
                        //queue2.TryPeek(out codigo2);
                        //if (codigo2=="JaEsta")
                        //{
                        //    queue2.TryDequeue(out codigo2);
                        //    Player_2_Choice = sReader.ReadLine();
                        //    queue2.Enqueue(Player_2_Choice);
                        //}
                        codigo = null;
                        //codigo2 = null;
                    }
                   
                }


                //if (TimerPerRound == 0)
                //{
                    
                    //Player_2_Choice = sReader.ReadLine();
                    //queue2.Enqueue(Player_2_Choice);

                    //TimerPerRound = 5;

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
                //                Player_2_Choice = sReader.ReadLine();
                                
                //            }


                //        }
                //        codigo2 = null;
                //    }


                //}

            }
            //server.Stop();
            //sWriter.WriteLine("Servidor Fechado!");
        }
    }
}