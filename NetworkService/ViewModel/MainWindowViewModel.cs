using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Helpers.Commands;
using NetworkService.Helpers.Display;
using NetworkService.Model;
using Notification.Wpf;
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
        private NotificationManager notificationManager = new NotificationManager();
        public static ObservableCollection<Entity> Entities { get; set; }
        public static readonly string AddToken = "Add";
        public static readonly string RemoveToken = "Remove";
        public static ObservableCollection<DisplayItem> Slots { get; set; }
        public static ObservableCollection<EntityByType> EntitiesByType { get; set; } 
        public static ObservableCollection<DisplayItemConnection> Connections { get; set; } = new ObservableCollection<DisplayItemConnection>();
        public static ObservableCollection<EntityType> Types { get; private set; } 
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
            LoadInitialData();
            networkDisplayViewModel = new NetworkDisplayViewModel();
            networkEntitiesViewModel = new NetworkEntitiesViewModel();
            measurementGraphViewModel = new MeasurementGraphViewModel();
            currentViewModel = networkEntitiesViewModel;

            Messenger.Default.Register<Entity>(this, AddToken, AddToList);
            Messenger.Default.Register<Entity>(this, RemoveToken, RemoveFromList);
            Messenger.Default.Register<NotificationContent>(this, ShowToastNotification);
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
                        try 
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
                        }
                        catch
                        {
                            Trace.Write("Error in communication trying again");
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
        private void ShowToastNotification(NotificationContent notificationContent)
        {
            notificationManager.Show(notificationContent, "WindowNotificationArea");
        }

        private void LoadInitialData() 
        { 
            Slots = new ObservableCollection<DisplayItem>()
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
            Types = new ObservableCollection<EntityType>
            {
                new EntityType("RTD", "../../Data/Images/RTD.png"),
                new EntityType("TC", "../../Data/Images/TC.png")
            };
            Entity e1 = new Entity { ID = 1, Name = "RTD-001", Type = Types[0] };
            Entity e2 = new Entity { ID = 3, Name = "RTD-002", Type = Types[0] };
            Entity e3 = new Entity { ID = 4, Name = "TSP-002", Type = Types[1] };
            Entity e4 = new Entity { ID = 2, Name = "TSP-001", Type = Types[1] };
            e1.Value = 336;
            e1.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 4);
            e1.Value = 184;
            e1.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 7);
            e1.Value = 265;
            e1.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 11);
            e1.Value = 276;
            e1.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 14);
            e1.Value = 442;
            e1.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 18);

            e2.Value = 234;
            e2.TimeStamp = new DateTime(2026, 06, 11, 10, 44, 4);
            e2.Value = 356;
            e2.TimeStamp = new DateTime(2026, 06, 11, 10, 44, 7);
            e2.Value = 434;
            e2.TimeStamp = new DateTime(2026, 06, 11, 10, 44, 11);
            e2.Value = 163;
            e2.TimeStamp = new DateTime(2026, 06, 11, 10, 44, 14);
            e2.Value = 354;
            e2.TimeStamp = new DateTime(2026, 06, 11, 10, 44, 18);

            e3.Value = 234;
            e3.TimeStamp = new DateTime(2026, 06, 12, 19, 04, 4);
            e3.Value = 342;
            e3.TimeStamp = new DateTime(2026, 06, 12, 19, 04, 7);
            e3.Value = 342;
            e3.TimeStamp = new DateTime(2026, 06, 12, 19, 04, 11);
            e3.Value = 346;
            e3.TimeStamp = new DateTime(2026, 06, 12, 19, 04, 14);
            e3.Value = 432;
            e3.TimeStamp = new DateTime(2026, 06, 12, 19, 04, 18);

            e4.Value = 321;
            e4.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 4);
            e4.Value = 323;
            e4.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 7);
            e4.Value = 346;
            e4.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 11);
            e4.Value = 232;
            e4.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 14);
            e4.Value = 443;
            e4.TimeStamp = new DateTime(2026, 06, 10, 19, 44, 18);

            Entities = new ObservableCollection<Entity>() { e1, e2, e3, e4 };
            count = Entities.Count;
            EntitiesByType = new ObservableCollection<EntityByType>(
                Entities.GroupBy(et => et.Type).Select(g => new EntityByType
                {
                    Type = g.Key,
                    Entities = new ObservableCollection<Entity>(g)
                })
            );
        }
    }
}
