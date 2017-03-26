using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using TreParking_wpf.Model;

namespace TreParking_wpf.Windows
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Variables to hold data about last update and the labelLayer reference.
        /// </summary>
        private DateTime lastUpdate;

        private List<ParkingMapData> lastMapData;
        private MapLayer labelLayer;

        /// <summary>
        ///  Runs on startup. Creates and sets the variables defined above, and adds the MapLayer to the MapControl.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            lastMapData = new List<ParkingMapData>();
            labelLayer = new MapLayer();
            MapControl.Children.Add(labelLayer);
        }

        /// <summary>
        /// Loads XML file from the source URL, converts to JSON and converts the JSON to ParkingData object then updates the actual map control.
        /// </summary>
        private void UpdateParkingData()
        {
            labelLayer.Children.Clear();

            XmlDocument document = new XmlDocument();
            string xml = string.Empty;
            using (WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 })
            {
                // Loads the API URL from App.config file and downloads the contents of the page.
                xml = webClient.DownloadString(System.Configuration.ConfigurationManager.AppSettings["ParkingDataSourceURL"]); ;
            }
            document.LoadXml(xml);

            var json = JsonConvert.SerializeXmlNode(document);
            ParkingData.RootObject data = JsonConvert.DeserializeObject<ParkingData.RootObject>(json);

            foreach (var parkingFacility in data.d2LogicaModel.payloadPublication.genericPublicationExtension.parkingFacilityTablePublication.parkingFacilityTable.parkingFacility)
            {
                var status = data.d2LogicaModel.payloadPublication.genericPublicationExtension.parkingFacilityTableStatusPublication.parkingFacilityStatus.FindAll(x => x.parkingFacilityReference.id == parkingFacility.id);
                Location location = new Location(double.Parse(parkingFacility.entranceLocation.pointByCoordinates.pointCoordinates.latitude), double.Parse(parkingFacility.entranceLocation.pointByCoordinates.pointCoordinates.longitude));
                TextBlock textBlock = new TextBlock() { Text = parkingFacility.parkingFacilityName, ToolTip = string.Format("{0}\n{1}\nStatus: {2}\nUpdated at: {3}", parkingFacility.parkingFacilityName, parkingFacility.carParkDetails.value, string.Join(", ", status[0].parkingFacilityStatus), status[0].parkingFacilityStatusTime), Foreground = Brushes.Black };
                labelLayer.AddChild(textBlock, location, PositionOrigin.Center);
                ParkingMapData mapData = new ParkingMapData() { TextBlock = textBlock, ParkingFacility = parkingFacility, ParkingFacilityStatus = status };
                lastMapData.Add(mapData);
            }
            lastUpdate = DateTime.Now;
            LastUpdateLabel.Text = string.Format("Updated at {0} {1}", lastUpdate.ToShortDateString(), lastUpdate.ToLongTimeString());
        }

        /// <summary>
        /// Runs when the main window has loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateParkingData();
        }

        /// <summary>
        /// Event handler when the refresh button is clicked. Simply runs <see cref="UpdateParkingData"/>
        /// </summary>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateParkingData();
        }

        /// <summary>
        /// Logic code for filtering the map data. Hides specified data entries when the checkbox is checked and shows them again when they're unchecked.
        /// </summary>
        private void HideClosed_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var mapData in lastMapData)
            {
                foreach (var facilityStatus in mapData.ParkingFacilityStatus)
                {
                    if (facilityStatus.parkingFacilityStatus.ToString() == "closed")
                    {
                        mapData.TextBlock.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void HideClosed_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var mapData in lastMapData)
            {
                foreach (var facilityStatus in mapData.ParkingFacilityStatus)
                {
                    if (facilityStatus.parkingFacilityStatus.ToString() == "closed")
                    {
                        mapData.TextBlock.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void HideOpen_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var mapData in lastMapData)
            {
                foreach (var facilityStatus in mapData.ParkingFacilityStatus)
                {
                    if (facilityStatus.parkingFacilityStatus.ToString() == "open")
                    {
                        mapData.TextBlock.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void HideOpen_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var mapData in lastMapData)
            {
                foreach (var facilityStatus in mapData.ParkingFacilityStatus)
                {
                    if (facilityStatus.parkingFacilityStatus.ToString() == "open")
                    {
                        mapData.TextBlock.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void HideFull_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var mapData in lastMapData)
            {
                foreach (var facilityStatus in mapData.ParkingFacilityStatus)
                {
                    if (facilityStatus.parkingFacilityStatus.ToString() == "full")
                    {
                        mapData.TextBlock.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void HideFull_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var mapData in lastMapData)
            {
                foreach (var facilityStatus in mapData.ParkingFacilityStatus)
                {
                    if (facilityStatus.parkingFacilityStatus.ToString() == "full")
                    {
                        mapData.TextBlock.Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }
}