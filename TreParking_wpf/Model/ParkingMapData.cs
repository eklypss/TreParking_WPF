using System.Collections.Generic;
using System.Windows.Controls;
using static TreParking_wpf.Model.ParkingData;

namespace TreParking_wpf.Model
{
    /// <summary>
    /// Object that keeps the map data from the API and the map data glued together.
    /// </summary>
    public class ParkingMapData
    {
        public ParkingFacility ParkingFacility { get; set; }
        public List<ParkingFacilityStatu> ParkingFacilityStatus { get; set; } = new List<ParkingFacilityStatu>();
        public TextBlock TextBlock { get; set; }
    }
}