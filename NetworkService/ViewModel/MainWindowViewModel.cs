using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Helpers.Commands;
using NetworkService.Helpers.Display;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public MyICommand<string> NavCommand { get; private set; }
        public MyICommand<Window> CloseWindowCommand { get; private set; }

        public static ObservableCollection<Entity> Entities { get; set; }
        public static readonly string AddToken = "Add";
        public static readonly string RemoveToken = "Remove";
        public static ObservableCollection<DisplayItem> Slots { get; set; }
            = new ObservableCollection<DisplayItem>()
            {
                new DisplayItem(){X=0,  Y=0},
                new DisplayItem(){X=160,Y=0},
                new DisplayItem(){X=320,Y=0},
                new DisplayItem(){X=480,Y=0},
                new DisplayItem(){X=0,  Y=130},
                new DisplayItem(){X=160,Y=130},
                new DisplayItem(){X=320,Y=130},
                new DisplayItem(){X=480,Y=130},
                new DisplayItem(){X=0,  Y=260},
                new DisplayItem(){X=160,Y=260},
                new DisplayItem(){X=320,Y=260},
                new DisplayItem(){X=480,Y=260},
                new DisplayItem(){X=0,  Y=390},
                new DisplayItem(){X=160,Y=390},
                new DisplayItem(){X=320,Y=390},
                new DisplayItem(){X=480,Y=390}

            };
        public static ObservableCollection<EntityByType> EntitiesByType { get; set; } 
        public static ObservableCollection<DisplayItemConnection> Connections { get; set; } = new ObservableCollection<DisplayItemConnection>();
        public static ObservableCollection<EntityType> Types { get; } = new ObservableCollection<EntityType>
        {
            new EntityType("RTD", "../../Data/Images/RTD.png"),
            new EntityType("TC", "../../Data/Images/TC.png")
        };
        public static CommandStack EntitiesHistory { get; set; } = new CommandStack();
        public static CommandStack DisplayHistory { get; set; } = new CommandStack();
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
            CloseWindowCommand = new MyICommand<Window>(OnWindowClose);
            Entity e = new Entity { ID = 1, Name = "RTD-001", Type = Types[0] };
            Entity e1 = new Entity { ID = 2, Name = "TSP-001", Type = Types[1] };
            Entity e2 = new Entity { ID = 3, Name = "RTD-002", Type = Types[0] };
            Entity e3 = new Entity { ID = 4, Name = "TSP-002", Type = Types[1] };
            e.Value = 336;
            e.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 4);
            e.Value = 184;
            e.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 7);
            e.Value = 265;
            e.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 11);
            e.Value = 276;
            e.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 14);
            e.Value = 442;
            e.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 18);
            e1.Value = 206;
            e1.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 21);
            e2.Value = 302;
            e2.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 25);
            e3.Value = 442;
            e3.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 31);
            Entities = new ObservableCollection<Entity>() { e, e1, e2, e3 };
            count = Entities.Count;
            EntitiesByType = new ObservableCollection<EntityByType>(
                Entities.GroupBy(et => et.Type).Select(g => new EntityByType
                {
                    Type = g.Key,
                    Entities = new ObservableCollection<Entity>(g)
                })
            );
            networkDisplayViewModel = new NetworkDisplayViewModel();
            networkEntitiesViewModel = new NetworkEntitiesViewModel();
            measurementGraphViewModel = new MeasurementGraphViewModel();
            currentViewModel = networkEntitiesViewModel;

            Messenger.Default.Register<Entity>(this, AddToken, AddToList);
            Messenger.Default.Register<Entity>(this, RemoveToken, RemoveFromList);
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
                            Entities[result.index].Value = result.value;
                            Entities[result.index].TimeStamp = DateTime.Now;
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
        private void OnWindowClose(Window window)
        {
            window.Close();
        }
        private void AddToList(Entity entity)
        {
            Entities.Add(entity);
            AddToCollectionByType(entity);
            count++;
        }
        private void RemoveFromList(Entity entity)
        {
            Entities.Remove(entity);
            RemoveFromCollectionByType(entity);
            foreach(var slot in Slots)
            {
                if(slot.Entity != null && slot.Entity.ID == entity.ID)
                {
                    List<int> idxs = CheckForConnection(slot);
                    if (idxs.Count > 0)
                    {
                        Disconnect(slot);
                    }
                    slot.Clear();
                    DisplayHistory.Clear();
                }
            }
            count--;
        }
        private (int index, int value) ParseIncomming(string incomming)
        {
            string[] parts = incomming.Split(new char[] { ':', '_' });
            return (int.Parse(parts[1]), int.Parse(parts[2]));
        }
        private void AddToCollectionByType(Entity entity)
        {
            int groupIndex = entity.Type.Name == "RTD" ? 0 : 1;
            EntitiesByType[groupIndex].Entities.Add(entity);
        }
        private void RemoveFromCollectionByType(Entity entity)
        {
            int groupIndex = entity.Type.Name == "RTD" ? 0 : 1;
            EntitiesByType[groupIndex].Entities.Remove(entity);
        }
        private List<int> CheckForConnection(DisplayItem item)
        {
            List<int> idxs = new List<int>();
            foreach (var conn in Connections)
            {
                if (conn.Item1.Entity.ID == item.Entity.ID || conn.Item2.Entity.ID == item.Entity.ID)
                {
                    idxs.Add(Connections.IndexOf(conn));
                }
            }
            return idxs;
        }
        private void Disconnect(DisplayItem item)
        {
            for (int i = 0; i < Connections.Count; i++)
            {
                if (Connections[i].Item1.Entity.ID == item.Entity.ID || Connections[i].Item2.Entity.ID == item.Entity.ID)
                {
                    Connections.RemoveAt(i);
                }
            }
        }
    }
}
