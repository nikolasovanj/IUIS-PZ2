using MVVM3.Helpers;
using MVVMLight.Messaging;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public MyICommand<string> NavCommand { get; private set; }

        public static ObservableCollection<Entity> Entities { get; set; }
        private int count = 10; // Inicijalna vrednost broja objekata u sistemu
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

            Entities = new ObservableCollection<Entity>();

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
                        File.AppendAllText("../../Data/log.txt", $"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] - {incomming}\n");
                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji

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
            Console.WriteLine(entity.ID);
        }

    }
}
