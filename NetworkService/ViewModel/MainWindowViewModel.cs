using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public MyICommand<string> NavCommand { get; private set; }

        public static ObservableCollection<Entity> Entities { get; set; }
        private int count = 1; // Inicijalna vrednost broja objekata u sistemu
                               // ######### ZAMENITI stvarnim brojem elemenata
                               //           zavisno od broja entiteta u listi
        public NetworkEntitiesViewModel networkEntitiesViewModel;
        public NetworkDisplayViewModel networkDisplayViewModel;
        public MeasurementGraphViewModel measurementGraphViewModel;
        private BindableBase currentViewModel;
        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }
        public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom
            NavCommand = new MyICommand<string>(OnNav);
            Entity e = new Entity { ID = 1, Name = "RTD-001", Type = new EntityType("RTD", "/")};
            e.Value = 336;
            e.Value = 184;
            e.Value = 265;
            e.Value = 276;
            e.Value = 442;
            Entities = new ObservableCollection<Entity>() { e };

            networkDisplayViewModel = new NetworkDisplayViewModel();
            networkEntitiesViewModel = new NetworkEntitiesViewModel();
            measurementGraphViewModel = new MeasurementGraphViewModel();
            currentViewModel = networkEntitiesViewModel;

            Messenger.Default.Register<Entity>(this, AddToList);
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Trace.WriteLine(incomming); //Na primer: "Entitet_1:272"
                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji
                            var result = ParseIncomming(incomming);
                            Entities[result.index].Value=result.value;
                            File.AppendAllText("../../Data/log.txt", Entities[result.index].ToLog());
                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }
        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "entity":
                    CurrentViewModel = networkEntitiesViewModel;
                    break;
                case "display":
                    CurrentViewModel = networkDisplayViewModel;
                    break;
                case "graph":
                    CurrentViewModel = measurementGraphViewModel;
                    break;
            }
        }

        private void AddToList(Entity entity)
        {
            Entities.Add(entity);
        }

        private (int index, int value) ParseIncomming(string incomming)
        {
            string[] parts = incomming.Split(new char[] {':', '_'});
            return (int.Parse(parts[1]), int.Parse(parts[2]));
        }

    }
}
